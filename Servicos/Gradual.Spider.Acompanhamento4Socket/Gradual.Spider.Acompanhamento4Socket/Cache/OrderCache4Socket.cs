using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;

using log4net;
using Gradual.Spider.Acompanhamento4Socket.Db;
using Gradual.Spider.Acompanhamento4Socket.Lib.Dados;
using Gradual.Core.Spider.OrderFixProcessing.Lib.Dados;
using Gradual.Spider.Acompanhamento4Socket.Rede;
using Gradual.Spider.Acompanhamento4Socket.Lib.Util;
using Gradual.Spider.Acompanhamento4Socket;
using Gradual.Core.Spider.OrderFixProcessing.Lib.Util;
using Gradual.OMS.RoteadorOrdens.Lib.Dados;
using System.Configuration;
using Gradual.Spider.CommSocket;
using System.Globalization;

namespace Gradual.Spider.Acompanhamento4Socket.Cache
{
    public class OrderTimestampInfo
    {
        public int Account { get; set; }
        public int OrderID { get; set; }
        public long LastTick { get; set; }

        public OrderTimestampInfo(int account, int orderid)
        {
            this.Account = account;
            this.OrderID = orderid;
            LastTick = DateTime.Now.Ticks;
        }

        public string Key
        {
            get { return string.Format("{0}|{1}", Account, OrderID); }
        }
    }

    public class OrderCache4Socket
    {
        #region log4net declaration
        public static readonly log4net.ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region Private Variables
        private static OrderCache4Socket _me = null;
        private static object sync_ = new object();

        private DbAc4Socket _db = null;
        bool _isRunning;
        bool _isConnected;
        bool _isProcessingSnap;
        bool _isSnapshotLoaded;
        // Thread _thReconnect = null;
        Thread _thLoadSnapShot = null;
        Thread _thProcessQueue = null;
        ConcurrentQueue<TOOrderFixInfo> _cqMsg = null;
        ConcurrentDictionary<int, ConcurrentDictionary<int, SpiderOrderInfo>> _orders;
        // ConcurrentDictionary<int, List<SpiderOrderDetailInfo>> _orderdetails;
        ConcurrentDictionary<int, ConcurrentDictionary<int, SpiderOrderDetailInfo>> _orderdetails;
        Dictionary<int, SpiderIntegration> _dicIntegration;
        int _maxDetailCount = 0;

        // Variaveis do controle de envio 
        private ConcurrentDictionary<string, OrderTimestampInfo> dctTimestampOrdens = new ConcurrentDictionary<string, OrderTimestampInfo>();
        private long OrderInterval { get; set; }
        private long OrderIntervaloNaoEnviados { get; set; }
        private DateTime DataHoraInicioPregao;
        private DateTime DataHoraFinalPregao;
        #endregion

        public bool SnapshotLoaded { get { return _isSnapshotLoaded; } }

        public static OrderCache4Socket GetInstance()
        {
            lock (sync_)
            {
                if (_me == null)
                {
                    _me = new OrderCache4Socket();
                }
            }
            return _me;
        }

        public OrderCache4Socket()
        {
            _db = new DbAc4Socket();
            _cqMsg = new ConcurrentQueue<TOOrderFixInfo>();
            _isRunning = false;
            _isConnected = false;
            _isSnapshotLoaded = false;
            _isProcessingSnap = false;
            _orders = new ConcurrentDictionary<int, ConcurrentDictionary<int, SpiderOrderInfo>>();
            _orderdetails = new ConcurrentDictionary<int, ConcurrentDictionary<int, SpiderOrderDetailInfo>>();
            _dicIntegration = new Dictionary<int, SpiderIntegration>();

            if (ConfigurationManager.AppSettings.AllKeys.Contains("MaxDetailCount"))
            {
                _maxDetailCount = Convert.ToInt32(ConfigurationManager.AppSettings["MaxDetailCount"]);
            }
            else
                _maxDetailCount = 0;

            OrderInterval = 500;
            OrderIntervaloNaoEnviados = 2000;
            if (ConfigurationManager.AppSettings["OrderInterval"] != null)
            {
                OrderInterval = Convert.ToInt64(ConfigurationManager.AppSettings["OrderInterval"].ToString());
            }

            if (ConfigurationManager.AppSettings["OrderIntervaloNaoEnviados"] != null)
            {
                OrderIntervaloNaoEnviados = Convert.ToInt64(ConfigurationManager.AppSettings["OrderIntervaloNaoEnviados"].ToString());
            }

            string horainicio = "1000";
            if (ConfigurationManager.AppSettings["HorarioInicioPregao"] != null)
            {
                horainicio = ConfigurationManager.AppSettings["HorarioInicioPregao"].ToString();
            }

            DataHoraInicioPregao = DateTime.ParseExact(DateTime.Now.ToString("yyyyMMdd") + horainicio, "yyyyMMddHHmm", CultureInfo.InvariantCulture);

            string horafinal = "1715";
            if (ConfigurationManager.AppSettings["HorarioFinalPregao"] != null)
            {
                horafinal = ConfigurationManager.AppSettings["HorarioFinalPregao"].ToString();
            }

            DataHoraFinalPregao = DateTime.ParseExact(DateTime.Now.ToString("yyyyMMdd") + horafinal, "yyyyMMddHHmm", CultureInfo.InvariantCulture);

            logger.InfoFormat("Inicio Pregao [{0}] Final [{1}]", DataHoraInicioPregao.ToString("yyyy/MM/dd HH:mm"),
                DataHoraFinalPregao.ToString("yyyy/MM/dd HH:mm"));
        }

        public void Start()
        {
            try
            {
                logger.Info("Iniciando OrderCache 4 Socket ...");

                logger.Info("Carregando SpiderIntegration Info");
                this._loadIntegrationName();

                _isRunning = true;

                logger.Info("Iniciando Thread ProcessQueue...");
                _thProcessQueue = new Thread(new ThreadStart(_dequeueProcess));
                _thProcessQueue.Start();

                logger.Info("Iniciando Thread LoadSnapShot...");
                _thLoadSnapShot = new Thread(new ThreadStart(_loadSnapshotProcess));
                _thLoadSnapShot.Start();
            }
            catch (Exception ex)
            {
                logger.Error("Problemas no start do Order Cache 4 Socket: " + ex.Message, ex);
                throw;
            }
        }

        public void Stop()
        {
            try
            {
                logger.Info("Parando OrderCache 4 Socket... ");
                _isRunning = false;
                logger.Info("Parando Thread LoadSnapShot...");
                if (null != _thLoadSnapShot)
                {
                    if (_thLoadSnapShot.IsAlive) _thLoadSnapShot.Join(500);
                    if (_thLoadSnapShot.IsAlive) _thLoadSnapShot.Abort();
                    _thLoadSnapShot = null;
                }
                logger.Info("Parando Thread ProcessQueue...");
                if (null != _thProcessQueue)
                {
                    if (_thProcessQueue.IsAlive) _thProcessQueue.Join(500);
                    if (_thProcessQueue.IsAlive) _thProcessQueue.Abort();
                    _thProcessQueue = null;
                }

            }
            catch (Exception ex)
            {
                logger.Error("Problemas no stop do Order Cache 4 Socket: " + ex.Message, ex);
                throw;
            }
        }

        public void Connected(bool valor)
        {
            _isConnected = valor;
            if (false == valor)
            {
                _isProcessingSnap = false;
                _isSnapshotLoaded = false;
            }
        }

        

        #region Thread Controls
        private void _loadSnapshotProcess()
        {
            try
            {
                int i = 0;
                while (_isRunning)
                {
                    i++;
                    if (i >= 150)
                    {
                        if (_isConnected && !_isProcessingSnap)
                        {
                            _isProcessingSnap = true;
                            logger.Info("Iniciando montagem do snapshot...");
                            int maxSnapshot = 10;
                            int x = 0;
                            if (ConfigurationManager.AppSettings.AllKeys.Contains("MaxSnapshotTries"))
                                maxSnapshot = Convert.ToInt32(ConfigurationManager.AppSettings["MaxSnapshotTries"]);

                            while (x < maxSnapshot)
                            {
                                x++;
                                logger.Info("Tentativa de montagem de snapshot: " + x);
                                bool ret = _db.LoadOrderSnapshot(DateTime.Now, out _orders, out _orderdetails);
                                // Efetuar sincronizacao com informacoes existentes na fila
                                logger.Info("Order Count: " + _orders.Count);
                                logger.Info("OrderDetail Count: " + _orderdetails.Count);
                                _isSnapshotLoaded = _syncSnapshotOrders(_orders);

                                if (ret && _isSnapshotLoaded)
                                    break;
                            }
                            if (x >= maxSnapshot)
                                throw new Exception("Problemas na geracao do snapshot");

                            logger.Info("Fim montagem do snapshot");
                            if (_orders == null)
                            {
                                logger.ErrorFormat("Erro na carga do Snapshot!!!");
                                _isProcessingSnap = false;
                            }
                        }
                    }
                    Thread.Sleep(200);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Problemas no processamento da thread de carga de snapshot: " + ex.Message, ex);
                throw;
            }
        }

        private bool _syncSnapshotOrders(   ConcurrentDictionary<int, ConcurrentDictionary<int, SpiderOrderInfo>> orders) 
                                            // ConcurrentDictionary<int, ConcurrentDictionary<int, SpiderOrderDetailInfo>> orderdetails) 
        {
            bool ret = false;
            try
            {
                logger.Info("Sincronizando o acompanhamento de ordens...");
                
                // Buscar o maior MsgSeqNum
                long maxSeqNum = 0;
                foreach (KeyValuePair<int, ConcurrentDictionary<int, SpiderOrderInfo>> item in orders)
                {
                    foreach (KeyValuePair<int, SpiderOrderInfo> item2 in item.Value)
                    {
                        if (item2.Value.MsgSeqNum >= maxSeqNum)
                            maxSeqNum = item2.Value.MsgSeqNum;
                    }
                }

                logger.Info("MaxMsgSeqNum encontrado nas ordens: " + maxSeqNum);

                // Descartar os itens que estiverem na fila
                long currentSeqNum = 0;
                while(true)
                {
                    // Se fila vazia, cai fora do laco
                    if (_cqMsg.Count == 0)
                    {
                        logger.Info("Fila zerada. Sincronizado!!!!!");
                        ret = true;
                        break;
                    }
                    TOOrderFixInfo to = null;
                    _cqMsg.TryPeek(out to);
                    if (to!=null)
                    {
                        currentSeqNum = to.OrderFixInfo.MsgSeqNum;
                        if (currentSeqNum - maxSeqNum > 1)
                        {
                            // TODO[FF]: Verificar como fazer o resnapshot
                            logger.ErrorFormat("Perda de mensagem. Necessario refazer o snapshot: Max[{0}] Current[{1}]", maxSeqNum, currentSeqNum);
                            ret = false;
                            break;
                        }
                        // Verificar perda de msg
                        if (currentSeqNum > maxSeqNum)
                        {
                            logger.InfoFormat("Nao precisa descartar mais nada... Sincronizado...Current:[{0}] MaxSnapshot[{1}]",
                                currentSeqNum, maxSeqNum);
                            break;
                        }

                        if (currentSeqNum  == maxSeqNum)
                        {
                            logger.InfoFormat("Ok... sincronizado: Current[{0}] MaxSnapshot[{1}]", currentSeqNum, maxSeqNum);
                            _cqMsg.TryDequeue(out to);
                            ret = true;
                            break;
                        }
                        if (currentSeqNum <= maxSeqNum)
                        {
                            logger.InfoFormat("Descartando OrderID[{0}] SeqNum[{1}]", to.OrderFixInfo.OrderID, to.OrderFixInfo.MsgSeqNum);
                            _cqMsg.TryDequeue(out to);
                            continue;
                        }
                    }
                }

                return ret;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na sincronizacao do snapshot com o canal incremental: " + ex.Message, ex);
                return false;
            }
        }

        public void EnqueueOrderInfo(TOOrderFixInfo to)
        {
            try
            {
                _cqMsg.Enqueue(to);
            }
            catch (Exception ex)
            {
                logger.Error("Problemas no enfileiramneto da mensagem: " + ex.Message,ex);
            }
        }

        private void _dequeueProcess()
        {
            long lastlog = 0;
            long lastAlert = 0;
            long lastPending = 0;
            try
            {
                while (_isRunning)
                {
                    TOOrderFixInfo item = null;
                    if ( _isSnapshotLoaded)
                    {
                        // Processa os acompanhamentos enfileirados
                        TimeSpan ts;
                        if (_cqMsg.TryDequeue(out item))
                        {
                            if (item != null)
                                this._processMsg(item);

                            ts = new TimeSpan(DateTime.Now.Ticks - lastlog);
                            if (ts.TotalMilliseconds > 2000)
                            {
                                logger.Info("Fila de mensagens a processar: " + _cqMsg.Count);
                                lastlog = DateTime.Now.Ticks;
                            }

                            ts = new TimeSpan(DateTime.Now.Ticks - lastAlert);
                            if (ts.TotalMilliseconds > 30000)
                            {
                                if (_cqMsg.Count > 2000)
                                {
                                    string msg = "FILA DE MENSAGENS COM ENFILEIRAMENTO EXCESSIVO: " + _cqMsg.Count;
                                    logger.Warn(msg);
                                    Gradual.OMS.Library.Utilities.EnviarEmail("Alerta Spider_Acompanhamento4Socket: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), msg);
                                }

                                lastAlert = DateTime.Now.Ticks;
                            }

                            continue;
                        }

                        // Processa os acompanhamentos pendentes de envio
                        ts = new TimeSpan(DateTime.Now.Ticks - lastPending);
                        if (ts.TotalMilliseconds > 10000)
                        {
                            logger.Info("Obtendo lista dos acompanhamento pendentes a enviar");

                            lastPending = DateTime.Now.Ticks;
                            List<OrderTimestampInfo> lstNotSent = this.ObterAcompanhamentosNaoEnviados();

                            if (lstNotSent.Count > 0)
                            {

                                logger.Info("Lista com " + lstNotSent.Count + " acompanhamentos com sinal HB pendente. Enviando...");

                                foreach (OrderTimestampInfo info in lstNotSent)
                                {
                                    if (_orders.ContainsKey(info.Account))
                                    {
                                        ConcurrentDictionary<int, SpiderOrderInfo> ordems = _orders[info.Account];
                                        if (ordems.ContainsKey(info.OrderID))
                                        {
                                            SpiderOrderInfo itemOrder = ordems[info.OrderID];

                                            logger.InfoFormat("Enviando ultima posicao de Acount [{0}] Symbol [{1}] OrderID [{2}] previamente represada",
                                                info.Account,
                                                itemOrder.Symbol,
                                                info.OrderID);

                                            SpiderOrderInfo aux = CloneSpiderOrderInfo.CopyOrderInfo(itemOrder);

                                            A4SocketSrv.GetInstance().SendAcOrdem(aux);
                                            A4SocketSrv.GetInstance().SendStreamerOrder(aux);

                                            OrderTimestampInfo ouch = null;
                                            if (itemOrder.OrdStatus == 2 || itemOrder.OrdStatus == 4)
                                                dctTimestampOrdens.TryRemove(info.Key, out ouch);
                                        }
                                    }
                                }

                                logger.Info("Enviado sinal de " + lstNotSent.Count + " acompanhamentos. Done.");
                            }
                            else
                            {
                                logger.Info("Nao ha acompanhamentos pendentes de envio");
                            }
                        }
                    }

                    Thread.Sleep(100);
                }

            }
            catch (Exception ex)
            {
                logger.Error("Problemas no processamento da thread de carga de snapshot: " + ex.Message, ex);
            }
        }

        private void _processMsg(TOOrderFixInfo to)
        {
            try
            {
                int ordId = 0;
                bool notSend = false;
                // Validar se existe registro (nao devia cair nesta situacao)
                if (null == to.OrderFixInfo)
                {
                    logger.Error("SpiderOrd NULL!! Nao deveria cair nesta situacao!!!!!!");
                    return;
                }
                
                // Verifica se existe o account no dicionario
                ConcurrentDictionary<int, SpiderOrderInfo> item = null;
                if (!_orders.TryGetValue(to.OrderFixInfo.Account, out item))
                {
                    item = new ConcurrentDictionary<int, SpiderOrderInfo>();
                    _orders.AddOrUpdate(to.OrderFixInfo.Account, item, (key, oldValue) => item);
                }
                // Verificar se existe o orderID
                SpiderOrderInfo itemOrder = null;
                SpiderOrderInfo aux = null;
                if (!item.TryGetValue(to.OrderFixInfo.OrderID, out itemOrder))
                {
                    itemOrder = new SpiderOrderInfo();
                    item.AddOrUpdate(to.OrderFixInfo.OrderID, itemOrder, (key, oldValue) => itemOrder);
                }

                itemOrder.MsgSeqNum = to.OrderFixInfo.MsgSeqNum;
                // Efetuar atribuicao do SpiderOrder, propriedade a propriedade
                // para evitar de sobrescrever a lista de SpiderOrderDetails;
                if (string.IsNullOrEmpty(to.OrderFixInfo.HandlInst))
                    logger.Error("HandlInst em branco: " + to.OrderFixInfo.OrderID);

                this._spiderOrderUpdate(itemOrder, to.OrderFixInfo);
                // itemOrder = CloneSpiderOrderInfo.CopyOrderInfo(to.OrderFixInfo);
                //if (itemOrder.OrdStatus != to.OrderFixInfo.OrdStatus)
                //{
                //    logger.Info("Problema aqui!!!!");
                //}
                string ax = string.Empty;
                ordId = to.OrderFixInfo.OrderID;
                ConcurrentDictionary<int, SpiderOrderDetailInfo> cdAux  = null;
                if (!_orderdetails.TryGetValue(ordId, out cdAux))
                {
                    cdAux = new ConcurrentDictionary<int, SpiderOrderDetailInfo>();
                    ax = "Order id nao existe, mas OrderfixDetail Null";
                    if (null != to.OrderFixDetail)
                        cdAux.AddOrUpdate(to.OrderFixDetail.OrderDetailID, to.OrderFixDetail, (key, old) => to.OrderFixDetail);
                    _orderdetails.AddOrUpdate(ordId, cdAux, (key, oldValue) => cdAux);
                }
                else
                {
                    if (null != to.OrderFixDetail)
                    {
                        SpiderOrderDetailInfo xpto;
                        if (!cdAux.TryGetValue(to.OrderFixDetail.OrderDetailID, out xpto))
                            cdAux.AddOrUpdate(to.OrderFixDetail.OrderDetailID, to.OrderFixDetail, (key, old) => to.OrderFixDetail);
                    }
                    ax = "Order id existe, mas orderfixdetail null ou ja existe na lista";                
                }

                if (cdAux!=null && cdAux.Count == 0)
                {
                    if (to.OrderFixInfo.OrdStatus == 1 || to.OrderFixInfo.OrdStatus == 2)
                        logger.InfoFormat("LST AUX NULL ou ZERADO com STATUS PARC.EXEC ou EXEC: {0} OrderID[{1}]", ax, to.OrderFixInfo.OrderID);
                    else
                        logger.InfoFormat("LST AUX NULL ou ZERADO com STATUS PARC.EXEC ou EXEC: {0}", ax);
                }
                
                lock (itemOrder.Details)
                {
                    itemOrder.Details.Clear();
                    itemOrder.Details.AddRange(cdAux.Values);
                }
                if (null != to.OrderFixDetail) // calcular somente se for nulo e se o status for partial fill ou fill
                {
                    if (to.OrderFixDetail.OrderStatusID == (int)SpiderOrderStatus.PARCIALMENTEEXECUTADA ||
                        to.OrderFixDetail.OrderStatusID == (int)SpiderOrderStatus.EXECUTADA)
                    {
                        decimal avgPxW;
                        decimal avgPx;

                        Calculator.CalculateAllPxAverages(cdAux.Values.ToList(), out avgPx, out avgPxW);

                        itemOrder.AvgPxW = avgPxW;
                        itemOrder.AvgPx = avgPx;
                    }
                    if (_maxDetailCount > 0)
                    {
                        if (to.OrderFixDetail.OrderStatusID == (int)SpiderOrderStatus.ENVIADA_PARA_O_ROTEADOR ||
                            to.OrderFixDetail.OrderStatusID == (int)SpiderOrderStatus.ENVIADA_PARA_A_BOLSA ||
                            to.OrderFixDetail.OrderStatusID == (int)SpiderOrderStatus.ENVIADA_PARA_O_CANAL)
                            notSend = true;
                    }
                }
                aux = CloneSpiderOrderInfo.CopyOrderInfo(itemOrder);
                if (logger.IsDebugEnabled)
                {
                    bool abc = to.OrderFixDetail == null ? true : false;
                    logger.DebugFormat("Processando OrderID==>AUX[{0}] MsgSeqNum[{1}] Detail: [{2}] Exchange [{3}] Account[{4}] Symbol[{5}] DateTime[{6}] Status[{7}] Side [{8}] OrderQty[{9}] OrderQtyRemaining [{10}] CumQty[{11}] IsDetailNull[{12}]",  
                        aux.OrderID, aux.MsgSeqNum, aux.Details.Count, aux.Exchange, aux.Account, aux.Symbol, aux.TransactTime, aux.OrdStatus, aux.Side, aux.OrderQty, aux.OrderQtyRemaining, aux.CumQty, abc);
                }
                
                if (!notSend)
                {
                    if (ShouldSendAcompanhamento(aux.Account, aux.OrderID))
                    {
                        A4SocketSrv.GetInstance().SendAcOrdem(aux);
                        A4SocketSrv.GetInstance().SendStreamerOrder(aux);
                    }
                    else
                        logger.DebugFormat("Acompanhamento OrderID[{0}] MsgSeqNum[{1}] Detail: [{2}] Exchange [{3}] Account[{4}] Symbol[{5}] DateTime[{6}] Status[{7}] Side [{8}] represado",
                            aux.OrderID, aux.MsgSeqNum, aux.Details.Count, aux.Exchange, aux.Account, aux.Symbol, aux.TransactTime, aux.OrdStatus, aux.Side);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Problemas no processamento da mensagem: " + ex.Message, ex);
            }
            
        }

        public void ProcessCancelRejection(OrdemInfo ord)
        {
            try
            {
                // Validar se existe registro (nao devia cair nesta situacao)
                if (null == ord)
                {
                    logger.Error("OrdemInfo NULL!! Nao deveria cair nesta situacao!!!!!!");
                    return;
                }

                // Buscar a ordem a partir do ExchangeNumberID
                List<SpiderOrderInfo> aux = new List<SpiderOrderInfo>();
                foreach (KeyValuePair<int, ConcurrentDictionary<int, SpiderOrderInfo>> item in _orders)
                {
                    aux.AddRange(item.Value.Values.ToList());
                }
                SpiderOrderInfo spiderOrd = aux.FirstOrDefault(x => x.ExchangeNumberID == ord.ExchangeNumberID);
                if (null == spiderOrd)
                {
                    // Se nao encontrado tentar pelo ClOrdID que na verdade eh o OrigClOrdID do ord
                    spiderOrd = aux.FirstOrDefault(x => x.ClOrdID == ord.OrigClOrdID);
                }
                if (null == spiderOrd)
                {
                    logger.ErrorFormat("Order nao encontrada na memoria do Ac4Socket: ExchangeNumber[{0}] ClOrdID[{1}]", 
                        ord.ExchangeNumberID, ord.OrigClOrdID);
                    return;
                }
                logger.InfoFormat("Requisicao de cancelamento da ordem rejeitada. OrderID: [{0}] ExchangeNumberID: [{1}] ", 
                    spiderOrd.OrderID, spiderOrd.ExchangeNumberID);
                
                // Criar o order detail de rejeicao de cancelamento e atualizar a memoria
                // List<SpiderOrderDetailInfo> lstDetail = null;
                ConcurrentDictionary<int, SpiderOrderDetailInfo> cdDetail = null;
                if (!_orderdetails.TryGetValue(spiderOrd.OrderID, out cdDetail))
                {
                    logger.ErrorFormat("SpiderOrderDetails nao encontrado: [{0}] ExchangeNumberID: [{1}] ", 
                        spiderOrd.OrderID, spiderOrd.ExchangeNumberID);
                    return;
                }
                // Criar o SpiderOrderDetail
                SpiderOrderDetailInfo rejDet = new SpiderOrderDetailInfo();
                rejDet.OrderID = spiderOrd.OrderID;
                rejDet.OrderQty = spiderOrd.OrderQty;
                rejDet.OrderQtyRemaining = spiderOrd.OrderQtyRemaining;
                rejDet.Price = spiderOrd.Price;
                rejDet.StopPx = spiderOrd.StopPx;
                rejDet.OrderStatusID = 8; // Rejected
                rejDet.TransactTime = DateTime.Now;
                rejDet.Description = "Requisicao de cancelamento rejeitada";
                rejDet.TradeQty = 0;
                rejDet.CumQty = spiderOrd.CumQty;

                // Efetuar a gravacao no banco de dados
                bool ret = _db.InserirOrdemDetalhe(rejDet, string.Empty);
                if (!ret)
                {
                    logger.ErrorFormat("Nao inseriu o SpiderOrderDetail no banco. Mesmo assim nao retornara erro e tentara compor a memoria. " +
                                        "OrderID: [{0}] ClOrdID: [{1}] ExchangeNumberID: [{2}]",
                                        spiderOrd.OrderID, spiderOrd.ClOrdID, spiderOrd.ExchangeNumberID);
                }
                cdDetail.AddOrUpdate(rejDet.OrderDetailID, rejDet, (key, old) => rejDet);
                

                SpiderOrderInfo obj = null;
                lock (spiderOrd)
                {
                    spiderOrd.Details = null;
                    spiderOrd.Details = new List<SpiderOrderDetailInfo>(cdDetail.Values);
                }
                obj = CloneSpiderOrderInfo.CopyOrderInfo(spiderOrd);
                
                // Sinalizar o envio para os clients
                A4SocketSrv.GetInstance().SendAcOrdem(obj);
                A4SocketSrv.GetInstance().SendStreamerOrder(obj);
            }
            catch (Exception ex)
            {
                logger.Error("Problemas no processamento da mensagem: " + ex.Message, ex);
            }

        }

        #endregion


        #region "Filter Methods"
        public List<SpiderOrderInfo> GetOrders(FilterSpiderOrder flt, int maxLength, bool getDetails, out ExecResp ret)
        {
            ret = null;
            List<SpiderOrderInfo> lstRet = new List<SpiderOrderInfo>();
            try
            {
                // TODO[FF]: efetuar a montagem dos filtros
                
                // Varrer todas as contas e ir adicionando à lista
                
                if (flt.Account.Compare == TypeCompare.UNDEFINED)
                {
                    var pred = FilterAssemble.AssembleOrderFilter(flt);

                    List<SpiderOrderInfo> abcd = new List<SpiderOrderInfo>();
                    // Efetuar a carga dos valores em uma lista auxiliar
                    foreach (KeyValuePair<int, ConcurrentDictionary<int, SpiderOrderInfo>> item in _orders)
                    {
                        abcd.AddRange(item.Value.Values.ToList());
                    }
                    var query = from orders in abcd.AsQueryable().Where(pred)
                                select orders;
                    lstRet.AddRange(query.ToList().OrderByDescending(x => x.TransactTime));
                    ret = new ExecResp(MsgErrors.OK, MsgErrors.MSG_OK);
                    abcd.Clear();
                    abcd = null;
                    return lstRet;
                }
                else
                {
                    // Buscar registros a partir de um account
                    int account = flt.Account.Value;
                    ConcurrentDictionary<int, SpiderOrderInfo> ordPerAccount = null;
                    if (_orders.TryGetValue(account, out ordPerAccount))
                    {
                        var pred = FilterAssemble.AssembleOrderFilter(flt);
                        var query = from orders in ordPerAccount.Values.AsQueryable().Where(pred)
                                    select orders;
                        lstRet.AddRange(query.ToList().OrderByDescending(x=>x.TransactTime));
                        query = null;
                        if (lstRet.Count > maxLength)
                        {
                            lstRet.Clear();
                            ret = new ExecResp(MsgErrors.ERR_COUNT_MAX_EXCEEDED, MsgErrors.MSG_ERR_COUNT_MAX_EXCEEDED);
                            return lstRet;
                        }

                        // Montagem dos Details, caso flag true


                        // OK
                        ret = new ExecResp(MsgErrors.OK, MsgErrors.MSG_OK);
                        return lstRet;
                    }
                    else
                    {
                        ret = new ExecResp(MsgErrors.ERR_ACCOUNT_NOT_FOUND, MsgErrors.MSG_ERR_ACCOUNT_NOT_FOUND);
                        return lstRet;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na montagem da listagem de ordens: " + ex.Message, ex);
                ret = new ExecResp(MsgErrors.ERROR, ex.Message);
                return null;
            }
        }

        public List<SpiderOrderDetailInfo> GetOrderDetails(int orderID, out ExecResp ret, bool restrictDetails = true)
        {
            ret = new ExecResp(MsgErrors.OK, MsgErrors.MSG_OK);
            List<SpiderOrderDetailInfo> lstRet = new List<SpiderOrderDetailInfo>();
            try
            {
                ConcurrentDictionary<int, SpiderOrderDetailInfo> cdAux = null;
                if (_orderdetails.TryGetValue(orderID, out cdAux))
                {
                    // Se _maxDetailCount!=0, filtra os details
                    if (restrictDetails)
                    {
                        if (_maxDetailCount != 0)
                        {
                            List<SpiderOrderDetailInfo> aux = new List<SpiderOrderDetailInfo>(cdAux.Values.Where(x => x.OrderStatusID != 100 && x.OrderStatusID != 101 && x.OrderStatusID != 102).OrderBy(x => x.TransactTime));
                            int count = aux.Count;
                            lstRet.AddRange(aux.Skip(count - _maxDetailCount));
                        }
                        else
                            lstRet.AddRange(cdAux.Values);
                    }
                    else
                        lstRet.AddRange(cdAux.Values);
                    return lstRet;
                }
                else
                {
                    ret = new ExecResp(MsgErrors.ERR_ACCOUNT_NOT_FOUND, MsgErrors.MSG_ERR_ACCOUNT_NOT_FOUND);
                    return lstRet;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na montagem da listagem de ordens: " + ex.Message, ex);
                return null;
            }
        }
        #endregion


        #region Util Functions
        private void _spiderOrderUpdate(SpiderOrderInfo origOrd, SpiderOrderInfo newOrd)
        {
            origOrd.Account = newOrd.Account;
            origOrd.ChannelID = newOrd.ChannelID;
            origOrd.ClOrdID = newOrd.ClOrdID;
            origOrd.CumQty = newOrd.CumQty;
            origOrd.Description = newOrd.Description;
            origOrd.Exchange = newOrd.Exchange;
            origOrd.ExchangeNumberID = newOrd.ExchangeNumberID;
            origOrd.ExecBroker = newOrd.ExecBroker;
            origOrd.ExpireDate = newOrd.ExpireDate;
            origOrd.FixMsgSeqNum = newOrd.FixMsgSeqNum;
            origOrd.HandlInst = newOrd.HandlInst;
            origOrd.IdFix = newOrd.IdFix;
            origOrd.IntegrationName = newOrd.IntegrationName;
            origOrd.OrderQtyApar = newOrd.OrderQtyApar;
            origOrd.Memo = newOrd.Memo;
            origOrd.OrderQtyMin = newOrd.OrderQtyMin;
            origOrd.Msg42Base64 = newOrd.Msg42Base64;
            origOrd.MsgFix = newOrd.MsgFix;
            origOrd.OrderID = newOrd.OrderID;
            origOrd.OrderQty = newOrd.OrderQty;
            origOrd.OrderQtyRemaining = newOrd.OrderQtyRemaining;
            origOrd.OrdStatus = newOrd.OrdStatus;
            origOrd.OrdTypeID = newOrd.OrdTypeID;
            origOrd.OrigClOrdID = newOrd.OrigClOrdID;
            origOrd.Price = newOrd.Price;
            origOrd.RegisterTime = newOrd.RegisterTime;
            origOrd.SecurityExchangeID = newOrd.SecurityExchangeID;
            origOrd.SessionID = newOrd.SessionID;
            origOrd.SessionIDOriginal = newOrd.SessionIDOriginal;
            origOrd.Side = newOrd.Side;
            origOrd.StopPx = newOrd.StopPx;
            origOrd.StopStartID = newOrd.StopStartID;
            origOrd.Symbol = newOrd.Symbol;
            origOrd.SystemID = newOrd.SystemID;
            origOrd.TimeInForce = newOrd.TimeInForce;
            origOrd.TransactTime = newOrd.TransactTime;
            origOrd.AccountDv = newOrd.AccountDv;
            origOrd.Exchange = newOrd.Exchange;
            origOrd.IntegrationName = newOrd.IntegrationName;
            // Buscar o IntegrationName e Bolsa a partir do IdSessaoFix

            if (string.IsNullOrEmpty(origOrd.Exchange) || string.IsNullOrEmpty(origOrd.IntegrationName))
            {
                SpiderIntegration item = null;
                if (_dicIntegration.TryGetValue(origOrd.IdFix, out item))
                {
                    origOrd.IntegrationName = item.IntegrationName;
                    origOrd.Exchange = item.Exchange;
                }
            }
        }

        private bool _loadIntegrationName()
        {
            try
            {
                if (null == _db)
                {
                    _db = new DbAc4Socket();
                }
                _dicIntegration = _db.LoadIntegrationInfo();
                if (_dicIntegration == null)
                    return false;

                logger.Info("IntegrationInfo Count: " + _dicIntegration.Count);
                return true;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na carga dos IntegrationNames: " + ex.Message, ex);
                return false;
            }
        }

        #endregion

        #region Snapshot Methods
        public List<SpiderOrderInfo> GetMemorySnapshot()
        {
            try
            {
                List<SpiderOrderInfo> ret = new List<SpiderOrderInfo>();
                foreach ( KeyValuePair<int, ConcurrentDictionary<int, SpiderOrderInfo>> item in _orders)
                {
                    foreach (KeyValuePair<int, SpiderOrderInfo> item2 in item.Value)
                    {
                        // Buscar details do _orderdetails
                        SpiderOrderInfo aux = CloneSpiderOrderInfo.CopyOrderInfo(item2.Value);
                        aux.Details.Clear();
                        aux.IsSnapshot = true;

                        ConcurrentDictionary<int, SpiderOrderDetailInfo> cdAux;

                        _orderdetails.TryGetValue(item2.Key, out cdAux);
                        aux.Details.AddRange(cdAux.Values);
                        ret.Add(aux);
                    }
                }
                return ret.OrderBy(x=>x.MsgSeqNum).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na montagem do snapshot: " + ex.Message, ex);
                return null;
            }
        }
        #endregion

        #region Throttle de envio
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instrumento"></param>
        /// <returns></returns>
        public bool ShouldSendAcompanhamento(int account, int orderid)
        {
            OrderTimestampInfo info = null;
            string orderkey = string.Format("{0}|{1}", account, orderid);

            // Envia o sinal se for o 1o envio
            if (!dctTimestampOrdens.TryGetValue(orderkey, out info))
            {
                logger.Info("A1:" + orderkey);
                info = new OrderTimestampInfo(account, orderid);
                dctTimestampOrdens.AddOrUpdate(info.Key, info, (key, oldValue) => info);
                return true;
            }

            // Envia o sinal se nao estiver no horario de pregao
            if (DateTime.Compare(DateTime.Now, DataHoraInicioPregao) <= 0 || DateTime.Compare(DateTime.Now, DataHoraFinalPregao) >= 0)
            {
                logger.Info("A2:" + orderkey);
                return true;
            }

            // Testa se o ultimo envio esta fora do intervalo
            TimeSpan ts = new TimeSpan(DateTime.Now.Ticks - info.LastTick);
            if (ts.TotalMilliseconds >= OrderInterval)
            {
                logger.Info("A3:" + orderkey);
                dctTimestampOrdens[orderkey].LastTick = DateTime.Now.Ticks;
                return true;
            }

            // Garante mandar um sinal por minuto
            DateTime x = new DateTime(info.LastTick);
            if (!x.ToString("yyyyMMddHHmm").Equals(DateTime.Now.ToString("yyyyMMddHHmm")) )
            {
                logger.Info("A4:" + orderkey);
                dctTimestampOrdens[orderkey].LastTick = DateTime.Now.Ticks;
                return true;
            }

            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<OrderTimestampInfo> ObterAcompanhamentosNaoEnviados()
        {
            List<OrderTimestampInfo> lstNotSent = new List<OrderTimestampInfo>();

            string[] keys = dctTimestampOrdens.Keys.ToArray();

            //foreach (KeyValuePair<string, SinalTimestampInfo> entry in dctTimestampLOF)
            foreach (string key in keys)
            {
                OrderTimestampInfo entry;

                if (dctTimestampOrdens.TryGetValue(key, out entry))
                {
                    TimeSpan ts = new TimeSpan(DateTime.Now.Ticks - entry.LastTick);
                    DateTime corte = DateTime.Now.Subtract(new TimeSpan(TimeSpan.TicksPerMinute / 2));

                    logger.DebugFormat("OrderID [{0}] [{1}] LastTick [{2}]  Cut[{3}] Now[{4}] ",
                        entry.OrderID,
                        entry.Account,
                        (new DateTime(entry.LastTick)).ToString("dd/MM/yyyy HH:mm:ss.fff"),
                        corte.ToString("dd/MM/yyyy HH:mm:ss.fff"),
                        DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"));

                    if (ts.TotalMilliseconds >= this.OrderIntervaloNaoEnviados && entry.LastTick >= corte.Ticks)
                    {
                        OrderTimestampInfo newEntry = new OrderTimestampInfo(entry.Account, entry.OrderID);
                        newEntry.LastTick = DateTime.Now.Ticks;

                        lstNotSent.Add(entry);

                        //dctTimestampOrdens.TryUpdate(newEntry.Key, newEntry, entry);
                    }
                }
            }

            return lstNotSent;
        }

        #endregion //Throttle de envio
    }
}
