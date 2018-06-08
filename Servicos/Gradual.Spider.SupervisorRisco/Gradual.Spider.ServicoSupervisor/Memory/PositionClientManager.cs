﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Collections.Concurrent;
using Gradual.Spider.SupervisorRisco.Lib.Dados;
using Gradual.Spider.SupervisorRisco.DB.Lib;
using Gradual.Spider.SupervisorRisco.DB.Lib.Persistencia;
using System.Threading;
using System.Configuration;
using Gradual.Core.Spider.OrderFixProcessing.Lib.Dados;
using Gradual.Spider.SupervisorRisco.Lib.Util;
using Gradual.Spider.ServicoSupervisor.Calculator;
using Gradual.Spider.SupervisorRisco.Lib.Handlers;

namespace Gradual.Spider.ServicoSupervisor.Memory
{
    public class PositionClientManager
    {

        public static readonly log4net.ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        object _sync = new object();

        // Novos Limites / PositionClient
        // protected ConcurrentDictionary<int, List<PosClientSymbolInfo>> _dicPositionClient;
        public ConcurrentDictionary<int, ConcurrentDictionary<string, PosClientSymbolInfo>> _dicPositionClient;
        //protected ConcurrentDictionary<int, List<ExecSymbolInfo>> _dicExecSymbols;
        protected ConcurrentDictionary<int, ConcurrentDictionary<int, ExecSymbolInfo>> _dicExecSymbols;
        protected ConcurrentDictionary<int, SaldoCcInfo> _dicSaldoCc;
        protected ConcurrentDictionary<string, List<PosClientSymbolInfo>> _dicPosClientBySymbol;
        // protected ConcurrentDictionary<string, SymbolInfo> _dicSymbols;

        DbRisco _db = null;
        DbRiscoOracle _DbOracle = null;
        ConcurrentQueue<TOPosClient> _cqPosClient;
        object _syncPosClient = new object();
        Thread _thPosClient;
        Thread _thPosDb;
        Thread _thPosAbertura;
        Thread _thPosContaCorrenteAbertura;

        /// <summary>
        /// Dictionary que guarda o Codigo Bovespa Principal do cliente e seu Mutex.
        /// IMPORTANTE - Têm a Finalidade de controle de como vai ser calculado, como intervalo de recalculo, status de processamento
        /// </summary>
        private ConcurrentDictionary<int, ClienteMutexInfo> htClientes = new ConcurrentDictionary<int, ClienteMutexInfo>();

        /// <summary>
        /// Objeto com a Lista de cliente que são monitorados para calcular o risco/Prejuizo um por um.
        /// Têm a finalidade de guardar os clientes que irão ser calculados em cada rodada.
        /// </summary>
        private List<int> ClientesMonitor = new List<int>();

        /// <summary>
        /// Thread Principal de clientes que têm a finalidade de rodar o calculo das posições  para 
        /// definir o lucro prejuizo dos clientes
        /// </summary>
        private Thread thThreadClientes;

        /// <summary>
        /// Retorna o Intervalo de calculo pronto (em milisegundos) para cada CLIENTE
        /// Têm a finalidade de assegurar que se não já nada configurado para o Intervalo de recalculo a propriedade 
        /// irá setar 01 minuto
        /// </summary>
        private int IntervaloRecalculo
        {
            get
            {
                int intervaloRecalculo = 60000;

                if (ConfigurationManager.AppSettings["IntervaloRecalculo"] != null)
                {
                    intervaloRecalculo = Convert.ToInt32(ConfigurationManager.AppSettings["IntervaloRecalculo"].ToString());
                    intervaloRecalculo *= 1000;
                }

                return intervaloRecalculo;
            }
        }

        /// <summary>
        /// Retorna o Intervalo de calculo Fora do Pregão pronto (em milisegundos) para ser comparado se está em horário de 
        /// Leilão ou quando acabou o pregão
        /// </summary>
        private int IntervaloRecalculoForaPregao
        {
            get
            {
                int intervaloRecalculo = 300000;

                if (ConfigurationManager.AppSettings["IntervaloRecalculoForaPregao"] != null)
                {
                    intervaloRecalculo = Convert.ToInt32(ConfigurationManager.AppSettings["IntervaloRecalculoForaPregao"].ToString());
                    intervaloRecalculo *= 1000;
                }

                return intervaloRecalculo;
            }
        }

        PersistPositionClient _pPosClient;
        bool _isLoaded = false;
        bool _isRunningDB = false;
        bool _isExecutingMDS = false;
        int _codCarteiraGeral;
        /// <summary>
        /// Singleton da classe usado para controle de lock
        /// </summary>
        private object _Singleton = new object();

        // Mds Objects
        ConcurrentDictionary<string, TOSymbolInfo> _cdMdsSymbol;
        //ConcurrentQueue<TOSymbolInfo> _cqMdsSymbol;
        Thread _thMds;
        object _syncMds = new object();
        bool _isRunning = false;


        // SpiderOrderInfo History
        ConcurrentDictionary<int, SpiderOrderInfo> _cdSpiderOrderInfo;

        public delegate void PositionClientUpdateHandler(object sender, PositionClientEventArgs args);            
        public event PositionClientUpdateHandler OnPositionClientUpdate;
        
        private static PositionClientManager _me = null;
        public static PositionClientManager Instance
        {
            get
            {
                if (_me == null)
                {
                    _me = new PositionClientManager();
                }

                return _me;
            }
        }

        /// <summary>
        /// Construtor da classe de position Client Manager para carregamento do 
        /// instanicia da classe de persistencia de position client
        /// </summary>
        public PositionClientManager()
        {
            _pPosClient = new PersistPositionClient("PositionClientRMS", true);
            if (ConfigurationManager.AppSettings.AllKeys.Contains("CodCarteiraGeral"))
                _codCarteiraGeral = Convert.ToInt32(ConfigurationManager.AppSettings["CodCarteiraGeral"].ToString());
            else
                _codCarteiraGeral = 0;
        }

        public void Start()
        {
            try
            {
                logger.Info("Iniciando Position Client Manager...");
                this.LoadPositionClientData();

                _isRunning = true;
                logger.Info("Iniciando thread MDS...");
                _cdMdsSymbol = new ConcurrentDictionary<string,TOSymbolInfo>();
                _thMds = new Thread(new ThreadStart(_dequeueSymbol));
                _thMds.Start();

                thThreadClientes = new Thread(new ThreadStart(ThreadClientes));
                thThreadClientes.Name = "ThreadClientes";
                thThreadClientes.Start();
            }
            catch (Exception ex)
            {
                logger.Error("Problemas no start do PositionClient Manager...: " + ex.Message, ex);
                throw;
            }
        }

        public void Stop()
        {
            try
            {
                _isRunning = false;
                logger.Info("Parando Position client manager...");
                this.UnloadPositionClientData();

                logger.Info("Parando MDS thread...");
                if (_thMds != null && _thMds.IsAlive)
                {
                    _thMds.Join(300);
                    try
                    {
                        if (_thMds.IsAlive)
                            _thMds.Abort();
                    }
                    catch { }
                    _thMds = null;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Problemas no stop do PositionClient Manager...: " + ex.Message, ex);
                throw;
            }
        }

        public void LoadPositionClientData()
        {
            try
            {
                if (_dicPositionClient == null)
                {
                    _dicPositionClient = new ConcurrentDictionary<int, ConcurrentDictionary<string, PosClientSymbolInfo>>();
                }

                if (_dicPosClientBySymbol == null)
                {
                    _dicPosClientBySymbol = new ConcurrentDictionary<string, List<PosClientSymbolInfo>>();
                }

                //_dicExecSymbols = new ConcurrentDictionary<int, List<ExecSymbolInfo>>();
                _dicExecSymbols = new ConcurrentDictionary<int, ConcurrentDictionary<int,ExecSymbolInfo>>();
                
                _cdSpiderOrderInfo = new ConcurrentDictionary<int, SpiderOrderInfo>();

                //_dicSymbols = new ConcurrentDictionary<string, SymbolInfo>();

                if ( _db == null)
                    _db = new DbRisco();
                
                if (_DbOracle == null)
                    _DbOracle = new DbRiscoOracle();

                logger.Info("Carregando Posicao de Clientes - Abertura - Pode demorar um pouco...");

                if (null == _cqPosClient) _cqPosClient = new ConcurrentQueue<TOPosClient>();

                if (false == _loadPositionClient())
                {
                    throw new Exception("Problemas no carregamento de posicao de clientes - abertura");
                }

                logger.Info("Carregando Posicao de Clientes por simbolo...");
                
                _isLoaded = true;
                //if (null == _cqPosClient) _cqPosClient = new ConcurrentQueue<TOPosClient>();

                _thPosClient                = new Thread(new ThreadStart(_processPosClient));
                _thPosClient.Start();

                _thPosDb                    = new Thread(new ThreadStart(_processPosClientDB));
                _thPosDb.Start();

                //_thPosAbertura = new Thread(new ThreadStart(_processPosCliAbertura));
                //_thPosAbertura.Start();

                _thPosContaCorrenteAbertura = new Thread(new ThreadStart(_processPosCCAbertura));
                _thPosContaCorrenteAbertura.Start();

                //_processPosCliAbertura();
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na carga de informações de Position Client: " + ex.Message, ex);
            }
        }

        public void UnloadPositionClientData()
        {
            try
            {

                _isLoaded = false;
                if (null != _dicPositionClient)     _dicPositionClient.Clear();     _dicPositionClient = null;
                if (null != _dicExecSymbols)        _dicExecSymbols.Clear();        _dicExecSymbols = null;
                if (null != _dicPosClientBySymbol)  _dicPosClientBySymbol.Clear();  _dicPosClientBySymbol = null;
                if (null != _dicSaldoCc)            _dicSaldoCc.Clear();            _dicSaldoCc = null;
                if (null !=_cdSpiderOrderInfo)      _cdSpiderOrderInfo.Clear();     _cdSpiderOrderInfo = null;
                
                
                if (_thPosClient != null && _thPosClient.IsAlive)
                {
                    _thPosClient.Join(500);
                    if (_thPosClient.IsAlive)
                    {
                        try
                        {
                            _thPosClient.Abort();
                            _thPosClient = null;
                        }
                        catch { }
                    }
                }

                if (_thPosDb != null && _thPosDb.IsAlive)
                {
                    _thPosDb.Join(500);
                    if (_thPosDb.IsAlive) //if (_thPosClient.IsAlive)
                    {
                        try
                        {
                            _thPosDb.Abort();
                            _thPosDb = null;
                        }
                        catch { }
                    }
                }

                if (_thPosAbertura != null && _thPosAbertura.IsAlive)
                {
                    _thPosAbertura.Join(500);

                    if (_thPosAbertura.IsAlive)
                    {
                        try
                        {
                            _thPosAbertura.Abort();
                            _thPosAbertura = null;
                        }
                        catch { }
                    }
                }

                if (_thPosContaCorrenteAbertura != null && _thPosContaCorrenteAbertura.IsAlive)
                {
                    _thPosContaCorrenteAbertura.Join(500);

                    if (_thPosContaCorrenteAbertura.IsAlive)
                    {
                        try
                        {
                            _thPosContaCorrenteAbertura.Abort();
                            _thPosContaCorrenteAbertura = null;
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Problemas no unload das informacoes de position client: " + ex.Message, ex);
            }
        }

        public ConcurrentDictionary<int, List<PosClientSymbolInfo>> SnapshotPositionClient()
        {
            ConcurrentDictionary<int, List<PosClientSymbolInfo>> ret = new ConcurrentDictionary<int, List<PosClientSymbolInfo>>();
            try
            {
                lock (_sync)
                {
                    if (null != _dicPositionClient)
                    {
                        
                        foreach (KeyValuePair<int, ConcurrentDictionary<string, PosClientSymbolInfo>> item in _dicPositionClient)
                        {
                            List<PosClientSymbolInfo> lst = new List<PosClientSymbolInfo>();
                            foreach (KeyValuePair<string, PosClientSymbolInfo> item2 in item.Value)
                            {
                                lst.Add(item2.Value);
                            }
                            ret.AddOrUpdate(item.Key, lst, (key, oldValue) => lst);
                            lst = null;
                        }

                        ////KeyValuePair<int, List<PosClientSymbolInfo>>[] items = _dicPositionClient.ToArray();
                        //foreach (KeyValuePair<int, List<PosClientSymbolInfo>> item in items)
                        //{
                        //    ret.AddOrUpdate(item.Key, item.Value, (key, oldValue) => item.Value);
                        //}
                    }
                    else
                    {
                        logger.Info("Restriction Global sem registros...");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na montagem do snapshot Restriction Global: " + ex.Message, ex);
            }
            return ret;
        }
        
        private bool _loadPositionClient()
        {
            try
            {
                var dbOracle = new DbRiscoOracle();

                var lDicPositionClient = dbOracle.CarregarPosicaoAbertura(RiskCache.Instance.GetAllSymbols());

                foreach (var pos in lDicPositionClient)
                {
                    foreach (var position in pos.Value)
                    {
                        _processPosClientBySymbol(position.Value);
                    }
                }
                /*
                var lClientes = ConsolidatedRiskManager.Instance.ListaClientes;

                foreach (var pos in lDicPositionClient)
                {
                    foreach (var position in pos.Value)
                    {
                        if (position.Value.Bolsa.ToLower() == "bmf")
                        {
                            var lCliente = lClientes.Find(cliente => cliente.CodigoBmf == pos.Key || cliente.CodigoBovespa == pos.Key);

                            if (lCliente != null)
                            {
                                position.Value.NomeCliente = lCliente.NomeCliente;
                            }

                            BmfCalculator.Instance.CalcularPosicaoBmf(position.Value);
                        }
                        else
                        {
                            _processPosClientBySymbol(position.Value);

                            CalculatePositionClientAberturaByMds(position.Value);
                        }
                        
                        ConsolidatedRiskManager.Instance.CalculateConsolidatedPosition(position.Value);
                    }
                }
                */
                _dicPositionClient = lDicPositionClient;
                
                if (null != _dicPositionClient)
                {
                    logger.Info("_loadPositionClient() - No. Registros: " + _dicPositionClient.Count);
                }
                else
                {
                    logger.Info("_loadPositionClient() - Nao foram encontrado registros");
                }

                logger.Info("Efetuando a carga no banco de dados SQL Server...");
                
                logger.Info("Excluindo os registros do dia corrente...");
                
                _db.LimparPositionClient();
                
                foreach (KeyValuePair<int, ConcurrentDictionary<string, PosClientSymbolInfo>> item in _dicPositionClient)
                {
                    foreach (KeyValuePair<string, PosClientSymbolInfo> item2 in item.Value)
                    {
                         _db.InserirPositionClientMvto(item2.Value);
                        _pPosClient.TraceInfo(item2.Value);
                    }
                    //for (int i = 0; i < lst.Count; i++)
                    //{
                    //    //_db.AtualizarPositionClient(lst[i]);
                    //    _db.InserirPositionClientMvto(item.Value);
                    //}
                }

                return true;
            }
            catch (Exception ex)
            {
                logger.Error("Erro na carga de load position client: " + ex.Message, ex);
                return false;
            }
        }

        private void _addPosClient(TOPosClient to)
        {
            _cqPosClient.Enqueue(to);
            lock (_syncPosClient)
                Monitor.Pulse(_syncPosClient);
        }

        private void _processPosClient()
        {
            while (_isLoaded)
            {
                try
                {
                    TOPosClient to = null;
                    if (_cqPosClient.TryDequeue(out to))
                    {
                        //_db.AtualizarPositionClient(to.PositionClient);
                        //_db.InserirPositionClientMvto(to.PositionClient);
                        _pPosClient.TraceInfo(to.PositionClient);
                    }
                    else
                    {
                        lock (_syncPosClient)
                            Monitor.Wait(_syncPosClient, 100);
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Método de processo de thread para abertura de conta corrente para ficar rodando em um horário especifico
        /// </summary>
        private void _processPosCCAbertura()
        {
            var dbOracle = new DbRiscoOracle();

            if (!ConfigurationManager.AppSettings.AllKeys.Contains("HoraLoadCCAbertura"))
            {
                return;
            }

            while (_isLoaded)
            {
                string lHorarios = ConfigurationManager.AppSettings["HoraLoadCCAbertura"].ToString();

                List<string> lListHorarios = ListScheduleAbertura(lHorarios);

                DateTime lNow = DateTime.Now;

                if (lListHorarios.Contains(lNow.ToString("HH:mm")))
                {
                    logger.Info("Entrou no processo de Carregamento de Conta corrente para atualização.");

                    try
                    {
                        ConsolidatedRiskManager.Instance.CarregarContaCorrente();
                        ConsolidatedRiskManager.Instance.CarregarPosicaoBTCTomador();
                        ConsolidatedRiskManager.Instance.CarregarGarantias();
                    }
                    catch (Exception ex)
                    {
                        logger.Error("_processPosCCAbertura() - ", ex);
                    }
                }

                lock (_Singleton)
                {
                    System.Threading.Monitor.Wait(_Singleton, 50);
                }
            }
        }

        /// <summary>
        /// Lista Agendamentos (horários para a abertura)
        /// </summary>
        /// <param name="pHorarios">String com os Horarios Configurados no arquivo de configuração</param>
        /// <returns>Retorna uma lista de horários pré configurados no arquivo de configuração.</returns>
        public List<string> ListScheduleAbertura(string pHorarios)
        {
            List<string> lretorno = new List<string>();

            try
            {
                if (pHorarios.Contains(";"))
                {
                    char[] lchars = { ';' };
                    string[] lhr = pHorarios.Split(lchars, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string hr in lhr) lretorno.Add(hr);
                }
                else if (!string.IsNullOrEmpty(pHorarios))
                {
                    lretorno.Add(pHorarios);
                }
            }
            catch (Exception ex)
            {
                logger.Error("ListScheduleAbertura() - ", ex);
            }

            return lretorno;
        }

        private void _processPosClientDB()
        {
            int i = 0;
            int mdsRefresh = 10;
            int sleep = 10;
            if (ConfigurationManager.AppSettings.AllKeys.Contains("PositionClientDBRefresh"))
                mdsRefresh = Convert.ToInt32(ConfigurationManager.AppSettings["PositionClientDBRefresh"].ToString());
            while (_isLoaded)
            {
                try
                {
                    if (i >= mdsRefresh * sleep)
                    {
                        if (_isRunningDB)
                            break;
                        _isRunningDB = true;
                        this._atualizaPosClientDB();
                        i = 0;
                        _isRunningDB = false;
                    }
                    Thread.Sleep(sleep * 10);
                    i++;
                }
                catch
                {
                    i = 0;
                    _isRunningDB = false;
                }
            }
        }

        private void _atualizaPosClientDB()
        {
            try
            {
                List<PosClientSymbolInfo> lstAux = new List<PosClientSymbolInfo>();

                if (_db == null)
                    _db = new DbRisco();
                
                foreach (KeyValuePair<int, ConcurrentDictionary<string, PosClientSymbolInfo>> item in _dicPositionClient)
                {
                    lstAux.AddRange(item.Value.Values.ToList());
                }
                
                logger.Info("======> Atualizando registros PositionClient");
                int reg = 0;
                foreach (PosClientSymbolInfo item in lstAux)
                {
                    if (!_db.AtualizarPositionClient(item))
                        logger.ErrorFormat("Problemas na atualizacao/ insercao da PositionClient : [{0}] [{1}]", item.Account, item.Ativo);
                    reg++;
                }
                logger.Info("======> Registros atualizados: " + lstAux.Count);
                lstAux.Clear();
                lstAux = null;
                
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na atualizacao das informacoes de cotacao: " + ex.Message, ex);
            }
        }

        private void _processPosClientBySymbol(PosClientSymbolInfo pos)
        {
            try
            {
                List<PosClientSymbolInfo> lst = null;

                if (_dicPosClientBySymbol == null)
                    _dicPosClientBySymbol = new ConcurrentDictionary<string, List<PosClientSymbolInfo>>();

                var lClientes = ConsolidatedRiskManager.Instance.ListaClientes;

                var lCliente = lClientes.Find(cliente=> cliente.CodigoBovespa == pos.Account || cliente.CodigoBmf == pos.Account );

                if (lCliente != null)
                {
                    pos.NomeCliente = lCliente.NomeCliente;
                }

                if (_dicPosClientBySymbol.TryGetValue(pos.Ativo, out lst))
                {
                    PosClientSymbolInfo item = lst.FirstOrDefault(x => x.Ativo.Equals(pos.Ativo) && x.Account.Equals(pos.Account));

                    if (item == null)
                        lst.Add(pos);
                    else
                        item = pos;
                }
                else
                {
                    lst = new List<PosClientSymbolInfo>();
                    lst.Add(pos);
                    _dicPosClientBySymbol.AddOrUpdate(pos.Ativo, lst, (key, oldValue) => lst);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Problemas no processamento de position client por simbolo: " + ex.Message, ex);
            }
        }

        private decimal _processAvgPrice(SpiderOrderInfo order, PosClientSymbolInfo ps)
        {
            decimal ret = decimal.Zero;
            if (order.Side.Equals(1))
                ret = ps.PcMedC;
            else
                ret = ps.PcMedV;

            // Se ordem nao for de execucao, retorna o mesmo valor já calculado
            //if (order.OrdStatus != 1 && order.OrdStatus != 2)
            //    return ret;

            if (order.CumQty == 0)
                return ret;
            try
            {
                // SpiderOrderDetailInfo det = this._getLastTradeQtyDet(order.Details);
                List<SpiderOrderDetailInfo> lstDet = order.Details.Select(x => x).Where(x => x.OrderStatusID.Equals(1) || x.OrderStatusID.Equals(2)).ToList();
                //int lastTradeQty = 0;
                //if (det != null)
                //    lastTradeQty = det.TradeQty;

                // Gravar informacoes da execucao e calcular o preco medio
                ConcurrentDictionary<int, ExecSymbolInfo> cdExec = null;
                if (!_dicExecSymbols.TryGetValue(order.Account, out cdExec))
                {
                    cdExec = new ConcurrentDictionary<int, ExecSymbolInfo>();
                    _dicExecSymbols.AddOrUpdate(order.Account, cdExec, (key, oldvalue) => cdExec);
                }

                foreach (SpiderOrderDetailInfo item in lstDet)
                {
                    ExecSymbolInfo aa = null;
                    if (!cdExec.TryGetValue(item.OrderDetailID, out aa))
                    {
                        aa = new ExecSymbolInfo();
                        aa.Id = item.OrderDetailID;
                        aa.Price = item.Price;
                        aa.Qty = item.TradeQty;
                        aa.Side = order.Side;
                        aa.Symbol = order.Symbol;
                        aa.Account = order.Account;
                        cdExec.AddOrUpdate(aa.Id, aa, (key, oldvalue) => aa);
                    }
                }


                // Calculo de preco medio
                List<ExecSymbolInfo> lstAvg = cdExec.Values.Select(x =>x).Where(x => x.Side == order.Side && x.Symbol == order.Symbol).ToList();
                ret = Tools.CalculateWeightedAvgPx(lstAvg);
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas no calculo do preco medio: " + ex.Message, ex);
                return ret;
            }
        }
        
        public void CalculatePositionClient(TOSpOrder toOrd)
        {
            try
            {
                SpiderOrderInfo ord = toOrd.Order;
                
                SpiderOrderInfo ordAnt = null;
                
                int ordStatus = toOrd.Order.OrdStatus;

                if (ordStatus != 0 && ordStatus != 1 && ordStatus != 2 && ordStatus != 4 && ordStatus != 5)
                    return;

                // Buscar informacoes de ordem anterior
                if (!_cdSpiderOrderInfo.TryGetValue(ord.OrderID, out ordAnt))
                {
                    _cdSpiderOrderInfo.AddOrUpdate(ord.OrderID, ord, (key, oldvalue) => ord);
                
                    ordAnt = new SpiderOrderInfo();
                    
                    ordAnt.OrderQtyRemaining = 0;
                    
                    ordAnt.CumQty = 0;
                }

                if (ordAnt.OrderQty == ord.OrderQty && ordAnt.OrderQtyRemaining == ord.OrderQtyRemaining && ordAnt.CumQty == ord.CumQty)
                    return;

                if (ord.RegisterTime.Date != DateTime.Now.Date)
                    return;

                bool foundPC = false;
                
                PosClientSymbolInfo posCli = null;
                
                // Buscar o objeto position client e atualizar as quantidades abertas, executadas e net;
                ConcurrentDictionary<string, PosClientSymbolInfo> cdPosC = null;
                
                if (!_dicPositionClient.TryGetValue(ord.Account, out cdPosC))
                {
                    // Account nao existe, adicionando a collection de PositionClient
                    cdPosC = new ConcurrentDictionary<string, PosClientSymbolInfo>();

                    _dicPositionClient.AddOrUpdate(ord.Account, cdPosC, (key, oldValue) => cdPosC);
                }
                else
                {
                    // Account existe, então procurando o ativo para ver se existe a position client
                    foundPC = cdPosC.TryGetValue(ord.Symbol, out posCli);
                }

                // Se nao achou, entao criara e adicionara no dicionario
                if (!foundPC)
                {
                    posCli = new PosClientSymbolInfo();
                    
                    posCli.Account = ord.Account;
                    
                    posCli.Ativo = ord.Symbol;
                    
                    posCli.Bolsa = ord.Exchange;
                    
                    if (ord.Account.Equals(0))
                        posCli.ExecBroker = ord.ExecBroker;
                    else
                        posCli.ExecBroker = ord.Account.ToString();
                    
                    posCli.DtPosicao = DateTime.Now.Date;
                    
                    // Atribuicao das propriedades comuns
                    SymbolInfo symbInfo = RiskCache.Instance.GetSymbol(ord.Symbol);
                    
                    if (symbInfo == null)
                        symbInfo = new SymbolInfo(); // Criar zerado somente para atribuicao
                    
                    posCli.TipoMercado = ParserTipoMercado.Segmento2TipoMercado(symbInfo.SegmentoMercadoValor, symbInfo.IndicadorOpcao);
                    
                    posCli.DtVencimento = symbInfo.DtVencimento;
                    
                    posCli.CodPapelObjeto = symbInfo.CodigoPapelObjeto;
                    
                    posCli.PrecoFechamento = symbInfo.VlrFechamento;
                }

                var lClientes = ConsolidatedRiskManager.Instance.ListaClientes;


                var lCliente = lClientes.Find(cliente => cliente.CodigoBovespa == posCli.Account || cliente.CodigoBmf == posCli.Account);

                if (lCliente != null)
                {
                    posCli.NomeCliente = lCliente.NomeCliente;
                }

                // Atualizacoes das collections
                cdPosC.AddOrUpdate(posCli.Ativo, posCli, (key, oldValue) => posCli);
                
                // Atribuicao das propriedades comuns
                SymbolInfo symbInfo1 = RiskCache.Instance.GetSymbol(ord.Symbol);

                if (symbInfo1 == null)
                {
                    symbInfo1 = new SymbolInfo(); // Criar zerado somente para atribuicao
                }
                //lock (posCli)
                //{
                posCli.DtMovimento = DateTime.Now;
                posCli.Variacao = symbInfo1.VlrOscilacao;
                posCli.UltPreco = symbInfo1.VlrUltima;

                if (ord.Side.Equals(1))
                {
                    // Compra
                    posCli.QtdAbC   = posCli.QtdAbC - ordAnt.OrderQtyRemaining + ord.OrderQtyRemaining;
                    posCli.QtdExecC = posCli.QtdExecC - ordAnt.CumQty + ord.CumQty;
                    posCli.PcMedC   = this._processAvgPrice(ord, posCli);

                    if (ord.Exchange.Equals(Exchange.Bovespa, StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (symbInfo1.LotePadrao > 100)
                        {
                            posCli.VolCompra = (posCli.PcMedC * posCli.QtdExecC ) / symbInfo1.LotePadrao;
                        }
                        else
                        {
                            posCli.VolCompra = posCli.PcMedC * posCli.QtdExecC;
                        }
                    }
                    else
                    {
                        posCli.VolCompra = posCli.VolCompra - ordAnt.CumQty + ord.CumQty;
                    }
                }
                else
                {
                    // Venda
                    posCli.QtdAbV   = posCli.QtdAbV - ordAnt.OrderQtyRemaining + ord.OrderQtyRemaining;
                    posCli.QtdExecV = posCli.QtdExecV - ordAnt.CumQty + ord.CumQty;
                    posCli.PcMedV   = this._processAvgPrice(ord, posCli);

                    if (ord.Exchange.Equals(Exchange.Bovespa, StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (symbInfo1.LotePadrao > 100)
                        {
                            posCli.VolVenda = (posCli.PcMedV * posCli.QtdExecV ) / symbInfo1.LotePadrao;
                        }
                        else
                        {
                            posCli.VolVenda = posCli.PcMedV * posCli.QtdExecV;
                        }
                    }
                    else
                    {
                        posCli.VolVenda = posCli.VolVenda - ordAnt.CumQty + ord.CumQty;
                    }
                }
                _cdSpiderOrderInfo.AddOrUpdate(ord.OrderID, ord, (key, oldValue) => ord);

                // Calcular todos os net's e afins
                posCli.NetAb            = posCli.QtdAbC - posCli.QtdAbV;
                posCli.NetExec          = posCli.QtdExecC - posCli.QtdExecV;
                posCli.QtdDisponivel    = posCli.QtdAbertura + posCli.QtdD1 + posCli.QtdD2;
                posCli.QtdTotal         = posCli.QtdDisponivel + posCli.NetExec;
                posCli.VolTotal         = posCli.VolCompra + posCli.VolVenda;
                

                // Financeiro Net / Lucro Prej
                if (ord.Exchange.Equals(Exchange.Bovespa, StringComparison.CurrentCultureIgnoreCase))
                {
                    decimal lAbertura = decimal.Zero;

                    if (posCli.QtdAbertura != 0)
                    {
                        if (symbInfo1.LotePadrao > 100)
                        {
                            lAbertura = (posCli.QtdAbertura * posCli.UltPreco) / symbInfo1.LotePadrao;
                        }
                        else
                        {
                            lAbertura = (posCli.QtdAbertura * posCli.UltPreco);
                        }
                    }

                    if (symbInfo1.LotePadrao > 100)
                    {
                        posCli.FinancNet = lAbertura + ((posCli.NetExec * posCli.UltPreco)/symbInfo1.LotePadrao);
                    }
                    else
                    {
                        posCli.FinancNet = lAbertura + (posCli.NetExec * posCli.UltPreco);
                    }
                    
                    decimal buy = 0, sell = 0;

                    if (symbInfo1.LotePadrao > 100)
                    {
                        buy = ((posCli.UltPreco - posCli.PcMedC) * posCli.QtdExecC ) / symbInfo1.LotePadrao;

                        sell = ((posCli.PcMedV - posCli.UltPreco) * posCli.QtdExecV )/ symbInfo1.LotePadrao;
                    }
                    else
                    {
                        buy = ((posCli.UltPreco - posCli.PcMedC) * posCli.QtdExecC);

                        sell = ((posCli.PcMedV - posCli.UltPreco) * posCli.QtdExecV);
                    }
   
                    posCli.LucroPrej =  buy + sell;
                        
                    posCli.LucroPrejuizoSemAbertura = buy + sell;
                }
                else
                {
                    // Calculo BMF: de acordo com o tipo de mercado
                    if (!BmfCalculator.Instance.CalcularPosicaoBmf(posCli))
                    {
                        logger.Error("Problemas no calculo de Finaceiro Net e L/P de BMF");
                    }
                }

                posCli.MsgId++;
                posCli.EventSource = "POSCLI";

                //if (ord.OrdStatus == 1 || ord.OrdStatus == 2 || (ord.OrdStatus == 5 && ord.CumQty > 0) || (ord.OrdStatus == 4 && ord.CumQty > 0))
                //{
                var ordProcess = from a in ord.Details where (a.OrderStatusID == 1 || a.OrderStatusID == 2) select a;

                if (ordProcess.Count() != 0)
                {
                    if (!ord.IsSnapshot)
                    {
                        this.AddTradeDetail(posCli, ord, ordAnt, symbInfo1);
                    }
                    else
                    {
                        this.AddTradeDetailSnapshot(posCli, ord, ordAnt, symbInfo1);
                    }
                }
                //}

                _dicPositionClient.AddOrUpdate(ord.Account, cdPosC, (key, oldValue) => cdPosC);
                
                // Processar PositionClient By Symbol (montagem da estrutura para facilidade no calculo de L/P e Financeiro NET)
                this._processPosClientBySymbol(posCli);
                
                // Efetuar chamada para calculo de risco consolidado
                ConsolidatedRiskManager.Instance.CalculateConsolidatedPosition(posCli);
                //}

                if (logger.IsDebugEnabled)
                {
                    logger.DebugFormat("Ord:[{0}] MsgSeqNum[{1}] Account[{2}] Ativo[{3}] Status:[{4}] Qty[{5}] QtyRemaining[{6}] CumQty[{7}] DetailCount[{8}] TransactTime[{9}]",
                        ord.OrderID, ord.MsgSeqNum, ord.Account, ord.Symbol, ord.OrdStatus, ord.OrderQty, ord.OrderQtyRemaining, ord.CumQty, ord.Details.Count, ord.TransactTime.ToString("HH:mm"));
                }
                // Enfileirar para gravar as informacoes na tabela sqlserver
                TOPosClient to = new TOPosClient();
                to.PositionClient = posCli;
                this._addPosClient(to);

                // Ativar o evento para sincronizar memoria client
                if (OnPositionClientUpdate != null)
                {
                    PositionClientEventArgs sync = new PositionClientEventArgs();
                    sync.Action = EventAction.UPDATE;
                    sync.Account = ord.Account;
                    PosClientSymbolInfo aux = Cloner.ClonePosClient(posCli);
                    List<PosClientSymbolInfo> lstAux = new List<PosClientSymbolInfo>();
                    lstAux.Add(aux);
                    sync.PosClient = lstAux;
                    OnPositionClientUpdate(this, sync);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Problemas no calculo de position client: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Adiciona a Negociação no objeto de Posição do Cliente
        /// </summary>
        /// <param name="pInfo">Parametro de Position Client Symbol  </param>
        /// <param name="order">Parametro de Order Info</param>
        private void AddTradeDetailSnapshot(PosClientSymbolInfo pInfo, SpiderOrderInfo pOrder, SpiderOrderInfo POrderAnt, SymbolInfo pSymb )
        {
            try
            {
                var lOrderDetails = from a in pOrder.Details where (a.OrderStatusID == 1 || a.OrderStatusID == 2) select a;

                lOrderDetails.ToList().ForEach(trade => 
                {
                    var lTrade = new PosClientSymbolDetailInfo();

                    //Calculando a quantidade executada corretamente
                    int QuantExec = trade.TradeQty;

                    lTrade.CodigoCliente = pOrder.Account;
                    lTrade.Instrumento  = pOrder.Symbol;
                    lTrade.PrecoCliente = trade.Price;// pOrder.Price;
                    lTrade.Bolsa        = pOrder.Exchange.Equals(Exchange.Bovespa, StringComparison.CurrentCultureIgnoreCase) ? Exchange.Bovespa : Exchange.Bmf;
                    lTrade.Sentido      = pOrder.Side.Equals(1) ? "compra" : "venda";
                    lTrade.ClOrdID      = pOrder.ClOrdID;
                    lTrade.Horario      = trade.TransactTime.ToString("HH:mm");
                    lTrade.Qtde         = QuantExec;
                    lTrade.PrecoMercado = pInfo.UltPreco;

                    if (lTrade.Bolsa.Equals(Exchange.Bovespa))
                    {
                        decimal lTotalLP = lTrade.Sentido.Equals("compra") ? ((pInfo.UltPreco - trade.Price ) *  lTrade.Qtde) : ((trade.Price - pInfo.UltPreco) * lTrade.Qtde);

                        lTrade.LP = lTotalLP;
                        lTrade.TotalCliente = (trade.Price * lTrade.Qtde);
                        lTrade.TotalMercado = (pInfo.UltPreco * lTrade.Qtde);
                    }
                    else
                    {
                        lTrade.TotalCliente = BmfCalculator.Instance.CalcularLucroPrejuizoBmfTotais(pSymb, lTrade.Qtde, trade.Price);
                        lTrade.TotalMercado = BmfCalculator.Instance.CalcularLucroPrejuizoBmfTotais(pSymb, lTrade.Qtde, pInfo.UltPreco);

                        decimal lDiferencial = lTrade.Sentido.Equals("compra") ? (lTrade.PrecoMercado - lTrade.PrecoCliente) : (lTrade.PrecoCliente - lTrade.PrecoMercado);

                        decimal lLPTotalBmf = BmfCalculator.Instance.CalcularLPSubtotal(pSymb, lTrade.Qtde, lDiferencial, lTrade.PrecoCliente);

                        lTrade.LP = lTrade.Sentido.Equals("compra") ? lLPTotalBmf : lLPTotalBmf * (-1);


                        //lTrade.LP = BmfCalculator.Instance.CalcularLPSubtotal(pSymb, lTrade.Qtde, lDiferencial, lTrade.PrecoCliente);
                    }

                    pInfo.TradeDetails.Add(lTrade);
                    
                });
            }
            catch (Exception ex)
            {
                logger.Error("Problema encontrado no método AddTradeDetail", ex);
            }
        }

        /// <summary>
        /// Adiciona a Negociação no objeto de Posição do Cliente
        /// </summary>
        /// <param name="pInfo">Parametro de Position Client Symbol  </param>
        /// <param name="order">Parametro de Order Info</param>
        private void AddTradeDetail(PosClientSymbolInfo pInfo, SpiderOrderInfo pOrder, SpiderOrderInfo POrderAnt, SymbolInfo pSymb )
        {
            try
            {
                //if (pOrder.CumQty != 0 && (pOrder.OrdStatus.Equals(1) || pOrder.OrdStatus.Equals(2)))
                if (pOrder.CumQty != 0)
                {
                    var lTrade = new PosClientSymbolDetailInfo();

                    //if (ordAnt.OrderQty == ord.OrderQty && ordAnt.OrderQtyRemaining == ord.OrderQtyRemaining && ordAnt.CumQty == ord.CumQty)
                        //return;
                    
                    //Calculando a quantidade executada corretamente
                    int QuantExec           = (POrderAnt.OrderQtyRemaining - pOrder.OrderQtyRemaining);

                    lTrade.CodigoCliente    = pOrder.Account;
                    lTrade.Instrumento      = pOrder.Symbol;
                    lTrade.PrecoCliente     = pOrder.Price;
                    lTrade.Bolsa            = pOrder.Exchange.Equals(Exchange.Bovespa, StringComparison.CurrentCultureIgnoreCase) ? Exchange.Bovespa : Exchange.Bmf;
                    lTrade.Sentido          = pOrder.Side.Equals(1) ? "compra" : "venda";
                    lTrade.ClOrdID          = pOrder.ClOrdID;
                    lTrade.Horario          = pOrder.TransactTime.ToString("HH:mm");
                    lTrade.Qtde             = QuantExec;
                    lTrade.PrecoMercado     = pSymb.VlrUltima;//pInfo.UltPreco;

                    

                    if (lTrade.Bolsa.Equals(Exchange.Bovespa))
                    {
                        decimal lTotalLP = lTrade.Sentido.Equals("compra") ? ((pSymb.VlrUltima - pOrder.Price) * lTrade.Qtde) : ((pOrder.Price - pSymb.VlrUltima) * lTrade.Qtde);

                        lTrade.LP           = lTotalLP;
                        lTrade.TotalCliente = (pOrder.Price * lTrade.Qtde);
                        lTrade.TotalMercado = (pInfo.UltPreco * lTrade.Qtde);
                    }
                    else
                    {
                        lTrade.TotalCliente = BmfCalculator.Instance.CalcularLucroPrejuizoBmfTotais(pSymb, lTrade.Qtde, pOrder.Price);
                        lTrade.TotalMercado = BmfCalculator.Instance.CalcularLucroPrejuizoBmfTotais(pSymb, lTrade.Qtde, pSymb.VlrUltima);

                        decimal lDiferencial = lTrade.Sentido.Equals("compra") ? (lTrade.PrecoMercado - lTrade.PrecoCliente) : (lTrade.PrecoCliente- lTrade.PrecoMercado);

                        decimal lLPTotalBmf = BmfCalculator.Instance.CalcularLPSubtotal(pSymb, lTrade.Qtde, lDiferencial, lTrade.PrecoCliente);

                        lTrade.LP = lTrade.Sentido.Equals("compra") ? lLPTotalBmf : lLPTotalBmf * (-1);
                    }

                    //lock (pInfo)
                    //{
                        pInfo.TradeDetails.Add(lTrade);
                    //}
                }

            }
            catch (Exception ex)
            {
                logger.Error("Problema encontrado no método AddTradeDetail", ex);
            }
        }

        /// <summary>
        /// Atualiza os Preços da negociações encontradas na Posicção de Position Client
        /// </summary>
        /// <param name="pInfo">Parametro de  Position Client Symbol</param>
        /// <param name="pOrder">Parametro de Order info</param>
        private void UptTradeDetail(PosClientSymbolInfo pInfo, SymbolInfo pSymbol)
        {
            try
            {
                pInfo.TradeDetails.ForEach(trade =>
                {
                    if (trade.Instrumento.ToUpper().Equals(pSymbol.Instrumento.ToUpper()))
                    {
                        if (trade.Bolsa.Equals(Exchange.Bovespa))
                        {

                            trade.PrecoMercado = pSymbol.VlrUltima;

                            decimal lTotalLP = trade.Sentido.Equals("compra") ? ((pSymbol.VlrUltima - trade.PrecoCliente) * trade.Qtde) : ((trade.PrecoCliente - pSymbol.VlrUltima) * trade.Qtde);

                            trade.LP            = lTotalLP; 
                            trade.TotalCliente  = (trade.PrecoCliente * trade.Qtde);
                            trade.TotalMercado = (pSymbol.VlrUltima * trade.Qtde);
                        }
                        else
                        {
                            trade.PrecoMercado = pSymbol.VlrUltima;
                            trade.TotalCliente  = BmfCalculator.Instance.CalcularLucroPrejuizoBmfTotais(pSymbol, trade.Qtde, trade.PrecoCliente);
                            trade.TotalMercado = BmfCalculator.Instance.CalcularLucroPrejuizoBmfTotais(pSymbol, trade.Qtde, pSymbol.VlrUltima);

                            decimal lDiferencial = trade.Sentido.Equals("compra") ? (trade.PrecoMercado - trade.PrecoCliente) : (trade.PrecoCliente - trade.PrecoMercado);

                            decimal lLPTotalBmf = BmfCalculator.Instance.CalcularLPSubtotal(pSymbol, trade.Qtde, lDiferencial, trade.PrecoCliente);

                            trade.LP = trade.Sentido.Equals("compra") ? lLPTotalBmf : lLPTotalBmf * (-1);

                            //trade.LP = BmfCalculator.Instance.CalcularLPSubtotal(pSymbol, trade.Qtde, totalLP, trade.PrecoCliente);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                logger.Error("Problema encontrado no método UptTradeDetail", ex);
            }
        }

        /// <summary>
        /// Thread em que os clientes são relacionados do banco de dados 
        /// para entrarem na relação de clientes monitorados.
        /// Depois de adicionados, é chamada a função de RunTimer
        /// </summary>
        private void ThreadClientes()
        {
            DateTime lastRun = DateTime.MinValue;
            DateTime lastRun10Min = DateTime.MinValue;

            while (_isRunning)
            {
                TimeSpan interval = DateTime.Now - lastRun;

                try
                {
                    List<int> RelacaoClientesRodada = null;


                    if (interval.TotalMilliseconds > 120000) //-->> 2 em 2 minutos 
                    {
                        lastRun = DateTime.Now;

                        logger.Debug("Obtendo relacao de clientes que operaram nos ultimos 2 minutos");

                        RelacaoClientesRodada = new DbRiscoOracle().ObterClientesPosicaoDiaLimitada();

                        if (RelacaoClientesRodada.Count > 0)
                        {
                            logger.Info("Relacao de clientes encontrados.[" + RelacaoClientesRodada.Count.ToString() + "].");
                        }
                        else
                        {
                            logger.Info("Não existe clientes para serem calculados nesta tentativa.");
                        }

                        //logger.Info("Efetuando Thread da carga de posição de opções exercídas pelo operador zero");

                        //this.RunTimerTaskOpcoesExecidaOperadorZero();

                        foreach (int Cliente in RelacaoClientesRodada)
                        {
                            if (!ClientesMonitor.Contains(Cliente))
                            {
                                logger.Info("Acrescentando [" + Cliente + "] na lista de monitoracao");
                                
                                this.ClientesMonitor.Add(Cliente);
                                
                                //this.RunTimer(Cliente);

                                this.RunTimerClienteCC(Cliente);

                                Thread.Sleep(250);
                            }
                        }

                        

                    }
                }
                catch (Exception ex)
                {
                    logger.Info("Ocorreu um erro no método RunTimer ", ex);
                }

                Thread.Sleep(250);
            }
        }

        /// <summary>
        /// Método RunTimer é responsável por montar o Mutex do cliente e inseri-lo
        /// na lista de clientes monitorados
        /// </summary>
        /// <param name="CodigoCliente">Código principal do cliente</param>
        private void RunTimerClienteCC(int CodigoCliente)
        {
            try
            {
                System.Threading.TimerCallback TimerDelegate = new System.Threading.TimerCallback(TimerTaskClientCC);

                System.Threading.Timer TimerItem = new System.Threading.Timer(TimerDelegate, CodigoCliente, 0, IntervaloRecalculo);

                //if (htClientes.ContainsKey(CodigoCliente))
                //{
                //    htClientes.TryRemove(CodigoCliente, out StateObj);
                //}

                if (ClientesMonitor.Contains(CodigoCliente))
                {
                    ClientesMonitor.Remove(CodigoCliente);
                }
            }
            catch (Exception ex)
            {
                logger.Info("Ocorreu um erro no método RunTimerClienteCC :  ", ex);
            }
        }

        /// <summary>
        /// Método RunTimerTaskOpcoesExecidaOperadorZero é responsável por obter as opções exercídas 
        /// pelo operador 0 (zero) e inserilas no calculo de risco e 
        /// prejuízo do Position Client e Consolidated Risk para acompanhamento 
        /// </summary>
        private void RunTimerTaskOpcoesExecidaOperadorZero()
        {
            try
            {
                TimerCallback TimerDelegate = new TimerCallback(TimerTaskOpcoesExecidaOperadorZero);

                Timer TimerItem = new Timer(TimerDelegate, null, 0, IntervaloRecalculo);
            }
            catch (Exception ex)
            {
                logger.Info("Ocorreu um erro no método RunTimerTaskOpcoesExecidaOperadorZero : ", ex);
            }
        }

        /// <summary>
        /// Método responsável pela inserção no calculo de Lucro Prejuízo de Operadores Intraday e Consolidated Risk
        /// dos clientes obtidos do oracle com posição de opções exercidas pelo operador 0 (zero) 
        /// Isso é necessário para o 
        /// </summary>
        private void TimerTaskOpcoesExecidaOperadorZero(object pObject)
        {
            try
            {
                var lListaPosition = _DbOracle.ObtemPosicaoOpcaoExercidaOperadorZero();

                foreach (var position in lListaPosition)
                {
                    _processPosClientBySymbol(position);
                }
            }
            catch (Exception ex)
            {
                logger.Info("Ocorreu um erro no método TimerTaskOpcoesExecidaOperadorZero : ", ex);
            }
        }

        /// <summary>
        /// Método de verificação de monitoração do cliente 
        /// Verifica s eo cliente não está em processamento e verifica se o cliente 
        /// pode ser recalculado para atualizar a exposição do mesmo.
        /// </summary>
        /// <param name="StateObj">Objeto cliente tipo object que irá posteriormente ser convertido para ClienteMutexInfo </param>
        private void TimerTaskClientCC(object StateObj)
        {
            try
            {
                int Cliente = (int)StateObj;

                ConsolidatedRiskManager.Instance.CarregarContaCorrente(Cliente);

                //if (OnPositionClientUpdate != null)
                //{
                //    PositionClientEventArgs sync = new PositionClientEventArgs();

                //    sync.Action = EventAction.UPDATE;
                                        
                //    sync.Account = Cliente;
                    
                    
                //    OnPositionClientUpdate(this, sync);
                //}
            }
            catch (Exception ex)
            {
                logger.Info("Ocorreu um erro no método TimerTaskClientCC : ", ex);
            }
        }

        /// <summary>
        /// Método RunTimer é responsável por montar o Mutex do cliente e inseri-lo
        /// na lista de clientes monitorados
        /// </summary>
        /// <param name="CodigoCliente">Código principal do cliente</param>
        private void RunTimer(int CodigoCliente)
        {
            try
            {
                var StateObj                = new ClienteMutexInfo();

                StateObj.TimerCanceled      = false;
                StateObj.SomeValue          = 1;
                StateObj.IdCliente          = CodigoCliente;
                StateObj.StatusProcessando  = EnumProcessamento.LIVRE;
                StateObj.FirstTimeProcessed = DateTime.Now;
                StateObj.Intervalo          = IntervaloRecalculo;

                System.Threading.TimerCallback TimerDelegate = new System.Threading.TimerCallback(TimerTaskClient);

                logger.Debug("Inicia timer para cliente [" + CodigoCliente + "] com [" + IntervaloRecalculo + "] ms");

                System.Threading.Timer TimerItem = new System.Threading.Timer(TimerDelegate, StateObj, 1000, IntervaloRecalculo);

                StateObj.TimerReference = TimerItem;

                if (htClientes.ContainsKey(CodigoCliente))
                {
                    htClientes.TryRemove(CodigoCliente, out StateObj);
                }

                if (ClientesMonitor.Contains(CodigoCliente))
                {
                    ClientesMonitor.Remove(CodigoCliente);
                }

                htClientes.AddOrUpdate(CodigoCliente, StateObj, (key, oldValue) => StateObj);

                //StateObj.Dispose(); = null;
                //StateObj.Dispose();
            }
            catch (Exception ex)
            {
                logger.Info("Ocorreu um erro no método RunTimer ", ex);
            }
        }

        /// <summary>
        /// Método de verificação de monitoração do cliente 
        /// Verifica s eo cliente não está em processamento e verifica se o cliente 
        /// pode ser recalculado para atualizar a exposição do mesmo.
        /// </summary>
        /// <param name="StateObj">Objeto cliente tipo object que irá posteriormente ser convertido para ClienteMutexInfo </param>
        private void TimerTaskClient(object StateObj)
        {
            logger.Debug("Entrando no timertask [" + ((ClienteMutexInfo)StateObj).IdCliente + "]");

            try
            {
                ClienteMutexInfo State = (ClienteMutexInfo)StateObj;

                int Cliente = State.IdCliente;

                System.Threading.Interlocked.Increment(ref State.SomeValue);

                logger.Info("Thread disparada  " + DateTime.Now.ToString() + " Cliente: " + State.IdCliente.ToString());

                if (State.StatusProcessando == EnumProcessamento.LIVRE)
                {
                    State.StatusProcessando = EnumProcessamento.EMPROCESSAMENTO;

                    logger.Debug(" Cliente: " + State.IdCliente.ToString() + " - Aguardando Mutex");

                    State._Mutex.WaitOne();

                    //
                    logger.Info("Inicia calculo de Conta corrente " + DateTime.Now.ToString() + " Cliente: " + State.IdCliente.ToString());

                    ConsolidatedRiskManager.Instance.CarregarContaCorrente(Cliente);

                    logger.Info("Conta corrente Calculada  " + DateTime.Now.ToString() + " Cliente: " + State.IdCliente.ToString());

                    //this.AtualizaExposicaoCliente(info);

                    State._Mutex.ReleaseMutex();
                    State.StatusProcessando = EnumProcessamento.LIVRE;
                    State.Dispose();
                }
                else
                {
                    logger.Info("processando  " + DateTime.Now.ToString() + " Cliente: " + State.IdCliente.ToString());
                }

                if (State.TimerReference != null)
                {
                    State.TimerReference.Dispose();
                    State.TimerReference = null;
                }


                if (State.TimerCanceled)
                {
                    if (State.TimerReference != null)
                    {
                        State.TimerReference.Dispose();
                    }

                    logger.Info("Done  " + DateTime.Now.ToString());
                }
            }
            catch (Exception ex)
            {
                logger.Error("Ocorreu um erro ao processar método TimerTaskCliente {requisição de processamento da rotina Calcular conta corrente} " + ex.Message, ex);
            }
        }

        #region Auxiliary Functions
        
        private SpiderOrderDetailInfo _getLastTradeQtyDet(List<SpiderOrderDetailInfo> dets)
        {
            try
            {
                // Buscar o penultimo status do detail (new ou replaced): Vai retornar ao menos 2 registros. Sempre pegar o penultimo
                List<SpiderOrderDetailInfo> lstDet = dets.Select(x => x).Where(x => x.OrderStatusID == (int)SpiderOrderStatus.EXECUTADA ||
                    x.OrderStatusID == (int)SpiderOrderStatus.PARCIALMENTEEXECUTADA).OrderByDescending(x => x.OrderDetailID).ToList();
                SpiderOrderDetailInfo od;
                if (lstDet.Count > 0)
                {
                    od = lstDet[0];
                    return od;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na busca do TradeQty Det: " + ex.Message, ex);
                return null;
            }
        }
        #endregion

        #region MDSEvents
        public void EnqueueSymbol(TOSymbolInfo item)
        {
            // Efetuar esta validacao para que nao sobrescreva o DtUpdated
            TOSymbolInfo to = null;
            if (_cdMdsSymbol.TryGetValue(item.Instrumento.Instrumento, out to))
            {
                to.Instrumento = item.Instrumento;
            }
            else
                _cdMdsSymbol.AddOrUpdate(item.Instrumento.Instrumento, item, (key, oldvalue) =>item);
        }


        public void _dequeueSymbol()
        {
            int sleep = 50;
            int i = 0;
            int mdsRefresh = 300;
            int mdsConsolidatedRisk = 10000;
            if (ConfigurationManager.AppSettings.AllKeys.Contains("MdsRefresh"))
                mdsRefresh = Convert.ToInt32(ConfigurationManager.AppSettings["MdsRefresh"].ToString());
            if (ConfigurationManager.AppSettings.AllKeys.Contains("MdsConsolidatedRisk"))
                mdsConsolidatedRisk = Convert.ToInt32(ConfigurationManager.AppSettings["MdsConsolidatedRisk"].ToString());

            while(_isRunning)
            {
                try
                {
                    if (_isExecutingMDS)
                    {
                        Thread.Sleep(sleep);
                        break;
                    }
                    _isExecutingMDS = true;
                    
                    // Buscar os simbolos que tem difernenca maior que mdsRefresh entre DtUpdate(envio da posicao) e 
                    // DtMvto
                    List<TOSymbolInfo> aux = _cdMdsSymbol.Values.Select(x=>x).Where(x=>(DateTime.Now.Subtract(x.DtUpdated)).TotalMilliseconds >= mdsRefresh).ToList();
                    
                    foreach (TOSymbolInfo item in aux)
                    {
                        if (item.VlrUltimoAnterior != item.Instrumento.VlrUltima)
                        {
                            this.CalculatePositionClientByMds(item.Instrumento);
                            ConsolidatedRiskManager.Instance.CalcularTotalCustodia(item.Instrumento);
                            item.VlrUltimoAnterior = item.Instrumento.VlrUltima;
                            item.DtUpdated = DateTime.Now;
                        }
                        else
                        {
                            item.DtUpdated = DateTime.Now;
                        }
                    }
                    _isExecutingMDS = false;
                    Thread.Sleep(sleep);
                }
                catch (Exception ex)
                {
                    logger.ErrorFormat(ex.Message, ex);
                }

            }
        }

        /*public void CalculatePositionClientByMds(List<TOSymbolInfo> symbs)
        {
            try
            {
                foreach (TOSymbolInfo to in symbs)
                    to.DtUpdated = DateTime.Now;
                List<string> ativos = symbs.Select(x=>x.Instrumento.Instrumento).ToList();
                Dictionary<string, SymbolInfo> dic = symbs.ToDictionary(x=> x.Instrumento.Instrumento, x=>x.Instrumento);
                List<ExecSymbolInfo> lst = new List<ExecSymbolInfo>();
                foreach (KeyValuePair<int, ConcurrentDictionary<int, ExecSymbolInfo>> item in _dicExecSymbols)
                {
                    lst.AddRange(item.Value.Values.Where(x => ativos.Contains(x.Symbol)));
                }
                
                foreach (ExecSymbolInfo es in lst)
                {
                    ConcurrentDictionary<string, PosClientSymbolInfo> aa = null;
                    if (_dicPositionClient.TryGetValue(es.Account, out aa))
                    {
                        PosClientSymbolInfo pc = null;
                        if (aa.TryGetValue(es.Symbol, out pc))
                        {
                            SymbolInfo symb = dic[es.Symbol];
                            if (pc.UltPreco.Equals(symb.VlrUltima))
                                continue;

                            if (pc.Bolsa.Equals(Exchange.Bovespa, StringComparison.CurrentCultureIgnoreCase))
                            {
                                // Calculo Bov: preco medio e qtd.
                                pc.UltPreco = symb.VlrUltima;
                                pc.Variacao = symb.VlrOscilacao;
                                pc.FinancNet = pc.NetExec * pc.UltPreco;
                                decimal buy = (pc.UltPreco - pc.PcMedC) * pc.QtdExecC;
                                decimal sell = (pc.PcMedV - pc.UltPreco) * pc.QtdExecV;
                                pc.LucroPrej = buy + sell;
                            }
                            else
                            {
                                pc.Variacao = symb.VlrOscilacao;
                                pc.UltPreco = symb.VlrUltima;
                                // Calculo BMF: de acordo com o tipo de mercado
                                if (!BmfCalculator.Instance.CalcularPosicaoBmf(pc))
                                {
                                    logger.Error("Problemas no calculo de Finaceiro Net e L/P de BMF");
                                }
                            }
                            pc.OrderInfoCalc = null;
                            pc.DtMovimento = DateTime.Now;
                            pc.MsgId++;
                            pc.EventSource = "POSCLIMDS";

                            // logger.DebugFormat("CalculatePositioClientByMDS: Account[{0}] Symbol[{1}] Exchange[{2}] L/P[{3}]", pc.Account, pc.Ativo, pc.Bolsa, pc.LucroPrej);
                            // Efetuar chamada para calculo de risco consolidado
                            ConsolidatedRiskManager.Instance.CalculateConsolidatedPosition(pc);
                            //}
                            if (OnPositionClientUpdate != null)
                            {
                                PositionClientEventArgs sync = new PositionClientEventArgs();
                                sync.Action = EventAction.UPDATE;
                                PosClientSymbolInfo aux = null;
                                lock (pc)
                                    aux = Cloner.ClonePosClient(pc);
                                sync.Account = aux.Account;
                                List<PosClientSymbolInfo> lst1 = new List<PosClientSymbolInfo>();
                                lst1.Add(aux);
                                sync.PosClient = lst1;
                                OnPositionClientUpdate(this, sync);
                            }
                        }

                    }
                    
                }

            }
            catch (Exception ex)
            {
                logger.Error("Problemas no calculo de position client por simbolo: " + ex.Message, ex);
            }
        }*/

        public void CalculatePositionClientByMds(SymbolInfo symb)
        {
            try
            {
                List<PosClientSymbolInfo> lst = null;
                if (_dicPosClientBySymbol.TryGetValue(symb.Instrumento, out lst))
                {
                    // Verificar somente os que tem preco medio de compra ou venda 
                    List<PosClientSymbolInfo> lstAux = lst.Select(x => x)
                        //.Where(x => !x.PcMedC.Equals(decimal.Zero) || !x.PcMedV.Equals(decimal.Zero))
                        .ToList();
                    foreach (PosClientSymbolInfo pc in lstAux)
                    {
                        //lock (pc)
                        //{
                        if (pc.UltPreco.Equals(symb.VlrUltima))
                            continue;

                        var lClientes = ConsolidatedRiskManager.Instance.ListaClientes;

                        var lCliente = lClientes.Find(cliente => cliente.CodigoBovespa == pc.Account || cliente.CodigoBmf == pc.Account);

                        if (lCliente != null)
                        {
                            pc.NomeCliente = lCliente.NomeCliente;
                        }
                        
                        if (pc.QtdExecC != 0 || pc.QtdExecV != 0)
                        {
                            this.UptTradeDetail(pc, symb);
                        }

                        if (pc.Bolsa.Equals(Exchange.Bovespa, StringComparison.CurrentCultureIgnoreCase))
                        {
                            // Calculo Bov: preco medio e qtd.
                            pc.UltPreco = symb.VlrUltima;
                            pc.Variacao = symb.VlrOscilacao;
                            decimal buy = (pc.UltPreco - pc.PcMedC) * pc.QtdExecC;
                            decimal sell = (pc.PcMedV - pc.UltPreco) * pc.QtdExecV;
                            pc.LucroPrej = buy + sell;
                            pc.LucroPrejuizoSemAbertura = buy + sell;

                            decimal lAbertura = decimal.Zero;

                            if (pc.QtdAbertura != 0)
                            {
                                lAbertura = pc.QtdAbertura * pc.UltPreco;
                                //pc.LucroPrej += lAbertura;
                            }


                            //pc.FinancNet = lAbertura + (pc.NetExec * pc.UltPreco);

                            pc.FinancNet = pc.NetExec * pc.UltPreco;
                        }
                        else
                        {
                            pc.Variacao = symb.VlrOscilacao;
                            pc.UltPreco = symb.VlrUltima;
                            // Calculo BMF: de acordo com o tipo de mercado
                            if (!BmfCalculator.Instance.CalcularPosicaoBmf(pc))
                            {
                                logger.Error("Problemas no calculo de Finaceiro Net e L/P de BMF");
                            }
                        }
                        pc.OrderInfoCalc = null;
                        pc.DtMovimento = DateTime.Now;
                        pc.MsgId++;
                        pc.EventSource = "POSCLIMDS";

                        // logger.DebugFormat("CalculatePositioClientByMDS: Account[{0}] Symbol[{1}] Exchange[{2}] L/P[{3}]", pc.Account, pc.Ativo, pc.Bolsa, pc.LucroPrej);
                        // Efetuar chamada para calculo de risco consolidado
                        ConsolidatedRiskManager.Instance.CalculateConsolidatedPosition(pc);
                        //}
                        if (OnPositionClientUpdate != null)
                        {
                            PositionClientEventArgs sync = new PositionClientEventArgs();
                            sync.Action = EventAction.UPDATE;
                            PosClientSymbolInfo aux = null;
                            lock (pc)
                                aux = Cloner.ClonePosClient(pc);
                            sync.Account = aux.Account;
                            List<PosClientSymbolInfo> lst1 = new List<PosClientSymbolInfo>();
                            lst1.Add(aux);
                            sync.PosClient = lst1;
                            OnPositionClientUpdate(this, sync);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Problemas no calculo de position client por simbolo: " + ex.Message, ex);
            }
        }

        public void CalculatePositionClientAberturaByMds(PosClientSymbolInfo pc)
        {
            try
            {
                SymbolInfo lSymbol = null;

                if (RiskCache.Instance._dicSymbols.ContainsKey(pc.Ativo))
                {
                    lSymbol = RiskCache.Instance._dicSymbols[pc.Ativo];
                }
                else
                {
                    return;
                }

                if (pc.Bolsa.Equals(Exchange.Bovespa, StringComparison.CurrentCultureIgnoreCase))
                {
                    pc.UltPreco = lSymbol.VlrUltima;
                    pc.Variacao = lSymbol.VlrOscilacao;
                    decimal buy = (pc.UltPreco - pc.PcMedC) * pc.QtdExecC;
                    decimal sell = (pc.PcMedV - pc.UltPreco) * pc.QtdExecV;
                    //pc.LucroPrej = buy + sell;

                    decimal lAbertura = decimal.Zero;

                    if (pc.QtdAbertura != 0)
                    {
                        lAbertura = pc.QtdAbertura * pc.UltPreco;
                        pc.LucroPrej = lAbertura;
                    }

                    pc.FinancNet = lAbertura + (pc.NetExec * pc.UltPreco);

                    //pc.FinancNet = pc.NetExec * pc.UltPreco;
                }
                else
                {
                    pc.Variacao = lSymbol.VlrOscilacao;
                    pc.UltPreco = lSymbol.VlrUltima;
                    // Calculo BMF: de acordo com o tipo de mercado
                    if (!BmfCalculator.Instance.CalcularPosicaoBmf(pc))
                    {
                        logger.Error("Problemas no calculo de Finaceiro Net e L/P de BMF");
                    }
                }

                pc.OrderInfoCalc = null;
                pc.DtMovimento = DateTime.Now;
                pc.MsgId++;
                pc.EventSource = "POSCLIMDS";

                ConsolidatedRiskManager.Instance.CalculateConsolidatedPosition(pc);
            }
            catch (Exception ex)
            {
                logger.Error("Problemas no calculo de position client Abertura: " + ex.Message, ex);
            }
        }

        #endregion
    }
}
