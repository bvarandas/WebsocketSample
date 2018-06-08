using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Collections.Concurrent;
using Gradual.Spider.SupervisorRisco.Lib.Dados;
using Gradual.Spider.SupervisorRisco.DB.Lib;
using System.Threading;
using System.Configuration;
using Gradual.Spider.SupervisorRisco.Lib.Handlers;
using Gradual.Spider.ServicoSupervisor.Calculator;

namespace Gradual.Spider.ServicoSupervisor.Memory
{
    public class ConsolidatedRiskManager
    {

        public static readonly log4net.ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Private Variables
        ConcurrentDictionary<int, ConsolidatedRiskInfo> _cdRiscoConsolidado;
        ConcurrentDictionary<string, CRLucroPrej> _cdCRLucroPrej;
        ConcurrentDictionary<int, ListClientPosition> _ListPostion;
        DbRisco _dbRisco;
        DbRiscoOracle _dbOracle;
        bool _isRunning = false;
        object _syncDb = new object();

        bool _isRunningDB = false;
        
        Thread _thRiskDB = null;

        object _syncSnap = new object();

        //ConcurrentDictionary<string, CustodiaInfo> _cdBvsp;
        // ConcurrentDictionary<string, CustodiaInfo> _cdBmf;
        ConcurrentDictionary<string, List<CustodiaInfo>> _cdBvsp;
        ConcurrentDictionary<string, List<CustodiaInfo>> _cdBmf;
        ConcurrentDictionary<int, List<CustodiaInfo>> _cdBvspByAcc;
        ConcurrentDictionary<int, List<CustodiaInfo>> _cdBmfByAcc;
        ConcurrentDictionary<int, TesouroDiretoAbInfo> _cdTedi;
        
        bool _isLoadingCRM = false;

        #endregion
        public delegate void ConsolidatedRiskUpdateHandler(object sender, ConsolidatedRiskEventArgs args);
        public event ConsolidatedRiskUpdateHandler OnConsolidatedRiskUpdate;

        public List<ClientDataInfo> ListaClientes { get; set; }

        private static ConsolidatedRiskManager _me = null;
        public static ConsolidatedRiskManager Instance
        {
            get
            {
                if (_me == null)
                {
                    _me = new ConsolidatedRiskManager();
                }
                return _me;
            }
        }

        public ConsolidatedRiskManager()
        {
            
            
        }

        public void Start()
        {
            try
            {
                logger.Info("Iniciando Manager Risco Consolidado...");
                if (null == _dbRisco)
                    _dbRisco = new DbRisco();
                if (null == _dbOracle)
                    _dbOracle = new DbRiscoOracle();
                
                _ListPostion = new ConcurrentDictionary<int,ListClientPosition>();

                _cdRiscoConsolidado = new ConcurrentDictionary<int, ConsolidatedRiskInfo>();
                
                _cdCRLucroPrej = new ConcurrentDictionary<string, CRLucroPrej>();

                ListaClientes = new List<ClientDataInfo>();

                logger.Info("Carregar informacoes de abertura (custodia, conta corrente, garantias, produtos...");

                logger.Info("Limpando valores de tb_risco_consolidado...");
                _dbRisco.LimparRiscoConsolidado();

                logger.Info("Carregando informacoes iniciais de risco consolidado");
                this.LoadConsolidatedRisk();


                _isRunning = true;
                logger.Info("Iniciando thread de atualizacao no banco de dados...");
                _thRiskDB = new Thread(new ThreadStart(_processConsolidatedRiskDB));
                _thRiskDB.Start();
                
                
                
            }
            catch (Exception ex)
            {
                logger.Error("Problemas no start do manager Risco Consolidado: " + ex.Message, ex);
                throw;
            }
        }

        public void Stop()
        {
            try
            {
                _isRunning = false;
                logger.Info("Parando Manager Risco Consolidado...");

                logger.Info("Parando thread Banco de Dados");
                if (_thRiskDB != null && _thRiskDB.IsAlive)
                {
                    _thRiskDB.Join(500);
                    if (_thRiskDB.IsAlive)
                        _thRiskDB.Abort();
                    _thRiskDB = null;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Problemas no stop de Risco Consolidado: " + ex.Message, ex);
                throw;
            }
        }


        #region Cargas iniciais de risco consolidado
        /// <summary>
        /// Carrego inicial do dia das garantias de carteiras de bmf 
        /// </summary>
        /// <param name="CodigoCliente">Codigo do cliente</param>
        /// <param name="pGarantia">valor da garantia bmf</param>
        /// <returns>Retorna o valor sumarizado de bmf</returns>
        public GarantiaInfo CarregarGarantiasBmfCarteiras(int CodigoCliente, GarantiaInfo pGarantia, out decimal pValorGarantiaCarteira)
        {
            GarantiaInfo lRetorno = pGarantia;

            pValorGarantiaCarteira = 0;

            try
            {
                decimal lValorDisponivel = 0M;
                
                var lLista = _dbOracle.CarregarGarantiaBmfCarteiras(CodigoCliente);

                if (lLista == null)
                {
                    logger.Error("Problemas na carga de garantias de carteira do cliente "+ CodigoCliente +" de BMF");
                }

                foreach (GarantiaInfo info in lLista)
                {
                    SymbolInfo lSymbolInfo = null;

                    if(RiskCache.Instance._dicSymbols.TryGetValue(info.Instrumento, out lSymbolInfo))
                    {
                        lValorDisponivel+= BmfCalculator.Instance.CalcularCustodiaAberturaBmf(lSymbolInfo, info.Quantidade);
                    }
                }

                pValorGarantiaCarteira = lValorDisponivel;

                lRetorno.GarantiaDisponivel += lValorDisponivel;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na carga inicial das informacoes de garantia da carteira 23906 do cliente : " + CodigoCliente +" - " + ex.Message, ex);
            }

            return lRetorno;
        }

        /// <summary>
        /// Carrego inicial do dia das garantias de carteiras de bovespa
        /// </summary>
        /// <param name="CodigoCliente">Código do cliente</param>
        /// <param name="pGarantia">Valor da garantia Bovespa</param>
        /// <returns>Retorna o valor sumarizado de bovespa</returns>
        public GarantiaInfo CarregarGarantiasBovCarteiras(int CodigoCliente, GarantiaInfo pGarantia, out decimal pValorGarantiaCarteira)
        {
            GarantiaInfo lRetorno = pGarantia;

            pValorGarantiaCarteira = 0;

            try
            {
                decimal lValorDisponivel = 0M;

                var lLista = _dbOracle.CarregarGarantiaBovespaCarteiras(CodigoCliente);

                if (lLista == null)
                {
                    logger.Error("Problemas na carga de garantias de carteira do cliente " + CodigoCliente + " de Bovespa");
                }

                foreach (GarantiaInfo info in lLista)
                {
                    SymbolInfo lSymbolInfo = null;

                    if (RiskCache.Instance._dicSymbols.TryGetValue(info.Instrumento, out lSymbolInfo))
                    {
                        lValorDisponivel +=  (lSymbolInfo.VlrUltima * info.Quantidade);
                    }
                }

                lRetorno.GarantiaDisponivel += lValorDisponivel;

                pValorGarantiaCarteira = lValorDisponivel;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na carga inicial das informacoes de garantia bovespa da carteira 23019 do cliente : " + CodigoCliente + " - " + ex.Message, ex);
            }

            return lRetorno;
        }
        
        /// <summary>
        /// Carrego inicial do dia das garantias das tabelas do sinacor de bovespa e bmf
        /// </summary>
        public void CarregarGarantias()
        {
            try
            {
                ConcurrentDictionary<int, GarantiaInfo> cdBov = _dbOracle.CarregarGarantiaBovespa();
                ConcurrentDictionary<int, GarantiaInfo> cdBmf = _dbOracle.CarregarGarantiaBmf();
                if (cdBov == null)
                {
                    logger.Error("Problemas na carga de garantias de BOVESPA");
                    return;
                }
                if (cdBmf == null)
                {
                    logger.Error("Problemas na carga de garantias de BMF");
                    return;
                }

                logger.InfoFormat("Fazendo cargas de garantias BOVESPA[{0}] BMF[{1}]...", cdBov.Count, cdBmf.Count);

                foreach (KeyValuePair<int, GarantiaInfo> gBov in cdBov)
                {
                    ConsolidatedRiskInfo cr = null;
                    logger.InfoFormat("Garantia Bovespa - Account[{0}] GarantiaDisponivel[{1}]", gBov.Key, gBov.Value.GarantiaDisponivel);
                    if (_cdRiscoConsolidado.TryGetValue(gBov.Key, out cr))
                        cr.TotalGarantias += gBov.Value.GarantiaDisponivel;
                    else
                    {
                        cr = new ConsolidatedRiskInfo();
                        cr.TotalGarantias += gBov.Value.GarantiaDisponivel;
                        cr.Account = gBov.Key;

                    }

                    decimal lValorGarantiaCarteira = 0;

                    var lGarantia = CarregarGarantiasBovCarteiras(cr.Account, gBov.Value, out lValorGarantiaCarteira);

                    cr.TotalGarantias = lGarantia.GarantiaDisponivel;

                    cr.TotalGarantiaARemoverCustodia = lValorGarantiaCarteira;

                    cr.SaldoTotalAbertura = cr.TotalContaCorrenteOnline + ( cr.TotalCustodiaAbertura + cr.TotalGarantiaARemoverCustodia) + cr.TotalGarantias + cr.TotalProdutos;
                    cr.SFP = cr.SaldoTotalAbertura + cr.PLTotal;
                    cr.DtMovimento = DateTime.Now;
                    _cdRiscoConsolidado.AddOrUpdate(gBov.Key, cr, (key, oldvalue) => cr);
                }

                foreach (KeyValuePair<int, GarantiaInfo> gBmf in cdBmf)
                {
                    ConsolidatedRiskInfo cr = null;
                    logger.InfoFormat("Garantia Bmf - Account[{0}] GarantiaDisponivel[{1}]", gBmf.Key, gBmf.Value.GarantiaDisponivel);
                    
                    if (_cdRiscoConsolidado.TryGetValue(gBmf.Key, out cr))
                        cr.TotalGarantias += gBmf.Value.GarantiaDisponivel;
                    else
                    {
                        cr = new ConsolidatedRiskInfo();
                        cr.TotalGarantias += gBmf.Value.GarantiaDisponivel;
                        cr.Account = gBmf.Key;
                    }

                    decimal lValorGarantiaCarteira = 0;

                    var lGarantia = CarregarGarantiasBmfCarteiras(cr.Account, gBmf.Value, out lValorGarantiaCarteira);

                    cr.TotalGarantias = lGarantia.GarantiaDisponivel;

                    cr.TotalGarantiaARemoverCustodia = lValorGarantiaCarteira;

                    cr.SaldoTotalAbertura = cr.TotalContaCorrenteOnline + (cr.TotalCustodiaAbertura + cr.TotalGarantiaARemoverCustodia) + cr.TotalGarantias + cr.TotalProdutos;
                    cr.SFP = cr.SaldoTotalAbertura + cr.PLTotal;
                    cr.DtMovimento = DateTime.Now;
                    _cdRiscoConsolidado.AddOrUpdate(gBmf.Key, cr, (key, oldvalue) => cr);
                }

            }
            catch (Exception ex)
            {
                logger.Error("Problemas na carga inicial das informacoes de garantia: " + ex.Message, ex);
                throw;
            }

        }

        public void CarregarProdutos()
        {
            try
            {
                ConcurrentDictionary<int, ProdutosInfo> cdProd = _dbRisco.CarregarProdutos();
                if (cdProd != null)
                {
                    foreach (KeyValuePair<int, ProdutosInfo> item in cdProd)
                    {
                        ConsolidatedRiskInfo cr = null;
                        logger.InfoFormat("Produtos - Account[{0}] Produto Saldo Liquido[{1}]", item.Key, item.Value.SaldoLiquido);
                        if (_cdRiscoConsolidado.TryGetValue(item.Key, out cr))
                        {
                            cr.TotalProdutos = item.Value.SaldoLiquido;
                        }
                        else
                        {
                            cr = new ConsolidatedRiskInfo();
                            cr.TotalProdutos = item.Value.SaldoLiquido;
                            cr.Account = item.Key;
                        }
                        cr.SaldoTotalAbertura = cr.TotalContaCorrenteOnline + cr.TotalCustodiaAbertura + cr.TotalGarantias + cr.TotalProdutos;
                        cr.SFP = cr.SaldoTotalAbertura + cr.PLTotal;
                        cr.DtMovimento = DateTime.Now;
                        _cdRiscoConsolidado.AddOrUpdate(item.Key, cr, (key, oldvalue) => cr);
                    }
                    logger.InfoFormat("Fazendo carga de produtos. Total[{0}]", cdProd.Count);
                }
                else
                {
                    logger.Error("Problemas na carga de produtos!!");
                    return;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na carga inicial das informacoes de produtos (fundos): " + ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// Carrega as posições de tomador de btc de todos os clientes que a têm para depois subtrair do
        /// valor da custodia
        /// </summary>
        public void CarregarPosicaoBTCTomador()
        {
            try
            {
                var lListaBTC = _dbOracle.ObterPosicaoBTC();
                //primeiro limpamos os valores de BTC para ser carregados novamente

                foreach( KeyValuePair<int,ConsolidatedRiskInfo>  risco in _cdRiscoConsolidado)
                {
                    risco.Value.TotalBtcTomador = 0;
                }


                for (int i = 0; i < lListaBTC.Count; i++)
                {
                    BTCInfo lInfo = lListaBTC[i];

                    string Instrumento = lInfo.Instrumento;

                    SymbolInfo s = RiskCache.Instance.GetSymbol(Instrumento);

                    lListaBTC[i].PrecoMercado = s.VlrUltima;

                    ConsolidatedRiskInfo lConsolidatedInfo = null;

                    decimal lValorBTC = (s.VlrUltima * lInfo.Quantidade);

                    if (!_cdRiscoConsolidado.TryGetValue(lInfo.CodigoCliente, out lConsolidatedInfo))
                    {
                        lConsolidatedInfo = new ConsolidatedRiskInfo();
                        lConsolidatedInfo.Account = lInfo.CodigoCliente;
                        lConsolidatedInfo.TotalBtcTomador = lValorBTC;
                    }
                    else
                    {
                        lConsolidatedInfo.TotalBtcTomador += lValorBTC;
                    }

                    _cdRiscoConsolidado.AddOrUpdate(lInfo.CodigoCliente, lConsolidatedInfo, (key, oldvalue) => lConsolidatedInfo);

                }
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Problemas na carga de Posição de BTC do cliente", ex);
            }
        }

        /// <summary>
        /// Carrega os dados de conta corrente do cliente específico
        /// </summary>
        /// <param name="CodigoCliente">Codigo de cliente</param>
        public void CarregarContaCorrente(int CodigoCliente)
        {
            try
            {
                ContaCorrenteInfo cdCc = _dbOracle.CarregarContaCorrenteCliente(CodigoCliente);

                if (cdCc != null)
                {
                    ConsolidatedRiskInfo cr = null;

                    if (_cdRiscoConsolidado.TryGetValue(CodigoCliente, out cr))
                    {
                        cr.TotalContaCorrenteOnline = cdCc.VlrTotal;
                    }
                    else
                    {
                        cr = new ConsolidatedRiskInfo();
                        cr.TotalContaCorrenteOnline = cdCc.VlrTotal;
                        cr.Account = CodigoCliente;
                    }

                    cr.SaldoTotalAbertura = cr.TotalContaCorrenteOnline + cr.TotalCustodiaAbertura + cr.TotalGarantias + cr.TotalProdutos;

                    cr.SFP = cr.SaldoTotalAbertura + cr.PLTotal;

                    cr.DtMovimento = DateTime.Now;

                    _cdRiscoConsolidado.AddOrUpdate(CodigoCliente, cr, (key, oldvalue) => cr);

                    logger.InfoFormat("Fazendo carga de conta corrente do cliente : [{0}]", cdCc.CodCliente);
                }
                else
                {
                    logger.ErrorFormat("Problemas na carga de conta corrente do cliente  : [{0}]!!", CodigoCliente);
                    return;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na carga de informacoes de conta corrente: " + ex.Message, ex);

                throw ex;
            }
        }

        /// <summary>
        /// Carrega os dados de conta corrente de todos os clientes
        /// </summary>
        public void CarregarContaCorrente()
        {
            try
            {
                ConcurrentDictionary<int, ContaCorrenteInfo> cdCc = _dbOracle.CarregarContaCorrente();
                if (cdCc != null)
                {
                    foreach (KeyValuePair<int, ContaCorrenteInfo> item in cdCc)
                    {
                        ConsolidatedRiskInfo cr = null;
                        logger.InfoFormat("ContaCorrente - Account[{0}] VlrTotal[{1}]", item.Key, item.Value.VlrTotal);
                        if (_cdRiscoConsolidado.TryGetValue(item.Key, out cr))
                        {
                            cr.TotalContaCorrenteOnline = item.Value.VlrTotal;
                        }
                        else
                        {
                            cr = new ConsolidatedRiskInfo();
                            cr.TotalContaCorrenteOnline = item.Value.VlrTotal;
                            cr.Account = item.Key;
                        }
                        cr.SaldoTotalAbertura = cr.TotalContaCorrenteOnline + cr.TotalCustodiaAbertura + cr.TotalGarantias + cr.TotalProdutos;
                        cr.SFP = cr.SaldoTotalAbertura + cr.PLTotal;
                        cr.DtMovimento = DateTime.Now;
                        _cdRiscoConsolidado.AddOrUpdate(item.Key, cr, (key, oldvalue) => cr);
                    }
                    logger.InfoFormat("Fazendo carga de conta corrente: [{0}]", cdCc.Count);
                }
                else
                {
                    logger.Error("Problemas na carga de conta corrente!!");
                    return;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na carga de informacoes de conta corrente: " + ex.Message, ex);
                throw;
            }
        }

        public void CarregarCustodiaBovespa()
        {
            try
            {
                _cdBvsp = _dbOracle.CarregarCustodiaBvsp();
                
                _cdBvspByAcc = new ConcurrentDictionary<int, List<CustodiaInfo>>();
                
                if (_cdBvsp != null)
                {
                    foreach (KeyValuePair<string, List<CustodiaInfo>> item2 in _cdBvsp)
                    {
                        foreach (CustodiaInfo item in item2.Value)
                        {
                            logger.InfoFormat("CarregarCustodiaBovespa - Account[{0}] Symbol[{1}] Qtd[{2}]", item.Account, item.Symbol, item.Qty);

                            List<CustodiaInfo> lstByAcc = null;
                            
                            if (!_cdBvspByAcc.TryGetValue(item.Account, out lstByAcc))
                            {
                                lstByAcc = new List<CustodiaInfo>();

                                lstByAcc.Add(item);
                                
                                _cdBvspByAcc.AddOrUpdate(item.Account, lstByAcc, (key, old) => lstByAcc);
                            }
                            else
                            {
                                lstByAcc.Add(item);
                            }
                             
                            SymbolInfo s = RiskCache.Instance.GetSymbol(item.Symbol);
                            
                            if (s != null)
                            {
                                ConsolidatedRiskInfo cr = null;

                                if (s.LotePadrao > 100)
                                {
                                    item.ValorCustodia = (item.Qty * s.VlrFechamento) / s.LotePadrao;
                                }
                                else
                                {
                                    item.ValorCustodia = item.Qty * s.VlrFechamento;
                                }

                                if (_cdRiscoConsolidado.TryGetValue(item.Account, out cr))
                                {
                                    cr.TotalCustodiaBvsp = cr.TotalCustodiaBvsp + item.ValorCustodia;
                                }
                                else
                                {
                                    cr = new ConsolidatedRiskInfo();
                                    
                                    cr.TotalCustodiaBvsp = (item.ValorCustodia);
                                    
                                    cr.Account = item.Account;
                                }

                                cr.TotalCustodiaAbertura = cr.TotalCustodiaBvsp + cr.TotalCustodiaBmf;// +cr.TotalCustodiaTesouroDireto;

                                cr.TotalCustodiaAberturaFixa = cr.TotalCustodiaBvsp + cr.TotalCustodiaBmf;

                                cr.SaldoTotalAbertura = cr.TotalContaCorrenteOnline + cr.TotalCustodiaAbertura + cr.TotalGarantias + cr.TotalProdutos;
                                
                                cr.SFP = cr.SaldoTotalAbertura + cr.PLTotal;
                                
                                //cr.TotalProdutos = c

                                cr.DtMovimento = DateTime.Now;
                                
                                _cdRiscoConsolidado.AddOrUpdate(cr.Account, cr, (key, oldvalue) => cr);
                            }
                        }
                    }
                }
                else
                    logger.Error("CarregarCustodiaBovespa - Erro na carga da collection. Dicionario nulo");
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na carga de informacoes de custodia bovespa: " + ex.Message, ex);
            }
        }

        public void CarregarCustodiaBmf()
        {
            try
            {
                _cdBmf = _dbOracle.CarregarCustodiaBmf();

                _cdBmfByAcc = new ConcurrentDictionary<int, List<CustodiaInfo>>();
                
                if (_cdBmf != null)
                {
                    foreach (KeyValuePair<string, List<CustodiaInfo>> item2 in _cdBmf)
                    {
                        foreach (CustodiaInfo item in item2.Value)
                        {
                            logger.InfoFormat("CarregarCustodiaBmf - Account[{0}] Symbol[{1}] Qtd[{2}]", item.Account, item.Symbol, item.Qty);
                            
                            List<CustodiaInfo> lstByAcc = null;
                            if (!_cdBmfByAcc.TryGetValue(item.Account, out lstByAcc))
                            {
                                lstByAcc = new List<CustodiaInfo>();
                                lstByAcc.Add(item);
                                _cdBmfByAcc.AddOrUpdate(item.Account, lstByAcc, (key, old) => lstByAcc);
                            }
                            else
                            {
                                lstByAcc.Add(item);
                            }
                            
                            SymbolInfo s = RiskCache.Instance.GetSymbol(item.Symbol);

                            if (s != null)
                            {
                                ConsolidatedRiskInfo cr = null;

                                decimal bmf = BmfCalculator.Instance.CalcularCustodiaAberturaBmf(s, item.Qty);

                                item.ValorCustodia = bmf;

                                if (_cdRiscoConsolidado.TryGetValue(item.Account, out cr))
                                {
                                    cr.TotalCustodiaBmf = cr.TotalCustodiaBmf + item.ValorCustodia;
                                }
                                else
                                {
                                    cr = new ConsolidatedRiskInfo();

                                    cr.TotalCustodiaBmf = item.ValorCustodia;

                                    cr.Account = item.Account;
                                }
                                
                                cr.TotalCustodiaAbertura = cr.TotalCustodiaBvsp + cr.TotalCustodiaBmf;// +cr.TotalCustodiaTesouroDireto;

                                cr.TotalCustodiaAberturaFixa = cr.TotalCustodiaBvsp + cr.TotalCustodiaBmf;

                                cr.SaldoTotalAbertura = cr.TotalContaCorrenteOnline + cr.TotalCustodiaAbertura + cr.TotalGarantias + cr.TotalProdutos;
                                
                                cr.SFP = cr.SaldoTotalAbertura + cr.PLTotal;
                                
                                cr.DtMovimento = DateTime.Now;
                                
                                _cdRiscoConsolidado.AddOrUpdate(cr.Account, cr, (key, oldvalue) => cr);

                                CalculateConsolidatedPosition(new PosClientSymbolInfo() { Ativo = s.Instrumento, Account = cr.Account, QtdAbertura = item.Qty });
                                //PositionClientManager.Instance.MontaPositionClientAberturaInfo(s, item.Account, item.Qty, Exchange.Bmf);
                            }
                        }
                    }
                }
                else
                    logger.Error("CarregarCustodiaBmf - Erro na carga da collection. Dicionario nulo");
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na carga de informacoes de custodia bmf: " + ex.Message, ex);
            }
        }

        public void CarregarCustodiaTesouroDireto()
        {
            try
            {
                _cdTedi = _dbOracle.CarregarTesouroDireto();
                if (_cdTedi != null)
                {
                    foreach (KeyValuePair<int, TesouroDiretoAbInfo> item in _cdTedi)
                    {
                        logger.InfoFormat("CarregarCustodiaTesouroDireto - Account[{0}] Valor[{1}]", item.Value.CodCliente, item.Value.ValPosi);
                        ConsolidatedRiskInfo cr = null;
                        if (_cdRiscoConsolidado.TryGetValue(item.Value.CodCliente, out cr))
                        {
                            cr.TotalCustodiaTesouroDireto = cr.TotalCustodiaTesouroDireto + item.Value.ValPosi;
                        }
                        else
                        {
                            cr = new ConsolidatedRiskInfo();
                            cr.TotalCustodiaTesouroDireto = item.Value.ValPosi;
                            cr.Account = item.Value.CodCliente;
                        }
                        cr.TotalProdutos += cr.TotalCustodiaTesouroDireto;
                        cr.TotalCustodiaAbertura = cr.TotalCustodiaBvsp + cr.TotalCustodiaBmf;//; + 
                        cr.SaldoTotalAbertura = cr.TotalContaCorrenteOnline + cr.TotalCustodiaAbertura + cr.TotalGarantias + cr.TotalProdutos;
                        cr.SFP = cr.SaldoTotalAbertura + cr.PLTotal;
                        cr.DtMovimento = DateTime.Now;
                        _cdRiscoConsolidado.AddOrUpdate(cr.Account, cr, (key, oldvalue) => cr);
                    }
                }
                else
                {
                    logger.Error("CarregarCustodiaTesouroDireto - Collection nulo ou vazio");
                }
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na carga de custodia de tesouro direto: " + ex.Message, ex);
            }
            
        }

        public void CarregarDadosCliente()
        {
            try
            {
                var lDb = new DbRiscoOracle();

                ListaClientes = lDb.CarregarDadosCliente();

                foreach( KeyValuePair<int,ConsolidatedRiskInfo>  risco in _cdRiscoConsolidado)
                {
                    var lCliente = ListaClientes.Find(cliente => cliente.CodigoBovespa == risco.Key || cliente.CodigoBmf == risco.Key);

                    if (lCliente != null)
                    {
                        risco.Value.NomeCliente = lCliente.NomeCliente;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Problemas no calculo de risco consolidado: " + ex.Message, ex);
            }
        }

        public void LoadConsolidatedRisk()
        {
            try
            {
                if (_isLoadingCRM)
                    logger.Info("LoadConsolidatedRisk já em execucao");

                _isLoadingCRM = true;
                this.CarregarGarantias();
                this.CarregarProdutos();
                this.CarregarPosicaoBTCTomador();
                this.CarregarContaCorrente();
                this.CarregarCustodiaBovespa();
                this.CarregarCustodiaBmf();
                this.CarregarCustodiaTesouroDireto();
                this.CarregarDadosCliente();
                _isLoadingCRM = false;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na carga inicial do risco consolidado: " + ex.Message, ex);
            }
        }

        #endregion
        
        public void CalculateConsolidatedPosition(PosClientSymbolInfo pc)
        {
            try
            {
                ConsolidatedRiskInfo item = null;

                if (!_cdRiscoConsolidado.TryGetValue(pc.Account, out item))
                {
                    item = new ConsolidatedRiskInfo();
                }

                var lCliente = ListaClientes.Find(cliente => cliente.CodigoBovespa == pc.Account || cliente.CodigoBmf == pc.Account);

                if (lCliente != null)
                {
                    item.NomeCliente    = lCliente.NomeCliente;
                    item.CodigoAssessor = lCliente.CodigoAssessor;
                }

                // Buscar o L/P anterior da position client para descontar
                string chave = string.Format("{0}-{1}", pc.Account, pc.Ativo);
                
                CRLucroPrej cr = null;

                if (!_cdCRLucroPrej.TryGetValue(chave, out cr))
                {
                    cr = new CRLucroPrej();
                    cr.Account = pc.Account;
                    cr.Ativo = pc.Ativo;
                    cr.Bolsa = pc.Bolsa;
                    //cr.LucroPrejuizoSemAbertura = pc.LucroPrejuizoSemAbertura;
                    //cr.NetFinan = pc.FinancNet;
                    //cr.QuantCompra = pc.QtdExecC.DBToInt32();
                    //cr.QuantVenda = pc.QtdExecV.DBToInt32();
                    //cr.LucroPrejuizo = pc.LucroPrej;

                    _cdCRLucroPrej.AddOrUpdate(chave, cr, (key, old) => cr);
                }
                                
                item.Account = pc.Account;
                                
                item.TotalCustodiaAbertura = item.TotalCustodiaAbertura - cr.NetFinan + pc.FinancNet;
                
                //logger.InfoFormat("A Custódia do cliente [{0}] foi de [{1}]", item.Account, item.TotalCustodiaAbertura.ToString("n2"));
                //cr.FinancAbertura = item.TotalCustodiaAbertura;

                if (pc.Bolsa.Equals(Exchange.Bovespa, StringComparison.CurrentCultureIgnoreCase))
                {
                    item.PLBovespa = item.PLBovespa - cr.LucroPrejuizo + pc.LucroPrej ;

                    //item.PLBovespaSemAbertura = item.PLBovespa - cr.LucroPrejuizoSemAbertura + pc.LucroPrejuizoSemAbertura;
                    item.PLBovespaSemAbertura = item.PLBovespa;//(- cr.LucroPrejuizoSemAbertura + pc.LucroPrejuizoSemAbertura);

                    AddQtdyCustodiaMemoryBovespa(pc);
                }
                else
                {
                    item.PLBmf = item.PLBmf - cr.LucroPrejuizo + pc.LucroPrej;
                    //item.PLBmf = item.TotalCustodiaAbertura - cr.LucroPrejuizo + pc.LucroPrej;
                    AddQtdyCustodiaMemoryBmf(pc);
                }

                cr.LucroPrejuizo = pc.LucroPrej;

                cr.LucroPrejuizoSemAbertura = pc.LucroPrejuizoSemAbertura;

                cr.NetFinan = pc.FinancNet;

                _cdCRLucroPrej.AddOrUpdate(chave, cr, (key, old) => cr);

                decimal lTotalCustodiacomBtc = item.TotalCustodiaAbertura - item.TotalBtcTomador - item.TotalGarantiaARemoverCustodia;

                item.SaldoTotalAbertura = lTotalCustodiacomBtc + item.TotalContaCorrenteOnline + item.TotalGarantias + item.TotalProdutos;

                logger.InfoFormat("Calculando Percentual Atigido do Cliente [{0}] PLBovespa[{1}] PLBmf [{2}] Total Custodia com BTC [{3}] Total Conta Corrente[{4}] Total Garantias[{5}] Total Produtos[{6}] Total BTC[{7}]",
                    item.Account, 
                    item.PLBovespa.ToString("N2"), 
                    item.PLBmf.ToString("N2"), 
                    item.TotalCustodiaAbertura.ToString("N2"), 
                    item.TotalContaCorrenteOnline.ToString("N2"),
                    item.TotalGarantias.ToString("N2"),
                    item.TotalProdutos.ToString("N2"),
                    item.TotalBtcTomador.ToString("N2"));

                item.PLTotal = item.PLBovespa + item.PLBmf;
                
                item.SFP = item.SaldoTotalAbertura + item.PLTotal;
                
                decimal lSaldoTotalSemCustodia = item.TotalContaCorrenteOnline + item.TotalGarantias + item.TotalProdutos;

                //logger.InfoFormat("Saldo total sem Custódia [{0}]",  lSaldoTotalSemCustodia);

                
                if ((item.PLTotal + lTotalCustodiacomBtc + lSaldoTotalSemCustodia) < 0)
                {
                    item.TotalPercentualAtingido = ((item.PLTotal + lTotalCustodiacomBtc) + lSaldoTotalSemCustodia)-100;
                }

                if (item.SaldoTotalAbertura.Equals(decimal.Zero))
                {
                    item.TotalPercentualAtingido = item.PLTotal;
                }

                if (item.SaldoTotalAbertura > 0)
                {
                    if (lTotalCustodiacomBtc < 0)
                    {
                        item.TotalPercentualAtingido = ((item.PLTotal + lTotalCustodiacomBtc) / lSaldoTotalSemCustodia) * 100;
                    }
                    else
                    {
                        item.TotalPercentualAtingido = (item.PLTotal / item.SaldoTotalAbertura) * 100;
                    }
                }
                else
                {
                    if (lTotalCustodiacomBtc < 0)
                    {
                        item.TotalPercentualAtingido = ((item.PLTotal + lTotalCustodiacomBtc) / ((lSaldoTotalSemCustodia == 0) ? 1 : lSaldoTotalSemCustodia)) * (100);

                        if ((item.PLTotal + lTotalCustodiacomBtc) < 0 && lSaldoTotalSemCustodia < 0)
                        {
                            item.TotalPercentualAtingido = ((item.PLTotal + lTotalCustodiacomBtc) / ((lSaldoTotalSemCustodia == 0) ? 1 : lSaldoTotalSemCustodia)) * (-100);
                        }

                    }
                    else 
                    {
                        item.TotalPercentualAtingido = ((item.PLTotal + lTotalCustodiacomBtc) / (item.SaldoTotalAbertura == 0 ? 1 : item.SaldoTotalAbertura)) * (-100);

                        if ((item.PLTotal + lTotalCustodiacomBtc) >= 0 && item.SaldoTotalAbertura == 0)
                        {
                            item.TotalPercentualAtingido = (item.PLTotal + lTotalCustodiacomBtc);
                        }
                    }

                    /*
                    if ((item.PLTotal + lTotalCustodiacomBtc) > 0)
                    {

                    }
                    */
                    
                }
                /*}*/

                logger.InfoFormat("Percentual Atigido do cliente [{0}] foi [{1}]", item.Account, item.TotalPercentualAtingido.ToString("N2"));

                item.DtMovimento = DateTime.Now;
                // Atualizar memoria

                if (pc.NetExec != 0)
                {
                    item.Zerado = false;
                }

                if (pc.QtdExecC != 0 || pc.QtdExecV != 0)
                {
                    item.OperouIntraday = true;
                }

                item.HorarioCalculo = DateTime.Now;

                _cdRiscoConsolidado.AddOrUpdate(item.Account, item, (key, oldvalue) => item);
                
                if (OnConsolidatedRiskUpdate != null)
                {
                    ConsolidatedRiskEventArgs sync = new ConsolidatedRiskEventArgs();
                    sync.Action = EventAction.UPDATE;
                    sync.Account = item.Account;
                    ConcurrentDictionary<int, ConsolidatedRiskInfo> cd = new ConcurrentDictionary<int, ConsolidatedRiskInfo>();
                    cd.AddOrUpdate(item.Account, item, (key, oldvalue) => item);
                    sync.ConsolidatedRisk = cd;
                    OnConsolidatedRiskUpdate(this, sync);
                    
                }
            }
            catch (Exception ex)
            {
                logger.Error("Problemas no calculo de risco consolidado: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Adiciona a quantidade de da position client de Bovespa na mémoria no dicitionary de custódia para ser calculado na corretamente no 
        /// método posterior (CalcularTotalCustodia) 
        /// </summary>
        /// <param name="pc">Position Cliente atulizado</param>
        public void AddQtdyCustodiaMemoryBovespa(PosClientSymbolInfo pc)
        {
            try
            {
                List<CustodiaInfo> lListCustodia = null;

                CustodiaInfo lBvsp = new CustodiaInfo();

                if (!_cdBvsp.TryGetValue(pc.Ativo, out lListCustodia))
                {
                    lListCustodia = new List<CustodiaInfo>();
                    _cdBvsp.AddOrUpdate(pc.Ativo, lListCustodia, (key, old) => lListCustodia);

                    CustodiaInfo cBvsp = new CustodiaInfo();
                    cBvsp.Account = pc.Account;
                    cBvsp.Symbol = pc.Ativo;
                    cBvsp.Qty =  (int)(pc.QtdExecC - pc.QtdExecV);
                    cBvsp.ValorCustodia = (cBvsp.Qty * pc.UltPreco);
                    lBvsp = cBvsp;

                    lListCustodia.Add(cBvsp);
                }
                else
                {
                    CustodiaInfo cBvsp = lListCustodia.Find(x => { return x.Account == pc.Account; });

                    if (cBvsp == null)
                    {
                        cBvsp = new CustodiaInfo();
                        cBvsp.Account = pc.Account;
                        cBvsp.Symbol = pc.Ativo;

                        lListCustodia.Add(cBvsp);
                    }

                    lBvsp = cBvsp;

                    cBvsp.Qty = (int)(pc.QtdAbertura + (pc.QtdExecC - pc.QtdExecV));
                    
                    cBvsp.ValorCustodia = (cBvsp.Qty * pc.UltPreco);

                    _cdBvsp.AddOrUpdate(pc.Ativo, lListCustodia, (key, old) => lListCustodia);
                }

                List<CustodiaInfo> lstByAcc = null;

                if (!_cdBvspByAcc.TryGetValue(pc.Account, out lstByAcc))
                {
                    lstByAcc = new List<CustodiaInfo>();

                    lstByAcc.Add(lBvsp);

                    _cdBvspByAcc.AddOrUpdate(pc.Account, lstByAcc, (key, old) => lstByAcc);
                }
                else
                {
                    CustodiaInfo lPapelBvsp = lstByAcc.Find(x => { return x.Symbol == pc.Ativo; });

                    if (lPapelBvsp == null)
                    {
                        lPapelBvsp = new CustodiaInfo();
                        lPapelBvsp.Account = pc.Account;
                        lPapelBvsp.Symbol = pc.Ativo;

                        lstByAcc.Add(lPapelBvsp);
                    }

                    lPapelBvsp.Qty = (int)(pc.QtdAbertura + (pc.QtdExecC - pc.QtdExecV));

                    lPapelBvsp.ValorCustodia = (lPapelBvsp.Qty * pc.UltPreco);

                    _cdBvspByAcc.AddOrUpdate(pc.Account, lstByAcc, (key, old) => lstByAcc);

                }

                
            }
            catch (Exception ex)
            {
                logger.Error("Problemas no método AddQtdyCustodiaMemoryBovespa: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Adiciona a quantidade de da position client de Bmf na mémoria no dicitionary de custódia para ser calculado na corretamente no 
        /// método posterior (CalcularTotalCustodia) 
        /// </summary>
        /// <param name="pc">Position CLient nova</param>
        public void AddQtdyCustodiaMemoryBmf(PosClientSymbolInfo pc)
        {
            try
            {
                List<CustodiaInfo> lListCustodia = null;

                CustodiaInfo lBmf = new CustodiaInfo();

                if (!_cdBmf.TryGetValue(pc.Ativo, out lListCustodia))
                {
                    lListCustodia = new List<CustodiaInfo>();
                    _cdBmf.AddOrUpdate(pc.Ativo, lListCustodia, (key, old) => lListCustodia);

                    CustodiaInfo cBmf   = new CustodiaInfo();
                    cBmf.Account        = pc.Account;
                    cBmf.Symbol         = pc.Ativo;
                    cBmf.Qty            = (int)(pc.QtdExecC - pc.QtdExecV);

                    cBmf.ValorCustodia  = BmfCalculator.Instance.CalcularCustodiaAberturaBmf(RiskCache.Instance.GetSymbol(pc.Ativo), cBmf.Qty); 
                    lBmf = cBmf;

                    lListCustodia.Add(cBmf);
                }
                else
                {
                    CustodiaInfo cBmf = lListCustodia.Find(x => { return x.Account == pc.Account; });

                    if (cBmf == null)
                    {
                        cBmf                = new CustodiaInfo();
                        cBmf.Account        = pc.Account;
                        cBmf.Symbol         = pc.Ativo;
                        

                        lListCustodia.Add(cBmf);
                    }

                    cBmf.Qty = (int)(pc.QtdAbertura + (pc.QtdExecC - pc.QtdExecV));

                    cBmf.ValorCustodia = BmfCalculator.Instance.CalcularCustodiaAberturaBmf(RiskCache.Instance.GetSymbol(pc.Ativo), cBmf.Qty);

                    lBmf = cBmf;

                    _cdBmf.AddOrUpdate(pc.Ativo, lListCustodia, (key, old) => lListCustodia);
                }

                List<CustodiaInfo> lstByAcc = null;

                if (!_cdBmfByAcc.TryGetValue(pc.Account, out lstByAcc))
                {
                    lstByAcc = new List<CustodiaInfo>();

                    lstByAcc.Add(lBmf);

                    _cdBmfByAcc.AddOrUpdate(pc.Account, lstByAcc, (key, old) => lstByAcc);
                }
                else
                {
                    CustodiaInfo lPapelBmf = lstByAcc.Find(x => { return x.Symbol == pc.Ativo; });

                    if (lPapelBmf == null)
                    {
                        lPapelBmf           = new CustodiaInfo();
                        lPapelBmf.Account   = pc.Account;
                        lPapelBmf.Symbol    = pc.Ativo;

                        lstByAcc.Add(lPapelBmf);
                    }

                    lPapelBmf.Qty = (int)(pc.QtdAbertura + (pc.QtdExecC - pc.QtdExecV));

                    lPapelBmf.ValorCustodia = BmfCalculator.Instance.CalcularCustodiaAberturaBmf(RiskCache.Instance.GetSymbol(pc.Ativo), lPapelBmf.Qty);

                    _cdBmfByAcc.AddOrUpdate(pc.Account, lstByAcc, (key, old) => lstByAcc);
                    
                }
            }
            catch (Exception ex)
            {
                logger.Error("Problemas no método AddQtdyCustodiaMemoryBmf: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Método para recalcular a posição de custódia com o novo preço do ativo que acabou de chegar.
        /// </summary>
        /// <param name="inst">Informações sobre o instrumento , incluindo o ultimo preço</param>
        public void CalcularTotalCustodia(SymbolInfo inst)
        {
            try
            {
                
                List<int> lstAccounts = new List<int>();
                //List<CustodiaInfo> lstBvsp = _cdBvsp.Where(x => x.Key.IndexOf(inst.Instrumento) >= 0).Select(x => x.Value).ToList();

                List<CustodiaInfo> lstBvsp = null;
                if (_cdBvsp.TryGetValue(inst.Instrumento, out lstBvsp))
                {
                    foreach (CustodiaInfo x in lstBvsp)
                    {
                        if (inst.LotePadrao > 100)
                        {
                            x.ValorCustodia = ((x.Qty * inst.VlrUltima )/inst.LotePadrao);
                        }
                        else
                        {
                            x.ValorCustodia = x.Qty * inst.VlrUltima;
                        }

                        lstAccounts.Add(x.Account);
                    }
                }

                // Buscar todas as chaves bmf que tenham a chave
                List<CustodiaInfo> lstBmf = null;
                //List<CustodiaInfo> lstBmf = _cdBmf.Where(x => x.Key.IndexOf(inst.Instrumento) >= 0).Select(x => x.Value).ToList();
                if (_cdBmf.TryGetValue(inst.Instrumento, out lstBmf))
                {
                    
                    foreach (CustodiaInfo y in lstBmf)
                    {
                        decimal aux = BmfCalculator.Instance.CalcularCustodiaAberturaBmf(inst, y.Qty);
                        y.ValorCustodia = aux;
                        lstAccounts.Add(y.Account);
                    }
                }
                foreach (int item in lstAccounts)
                {
                    // Buscar tesouro direto e recalcular o RiscoConsolidado
                    
                    ConsolidatedRiskInfo cr = null;

                    if (_cdRiscoConsolidado.TryGetValue(item, out cr))
                    {
                        List<CustodiaInfo> lst1 = null;
                        
                        decimal vlrCustodiaBov = decimal.Zero;
                        
                        decimal vlrCustodiaBmf = decimal.Zero;
                        
                        decimal vlrCustodiaAbTedi = decimal.Zero;
                        
                        if (_cdBvspByAcc.TryGetValue(item, out lst1))
                            vlrCustodiaBov = lst1.Sum(x => x.ValorCustodia);

                        List<CustodiaInfo> lst2 = null;
                        
                        if (_cdBmfByAcc.TryGetValue(item, out lst2))
                            vlrCustodiaBmf = lst2.Sum(x => x.ValorCustodia);

                        TesouroDiretoAbInfo tD = null;
                        
                        vlrCustodiaAbTedi = decimal.Zero;
                        
                        if (_cdTedi.TryGetValue(item, out tD))
                            vlrCustodiaAbTedi = tD.ValPosi;
                        
                        cr.TotalCustodiaBmf = vlrCustodiaBmf;
                        
                        cr.TotalCustodiaBvsp = vlrCustodiaBov;
                        
                        cr.TotalCustodiaTesouroDireto = vlrCustodiaAbTedi;
                        //cr.TotalCustodiaAbertura = cr.TotalCustodiaBmf + cr.TotalCustodiaBvsp + cr.TotalCustodiaTesouroDireto;
                        cr.TotalCustodiaAbertura = cr.TotalCustodiaBmf + cr.TotalCustodiaBvsp ;

                        //logger.InfoFormat("Total custodia abertura [{0}]", cr.TotalCustodiaAbertura);

                        decimal lTotalCustodiacomBtc = cr.TotalCustodiaAbertura - cr.TotalBtcTomador - cr.TotalGarantiaARemoverCustodia;

                        cr.SaldoTotalAbertura = lTotalCustodiacomBtc + cr.TotalGarantias + cr.TotalContaCorrenteOnline + cr.TotalProdutos;
                        
                        cr.SFP = cr.SaldoTotalAbertura + cr.PLTotal;

                        decimal lSaldoTotalSemCustodia = cr.TotalContaCorrenteOnline + cr.TotalGarantias + cr.TotalProdutos;

                        
                        if ((cr.PLTotal + lTotalCustodiacomBtc + lSaldoTotalSemCustodia) < 0)
                        {
                            cr.TotalPercentualAtingido = ((cr.PLTotal + lTotalCustodiacomBtc) + lSaldoTotalSemCustodia)-100;
                        }
                        
                        if (cr.SaldoTotalAbertura.Equals(decimal.Zero))
                        {
                            cr.TotalPercentualAtingido = cr.PLTotal;
                        }

                        if (cr.SaldoTotalAbertura > 0)
                        {
                            if (cr.TotalCustodiaAbertura < 0)
                            {
                                cr.TotalPercentualAtingido = ((cr.PLTotal + lTotalCustodiacomBtc) / lSaldoTotalSemCustodia) * 100;
                            }
                            else
                            {
                                cr.TotalPercentualAtingido = (cr.PLTotal / cr.SaldoTotalAbertura) * 100;
                            }
                        }
                        else
                        {
                            if (cr.TotalCustodiaAbertura < 0)
                            {
                                cr.TotalPercentualAtingido = ((cr.PLTotal + lTotalCustodiacomBtc) / (lSaldoTotalSemCustodia==0?1: lSaldoTotalSemCustodia)) * (100);

                                if ((cr.PLTotal + lTotalCustodiacomBtc) < 0 && lSaldoTotalSemCustodia < 0)
                                {
                                    cr.TotalPercentualAtingido = ((cr.PLTotal + lTotalCustodiacomBtc) / ((lSaldoTotalSemCustodia == 0) ? 1 : lSaldoTotalSemCustodia)) * (-100);
                                }
                            }
                            else
                            {
                                cr.TotalPercentualAtingido = ((cr.PLTotal + lTotalCustodiacomBtc) / (cr.SaldoTotalAbertura == 0 ? 1 : cr.SaldoTotalAbertura) ) * (-100);

                                if ((cr.PLTotal + lTotalCustodiacomBtc) >= 0 && cr.SaldoTotalAbertura == 0)
                                {
                                    cr.TotalPercentualAtingido = (cr.PLTotal + lTotalCustodiacomBtc);
                                }
                            }
                        }
                        /*}*/

                        //logger.InfoFormat("Chegou Cotação!!!Recalculo de Percentual Atigido do cliente [{0}] foi [{1}]", cr.Account, cr.TotalPercentualAtingido.ToString("N2"));

                        cr.DtMovimento = DateTime.Now;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Problemas no calculo de Total de Custodia: " + ex.Message, ex);
            }
        }

        private void _processConsolidatedRiskDB()
        {
            int i = 0;
            int mdsRefresh = 10;
            int sleep = 10;
            if (ConfigurationManager.AppSettings.AllKeys.Contains("PositionClientDBRefresh"))
                mdsRefresh = Convert.ToInt32(ConfigurationManager.AppSettings["PositionClientDBRefresh"].ToString());
            while (_isRunning)
            {
                try
                {
                    if (i >= mdsRefresh * sleep)
                    {
                        if (_isRunningDB)
                            break;
                        _isRunningDB = true;
                        this._atualizaConsolidatedRiskDB();
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

        private void _atualizaConsolidatedRiskDB()
        {
            try
            {
                List<PosClientSymbolInfo> lstAux = new List<PosClientSymbolInfo>();

                if (_dbRisco == null)
                    _dbRisco = new DbRisco();

               logger.Info("======> Atualizando registros Risco Consolidado");
                foreach (KeyValuePair<int, ConsolidatedRiskInfo> item in _cdRiscoConsolidado)
                {
                    if (!_dbRisco.AtualizarRiscoConsolidado(item.Value))
                        logger.ErrorFormat("Problemas na atualizacao/ insercao de Risco Consolidado : [{0}]", item.Value.Account);
                }
                logger.Info("======> Registros atualizados (risco consolidado): " + _cdRiscoConsolidado.Count);
                
                lstAux.Clear();
                lstAux = null;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na atualizacao das informacoes de cotacao: " + ex.Message, ex);
            }
        }

        public ConcurrentDictionary<int, ConsolidatedRiskInfo> SnapshotConsolidatedRisk()
        {
            ConcurrentDictionary<int, ConsolidatedRiskInfo> ret = new ConcurrentDictionary<int, ConsolidatedRiskInfo>();
            try
            {
                lock (_syncSnap)
                {
                    if (null != _cdRiscoConsolidado)
                    {
                        KeyValuePair<int, ConsolidatedRiskInfo>[] items = _cdRiscoConsolidado.ToArray();

                        foreach (KeyValuePair<int, ConsolidatedRiskInfo> item in items)
                        {
                            ret.AddOrUpdate(item.Key, item.Value, (key, oldValue) => item.Value);
                        }
                    }
                    else
                    {
                        logger.Info("Risco Consolidado sem registros...");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na montagem do snapshot de risco consolidado: " + ex.Message, ex);
            }
            return ret;
        }
    }
}
