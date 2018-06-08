using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using log4net;
using Gradual.Spider.RiskClient.Lib.Mensagens;
using Gradual.Spider.SupervisorRisco.Lib.Dados;
using System.Text.RegularExpressions;
using Gradual.Spider.CommSocket;
using Gradual.Spider.Ordem.Lib.Dados;
using Gradual.Spider.DataSync.Lib.Mensagens;
using System.Net.Sockets;
using Gradual.Spider.DataSync.Lib;
using System.Configuration;
using System.Collections.Concurrent;
using Gradual.Spider.RiskClient.Lib.Dados;

namespace Gradual.Spider.RiskClient.Lib
{
    public class RiskClient
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static RiskClient _me = null;
        private bool bKeepRuning = false;
        private Thread thMonitor = null;

        protected ConcurrentDictionary<string, SymbolInfo> _dicSymbols = new ConcurrentDictionary<string, SymbolInfo>();
        protected ConcurrentDictionary<int, ClientParameterPermissionInfo> _dicParameters = new ConcurrentDictionary<int, ClientParameterPermissionInfo>();
        protected ConcurrentDictionary<int, FatFingerInfo> _dicFatFinger = new ConcurrentDictionary<int, FatFingerInfo>();
        //protected Dictionary<int, RiskExposureClientInfo> _dicRiskExposureClient = new Dictionary<int,RiskExposureClientInfo>();
        protected ConcurrentDictionary<SymbolKey, BlockedInstrumentInfo> _dicBlockedSymbolGlobal = new ConcurrentDictionary<SymbolKey, BlockedInstrumentInfo>();
        protected ConcurrentDictionary<SymbolKey, BlockedInstrumentInfo> _dicBlockedSymbolGroupClient = new ConcurrentDictionary<SymbolKey, BlockedInstrumentInfo>();
        protected ConcurrentDictionary<SymbolKey, BlockedInstrumentInfo> _dicBlockedSymbolClient = new ConcurrentDictionary<SymbolKey, BlockedInstrumentInfo>();
        protected ConcurrentDictionary<string, OptionBlockInfo> _dicOptionSeriesBlocked = new ConcurrentDictionary<string, OptionBlockInfo>();
        protected ConcurrentDictionary<int, List<OperatingLimitInfo>> _dicOperatingLimit = new ConcurrentDictionary<int, List<OperatingLimitInfo>>();
        protected ConcurrentDictionary<int, ClientLimitBMFInfo> _dicClientLimitBMF = new ConcurrentDictionary<int, ClientLimitBMFInfo>();
        protected ConcurrentDictionary<string, TestSymbolInfo> _dicTestSymbols = new ConcurrentDictionary<string, TestSymbolInfo>();
         
        protected Dictionary<int, FixLimitInfo> _dicLimitFix = new Dictionary<int, FixLimitInfo>();
        protected Dictionary<int, int> _dicAccBvspBmf = new Dictionary<int, int>();
        protected Dictionary<int, List<ContaBrokerInfo>> _dicContaBroker = new Dictionary<int, List<ContaBrokerInfo>>(); 
         
        private List<int> _lstIdClient = new List<int>();
        //private List<RiskExposureGlobalInfo> _lstRiskExposureGlobal = new List<RiskExposureGlobalInfo>();

        private Dictionary<TipoLimiteEnum, TipoLimiteEnum> _dicDualTipoLimite = new Dictionary<TipoLimiteEnum,TipoLimiteEnum>();

        // Novos Limites / Restricoes
        protected ConcurrentDictionary<string, RestrictionSymbolInfo> _dicRestrSymbol = new ConcurrentDictionary<string, RestrictionSymbolInfo>();
        protected ConcurrentDictionary<int, RestrictionGlobalInfo> _dicRestrGlobal = new ConcurrentDictionary<int, RestrictionGlobalInfo>();
        protected ConcurrentDictionary<string, RestrictionGroupSymbolInfo> _dicRestrGroupSymbol = new ConcurrentDictionary<string, RestrictionGroupSymbolInfo>();

        protected ConcurrentDictionary<int, List<PosClientSymbolInfo>> _dicPositionClient = new ConcurrentDictionary<int, List<PosClientSymbolInfo>>();

        protected ConcurrentDictionary<int, ConsolidatedRiskInfo> _dicConsolidatedRisk = new ConcurrentDictionary<int, ConsolidatedRiskInfo>();

        private SpiderSocket clientSocket;
        private int serverPort;
        private string serverHost;

        public static RiskClient Instance
        {
            get
            {
                if (_me == null)
                {
                    _me = new RiskClient();
                }

                return _me;
            }
        }

        public RiskClient()
        {
            serverHost = "127.0.0.1";
            serverPort = 5454;

            if (ConfigurationManager.AppSettings["SupervisorRiscoIP"] != null)
                serverHost = ConfigurationManager.AppSettings["SupervisorRiscoIP"].ToString();

            if (ConfigurationManager.AppSettings["SupervisorRiscoPort"] != null )
                serverPort = Convert.ToInt32(ConfigurationManager.AppSettings["SupervisorRiscoPort"].ToString());

            // Operacoes Dual de Limites (atualizacao) - bovespa
            _dicDualTipoLimite = new Dictionary<TipoLimiteEnum, TipoLimiteEnum>();
            _dicDualTipoLimite.Add(TipoLimiteEnum.COMPRAAVISTA, TipoLimiteEnum.VENDAAVISTA);
            _dicDualTipoLimite.Add(TipoLimiteEnum.VENDAAVISTA, TipoLimiteEnum.COMPRAAVISTA);
            _dicDualTipoLimite.Add(TipoLimiteEnum.COMPRAOPCOES, TipoLimiteEnum.VENDAOPCOES);
            _dicDualTipoLimite.Add(TipoLimiteEnum.VENDAOPCOES, TipoLimiteEnum.COMPRAOPCOES);
        }


        public void StartClient(List<int> subscription = null)
        {
            bKeepRuning = true;

            clientSocket = new SpiderSocket();
            clientSocket.IpAddr = serverHost;
            clientSocket.Port = serverPort;



            if (subscription == null || subscription.Count == 0)
            {
                clientSocket.AddHandler<AccountBvspBMFSyncMsg>(new ProtoObjectReceivedHandler<AccountBvspBMFSyncMsg>(OnAccountBvspBMFSync));
                clientSocket.AddHandler<BlockedInstrumentSyncMsg>(new ProtoObjectReceivedHandler<BlockedInstrumentSyncMsg>(OnBlockedInstrumentSync));
                clientSocket.AddHandler<ClientLimitBMFSyncMsg>(new ProtoObjectReceivedHandler<ClientLimitBMFSyncMsg>(OnClientLimitBMFSync));
                clientSocket.AddHandler<ClientParameterPermissionSyncMsg>(new ProtoObjectReceivedHandler<ClientParameterPermissionSyncMsg>(OnClientParameterPermissionSync));
                clientSocket.AddHandler<FatFingerSyncMsg>(new ProtoObjectReceivedHandler<FatFingerSyncMsg>(OnFatFingerSync));
                clientSocket.AddHandler<OperatingLimitSyncMsg>(new ProtoObjectReceivedHandler<OperatingLimitSyncMsg>(OnOperatingLimitSync));
                clientSocket.AddHandler<OptionBlockSyncMsg>(new ProtoObjectReceivedHandler<OptionBlockSyncMsg>(OnOptionBlockSync));
                clientSocket.AddHandler<SymbolListSyncMsg>(new ProtoObjectReceivedHandler<SymbolListSyncMsg>(OnSymbolListSync));
                clientSocket.AddHandler<TestSymbolSyncMsg>(new ProtoObjectReceivedHandler<TestSymbolSyncMsg>(OnTestSymbolSync));
                clientSocket.AddHandler<ContaBrokerSyncMsg>(new ProtoObjectReceivedHandler<ContaBrokerSyncMsg>(OnContaBrokerSync));

                clientSocket.AddHandler<RestrictionGlobalSyncMsg>(new ProtoObjectReceivedHandler<RestrictionGlobalSyncMsg>(OnRestrictionGlobalSync));
                clientSocket.AddHandler<RestrictionGroupSymbolSyncMsg>(new ProtoObjectReceivedHandler<RestrictionGroupSymbolSyncMsg>(OnRestrictionGroupSymbolSync));
                clientSocket.AddHandler<RestrictionSymbolSyncMsg>(new ProtoObjectReceivedHandler<RestrictionSymbolSyncMsg>(OnRestrictionSymbolSync));

                clientSocket.AddHandler<PositionClientSyncMsg>(new ProtoObjectReceivedHandler<PositionClientSyncMsg>(OnPositionClientSync));

                clientSocket.AddHandler<MaxLossSyncMsg>(new ProtoObjectReceivedHandler<MaxLossSyncMsg>(OnMaxLossSync));

                clientSocket.OnUnmappedObjectReceived += new UnmappedObjectReceivedHandler(clientSocket_OnUnmappedObjectReceived);
            }
            else
            {
                // Assinaturas obrigatorias
                //clientSocket.AddHandler<AccountBvspBMFSyncMsg>(new ProtoObjectReceivedHandler<AccountBvspBMFSyncMsg>(OnAccountBvspBMFSync));
                //clientSocket.AddHandler<SymbolListSyncMsg>(new ProtoObjectReceivedHandler<SymbolListSyncMsg>(OnSymbolListSync));
                //clientSocket.AddHandler<ClientParameterPermissionSyncMsg>(new ProtoObjectReceivedHandler<ClientParameterPermissionSyncMsg>(OnClientParameterPermissionSync));

                clientSocket.OnUnmappedObjectReceived += new UnmappedObjectReceivedHandler(clientSocket_OnUnmappedObjectReceived);
                // Verificar a lista de assinaturas
                if (subscription != null)
                {
                    for (int i = 0; i < subscription.Count; i++)
                    {
                        switch (subscription[i])
                        {
                            case SubscriptionTypes.BLOCKED_INSTRUMENTS:
                                clientSocket.AddHandler<BlockedInstrumentSyncMsg>(new ProtoObjectReceivedHandler<BlockedInstrumentSyncMsg>(OnBlockedInstrumentSync)); 
                                break;
                            case SubscriptionTypes.CLIENT_LIMIT_BMF:
                                clientSocket.AddHandler<ClientLimitBMFSyncMsg>(new ProtoObjectReceivedHandler<ClientLimitBMFSyncMsg>(OnClientLimitBMFSync));
                                break;
                            case SubscriptionTypes.FAT_FINGER:
                                clientSocket.AddHandler<FatFingerSyncMsg>(new ProtoObjectReceivedHandler<FatFingerSyncMsg>(OnFatFingerSync));
                                break;
                            case SubscriptionTypes.OPERATING_LIMIT:
                                clientSocket.AddHandler<OperatingLimitSyncMsg>(new ProtoObjectReceivedHandler<OperatingLimitSyncMsg>(OnOperatingLimitSync));
                                break;
                            case SubscriptionTypes.OPTION_BLOCK:
                                clientSocket.AddHandler<OptionBlockSyncMsg>(new ProtoObjectReceivedHandler<OptionBlockSyncMsg>(OnOptionBlockSync));
                                break;
                            case SubscriptionTypes.TEST_SYMBOL_LIST:
                                clientSocket.AddHandler<TestSymbolSyncMsg>(new ProtoObjectReceivedHandler<TestSymbolSyncMsg>(OnTestSymbolSync));
                                break;
                            case SubscriptionTypes.CONTA_BROKER:
                                clientSocket.AddHandler<ContaBrokerSyncMsg>(new ProtoObjectReceivedHandler<ContaBrokerSyncMsg>(OnContaBrokerSync));
                                break;
                            case SubscriptionTypes.RESTRICTION_GLOBAL:
                                clientSocket.AddHandler<RestrictionGlobalSyncMsg>(new ProtoObjectReceivedHandler<RestrictionGlobalSyncMsg>(OnRestrictionGlobalSync));
                                break;
                            case SubscriptionTypes.RESTRICTION_GROUP:
                                clientSocket.AddHandler<RestrictionGroupSymbolSyncMsg>(new ProtoObjectReceivedHandler<RestrictionGroupSymbolSyncMsg>(OnRestrictionGroupSymbolSync));
                                break;
                            case SubscriptionTypes.RESTRICTION_SYMBOL:
                                clientSocket.AddHandler<RestrictionSymbolSyncMsg>(new ProtoObjectReceivedHandler<RestrictionSymbolSyncMsg>(OnRestrictionSymbolSync));
                                break;
                            case SubscriptionTypes.POSITION_CLIENT:
                                clientSocket.AddHandler<PositionClientSyncMsg>(new ProtoObjectReceivedHandler<PositionClientSyncMsg>(OnPositionClientSync));
                                break;
                            case SubscriptionTypes.MAX_LOSS:
                                clientSocket.AddHandler<MaxLossSyncMsg>(new ProtoObjectReceivedHandler<MaxLossSyncMsg>(OnMaxLossSync));
                                break;

                            case SubscriptionTypes.CONSOLIDATED_RISK:
                                clientSocket.AddHandler<ConsolidatedRiskSyncMsg>(new ProtoObjectReceivedHandler<ConsolidatedRiskSyncMsg>(OnConsolidatedRiskSync));
                                break;
                        }
                    }
                }
            }

            //////clientSocket.AddHandle sonda

            thMonitor = new Thread(new ThreadStart(monitorProc));
            thMonitor.Start();

        }

        void clientSocket_OnUnmappedObjectReceived(object sender, int clientNumber, Socket clientSock, Type objectType, object objeto)
        {
            
        }

        public void StopClient()
        {
            bKeepRuning = false;
            if (clientSocket != null)
            {
                clientSocket.OnUnmappedObjectReceived -= clientSocket_OnUnmappedObjectReceived;
                clientSocket.CloseSocket();
                clientSocket.Dispose();
                clientSocket = null;
            }
        }


        public void monitorProc()
        {
            long lastSonda = 0;
            while (bKeepRuning)
            {
                try
                {
                    if ( !clientSocket.IsConectado() )
                    {
                        clientSocket.OpenConnection();
                    }

                    TimeSpan ts = new TimeSpan(DateTime.Now.Ticks - lastSonda);

                    if (ts.TotalMilliseconds > 30000)
                    {
                        lastSonda = DateTime.Now.Ticks;

                        // enviar/responder sonda 
                    }

                    Thread.Sleep(250);
                }
                catch (Exception ex)
                {
                    logger.Error("monitorProc(): " + ex.Message, ex);
                }
            }
        }


        private void OnAccountBvspBMFSync(object sender, int clientNumber, Socket clientSocket, AccountBvspBMFSyncMsg args)
        {
            switch (args.SyncAction)
            {
                case SyncMsgAction.SNAPSHOT:
                    _dicAccBvspBmf.Clear();
                    _dicAccBvspBmf = args.Accounts.ToDictionary(entry => entry.Key, entry => entry.Value);
                    logger.Info("Recebeu snapshot de contas Bovespa x BMF com " + args.Accounts.Count + " itens");
                    break;
                case SyncMsgAction.INSERT:
                    foreach (KeyValuePair<int, int> item in args.Accounts)
                        _dicAccBvspBmf.Add(item.Key, item.Value);
                    break;
                case SyncMsgAction.UPDATE:
                    foreach (KeyValuePair<int, int> item in args.Accounts)
                        _dicAccBvspBmf[item.Key] = item.Value;
                    break;
                case SyncMsgAction.DELETE:
                    foreach (KeyValuePair<int, int> item in args.Accounts)
                        _dicAccBvspBmf.Remove(item.Key);
                    break;
                default:
                    logger.Error("Acao [" + args.SyncAction + "] invalida para dicionario de contas BMF x Bovespa");
                    break;
            }
        }


        private void OnBlockedInstrumentSync(object sender, int clientNumber, Socket clientSocket, BlockedInstrumentSyncMsg args)
        {
            ConcurrentDictionary<SymbolKey, BlockedInstrumentInfo> dic = null;

            if (args.BlockedInstrumentType == BlockedInstrumentMsgType.BlockedSymbolClient)
                dic = _dicBlockedSymbolClient;
            else
                if (args.BlockedInstrumentType == BlockedInstrumentMsgType.BlockedSymbolGroupClient)
                    dic = _dicBlockedSymbolGroupClient;
                else
                    dic = _dicBlockedSymbolGlobal;

            switch (args.SyncAction)
            {
                case SyncMsgAction.SNAPSHOT:
                    dic.Clear();
                    dic = args.BlockedInstruments;// .ToDictionary(entry => entry.Key, entry => entry.Value);
                    logger.Info("Recebeu snapshot de bloqueios de instrumentos com " + args.BlockedInstruments.Count + " itens [" + args.BlockedInstrumentType.ToString() + "]");
                    break;
                case SyncMsgAction.INSERT:
                case SyncMsgAction.UPDATE:
                    foreach (KeyValuePair<SymbolKey,BlockedInstrumentInfo> item in args.BlockedInstruments)
                        dic.AddOrUpdate(item.Key, item.Value, (key, oldValue) => item.Value);
                    break;
                case SyncMsgAction.DELETE:
                    BlockedInstrumentInfo info;
                    foreach (KeyValuePair<SymbolKey, BlockedInstrumentInfo> item in args.BlockedInstruments)
                        dic.TryRemove(item.Key, out info);
                    break;
                default:
                    logger.Error("Acao [" + args.SyncAction + "] invalida para dicionario de instrumentos bloqueados " + args.BlockedInstrumentType.ToString() );
                    break;
            }
        }

        private void OnClientLimitBMFSync(object sender, int clientNumber, Socket clientSocket, ClientLimitBMFSyncMsg args)
        {
            switch (args.SyncAction)
            {
                case SyncMsgAction.SNAPSHOT:
                    _dicClientLimitBMF.Clear();
                    _dicClientLimitBMF = args.ClientLimits;//.ToDictionary(entry => entry.Key, entry => entry.Value);
                    break;
                case SyncMsgAction.INSERT:
                    foreach (KeyValuePair<int, ClientLimitBMFInfo> item in args.ClientLimits)
                        _dicClientLimitBMF.AddOrUpdate(item.Key, item.Value, (key, oldValue) => item.Value);
                    break;
                case SyncMsgAction.UPDATE:
                    foreach (KeyValuePair<int, ClientLimitBMFInfo> item in args.ClientLimits)
                        _dicClientLimitBMF[item.Key] = item.Value;
                    break;
                case SyncMsgAction.DELETE:
                    ClientLimitBMFInfo info;
                    foreach (KeyValuePair<int, ClientLimitBMFInfo> item in args.ClientLimits)
                        _dicClientLimitBMF.TryRemove(item.Key, out info);
                    break;
                default:
                    logger.Error("Acao [" + args.SyncAction + "] invalida para dicionario de limites de cliente BMF");
                    break;
            }
        }

        private void OnClientParameterPermissionSync(object sender, int clientNumber, Socket clientSocket, ClientParameterPermissionSyncMsg args)
        {
            switch (args.SyncAction)
            {
                case SyncMsgAction.SNAPSHOT:
                    _dicParameters.Clear();
                    _dicParameters = args.Parametros; //.ToDictionary(entry => entry.Key, entry => entry.Value);
                    logger.Info("Recebeu snapshot de parametros de permissoes por cliente com " + args.Parametros.Count + " itens.");
                    break;
                case SyncMsgAction.INSERT:
                    foreach (KeyValuePair<int, ClientParameterPermissionInfo> item in args.Parametros)
                        _dicParameters.AddOrUpdate(item.Key, item.Value, (key, oldValue) => item.Value);
                    break;
                case SyncMsgAction.UPDATE:
                    foreach (KeyValuePair<int, ClientParameterPermissionInfo> item in args.Parametros)
                        _dicParameters[item.Key] = item.Value;
                    break;
                case SyncMsgAction.DELETE:
                    ClientParameterPermissionInfo info;
                    foreach (KeyValuePair<int, ClientParameterPermissionInfo> item in args.Parametros)
                        _dicParameters.TryRemove(item.Key, out info);
                    break;
                default:
                    logger.Error("Acao [" + args.SyncAction + "] invalida para dicionario de parametros de limites de cliente");
                    break;
            }
        }

        private void OnFatFingerSync(object sender, int clientNumber, Socket clientSocket, FatFingerSyncMsg args)
        {
            switch (args.SyncAction)
            {
                case SyncMsgAction.SNAPSHOT:
                    _dicFatFinger.Clear();
                    _dicFatFinger = args.FatFingers; //.ToDictionary(entry => entry.Key, entry => entry.Value);
                    logger.Info("Recebeu snapshot de parametros fatfinger com " + args.FatFingers.Count + " itens.");
                    break;
                case SyncMsgAction.INSERT:
                case SyncMsgAction.UPDATE:
                    foreach (KeyValuePair<int, FatFingerInfo> item in args.FatFingers)
                        _dicFatFinger.AddOrUpdate(item.Key, item.Value, (key, oldValue) => item.Value);
                    break;
                case SyncMsgAction.DELETE:
                    FatFingerInfo info = null;
                    foreach (KeyValuePair<int, FatFingerInfo> item in args.FatFingers)
                        _dicFatFinger.TryRemove(item.Key, out info);
                    break;
                default:
                    logger.Error("Acao [" + args.SyncAction + "] invalida para dicionario de parametros de fatfinger");
                    break;
            }
        }

        private void OnOperatingLimitSync(object sender, int clientNumber, Socket clientSocket, OperatingLimitSyncMsg args)
        {
            switch (args.SyncAction)
            {
                case SyncMsgAction.SNAPSHOT:
                    _dicOperatingLimit.Clear();
                    _dicOperatingLimit = args.OperatingLimits;//.ToDictionary(entry => entry.Key, entry => entry.Value);
                    logger.Info("Recebeu snapshot de parametros OperatingLimit com " + args.OperatingLimits.Count + " itens.");
                    break;
                case SyncMsgAction.INSERT:
                case SyncMsgAction.UPDATE:
                    foreach (KeyValuePair<int, List<OperatingLimitInfo>> item in args.OperatingLimits)
                        _dicOperatingLimit.AddOrUpdate(item.Key, item.Value, (key, oldValue) => item.Value);
                    break;
                case SyncMsgAction.DELETE:
                    List<OperatingLimitInfo> lst = null;
                    foreach (KeyValuePair<int, List<OperatingLimitInfo>> item in args.OperatingLimits)
                        _dicOperatingLimit.TryRemove(item.Key, out lst);
                    break;
                default:
                    logger.Error("Acao [" + args.SyncAction + "] invalida para dicionario de limites operacionais");
                    break;
            }
        }

        private void OnOptionBlockSync(object sender, int clientNumber, Socket clientSocket, OptionBlockSyncMsg args)
        {
            switch (args.SyncAction)
            {
                case SyncMsgAction.SNAPSHOT:
                    _dicOptionSeriesBlocked.Clear();
                    _dicOptionSeriesBlocked = args.OptionsBlocks; //.ToDictionary(entry => entry.Key, entry => entry.Value);
                    logger.Info("Recebeu snapshot de bloqueios de opcoes com " + args.OptionsBlocks.Count + " itens.");
                    break;
                case SyncMsgAction.INSERT:
                case SyncMsgAction.UPDATE:
                    foreach (KeyValuePair<string, OptionBlockInfo> item in args.OptionsBlocks)
                        _dicOptionSeriesBlocked.AddOrUpdate(item.Key, item.Value, (key, oldValue) => item.Value);
                    break;
                case SyncMsgAction.DELETE:
                    OptionBlockInfo info = null;
                    foreach (KeyValuePair<string, OptionBlockInfo> item in args.OptionsBlocks)
                        _dicOptionSeriesBlocked.TryRemove(item.Key, out info);
                    break;
                default:
                    logger.Error("Acao [" + args.SyncAction + "] invalida para dicionario de bloqueios de serie de opcoes");
                    break;
            }
        }

        private void OnSymbolListSync(object sender, int clientNumber, Socket clientSocket, SymbolListSyncMsg args)
        {
            switch (args.SyncAction)
            {
                case SyncMsgAction.SNAPSHOT:
                    _dicSymbols.Clear();
                    _dicSymbols = args.Symbols;//.ToDictionary(entry => entry.Key, entry => entry.Value);
                    logger.Info("Recebeu snapshot de lista de instrumentos com " + args.Symbols.Count + " itens.");
                    break;
                case SyncMsgAction.INSERT:
                case SyncMsgAction.UPDATE:
                    foreach (KeyValuePair<string, SymbolInfo> item in args.Symbols)
                        _dicSymbols.AddOrUpdate(item.Key, item.Value, (key, oldValue) => item.Value);
                    break;
                case SyncMsgAction.DELETE:
                    SymbolInfo symb = null;
                    foreach (KeyValuePair<string, SymbolInfo> item in args.Symbols)
                        _dicSymbols.TryRemove(item.Key, out symb);
                    break;
                default:
                    logger.Error("Acao [" + args.SyncAction + "] invalida para dicionario/lista de instrumentos");
                    break;
            }
        }

        private void OnTestSymbolSync(object sender, int clientNumber, Socket clientSocket, TestSymbolSyncMsg args)
        {
            switch (args.SyncAction)
            {
                case SyncMsgAction.SNAPSHOT:
                    _dicTestSymbols.Clear();
                    _dicTestSymbols = args.TestSymbols;// .To Dictionary(entry => entry.Key, entry => entry.Value);
                    logger.Info("Recebeu snapshot de lista de instrumentos de teste com " + args.TestSymbols.Count + " itens.");
                    break;
                case SyncMsgAction.INSERT:
                case SyncMsgAction.UPDATE:
                    foreach (KeyValuePair<string, TestSymbolInfo> item in args.TestSymbols)
                        _dicTestSymbols.AddOrUpdate(item.Key, item.Value, (key, oldValue) => item.Value);
                    break;
                case SyncMsgAction.DELETE:
                    TestSymbolInfo aux = null;
                    foreach (KeyValuePair<string, TestSymbolInfo> item in args.TestSymbols)
                        _dicTestSymbols.TryRemove(item.Key, out aux);
                    break;
                default:
                    logger.Error("Acao [" + args.SyncAction + "] invalida para dicionario/lista de instrumentos de testes");
                    break;
            }
        }

        private void OnContaBrokerSync(object sender, int clientNumber, Socket clientSocket, ContaBrokerSyncMsg args)
        {
            switch (args.SyncAction)
            {
                case SyncMsgAction.SNAPSHOT:
                    _dicContaBroker.Clear();
                    _dicContaBroker = args.ContaBroker.ToDictionary(entry => entry.Key, entry => entry.Value);
                    logger.Info("Recebeu snapshot de contas broker com " + args.ContaBroker.Count + " itens.");
                    break;
                case SyncMsgAction.INSERT:
                    foreach (KeyValuePair<int, List<ContaBrokerInfo>> item in args.ContaBroker)
                        _dicContaBroker.Add(item.Key, item.Value);
                    break;
                case SyncMsgAction.UPDATE:
                    foreach (KeyValuePair<int, List<ContaBrokerInfo>> item in args.ContaBroker)
                        _dicContaBroker[item.Key] = item.Value;
                    break;
                case SyncMsgAction.DELETE:
                    foreach (KeyValuePair<int, List<ContaBrokerInfo>> item in args.ContaBroker)
                        _dicContaBroker.Remove(item.Key);
                    break;
                default:
                    logger.Error("Acao [" + args.SyncAction + "] invalida para dicionario/lista de conta broker...");
                    break;
            }
        }

        private void OnRestrictionSymbolSync(object sender, int clientNumber, Socket clientSocket, RestrictionSymbolSyncMsg args)
        {
            //logger.InfoFormat("ClientNumber[{0}] SyncAction: [{1}] Symbol [{2}] VlNetAlocado[{3}] QtNetAlocado [{4}]",
            //    clientNumber, args.SyncAction, args.RestrictionSymbol.ElementAt(0).Value.Symbol, 
            //    args.RestrictionSymbol.ElementAt(0).Value.VolumeNetAlocado,
            //    args.RestrictionSymbol.ElementAt(0).Value.QuantidadeNetAlocada);
            switch (args.SyncAction)
            {
                case SyncMsgAction.SNAPSHOT:
                    _dicRestrSymbol.Clear();
                    _dicRestrSymbol = args.RestrictionSymbol;//.ToDictionary(entry => entry.Key, entry => entry.Value);
                    logger.Info("Recebeu snapshot de restriction symbol com " + args.RestrictionSymbol.Count + " itens.");
                    break;
                case SyncMsgAction.INSERT:
                case SyncMsgAction.UPDATE:
                    foreach (KeyValuePair<string, RestrictionSymbolInfo> item in args.RestrictionSymbol)
                        _dicRestrSymbol.AddOrUpdate(item.Key, item.Value, (key, oldValue) => item.Value);
                    break;
                case SyncMsgAction.DELETE:
                    RestrictionSymbolInfo info = null;
                    foreach (KeyValuePair<string, RestrictionSymbolInfo> item in args.RestrictionSymbol)
                        _dicRestrSymbol.TryRemove(item.Key, out info);
                    break;
                default:
                    logger.Error("Acao [" + args.SyncAction + "] invalida para dicionario/lista de restriction symbol...");
                    break;
            }
        }

        private void OnRestrictionGroupSymbolSync(object sender, int clientNumber, Socket clientSocket, RestrictionGroupSymbolSyncMsg args)
        {

            //logger.InfoFormat("ClientNumber[{0}] SyncAction: [{1}] IdGrupo [{2}] VlNetAlocado[{3}] QtNetAlocado [{4}]",
            //    clientNumber, args.SyncAction, args.RestrictionGroupSymbol.ElementAt(0).Value.IdGrupo,
            //    args.RestrictionGroupSymbol.ElementAt(0).Value.VolumeNetAlocado,
            //    args.RestrictionGroupSymbol.ElementAt(0).Value.QuantidadeNetAlocada);
            switch (args.SyncAction)
            {
                case SyncMsgAction.SNAPSHOT:
                    _dicRestrGroupSymbol.Clear();
                    _dicRestrGroupSymbol = args.RestrictionGroupSymbol;// .ToDictionary(entry => entry.Key, entry => entry.Value);
                    logger.Info("Recebeu snapshot de restriction group symbol com " + args.RestrictionGroupSymbol.Count + " itens.");
                    break;
                case SyncMsgAction.INSERT:
                case SyncMsgAction.UPDATE:
                    foreach (KeyValuePair<string, RestrictionGroupSymbolInfo> item in args.RestrictionGroupSymbol)
                        _dicRestrGroupSymbol.AddOrUpdate(item.Key, item.Value, (key, oldValue) => item.Value);
                    break;
                case SyncMsgAction.DELETE:
                    RestrictionGroupSymbolInfo info = null;
                    foreach (KeyValuePair<string, RestrictionGroupSymbolInfo> item in args.RestrictionGroupSymbol)
                        _dicRestrGroupSymbol.TryRemove(item.Key, out info);
                    break;
                default:
                    logger.Error("Acao [" + args.SyncAction + "] invalida para dicionario/lista de restriction group symbol...");
                    break;
            }
        }

        private void OnRestrictionGlobalSync(object sender, int clientNumber, Socket clientSocket, RestrictionGlobalSyncMsg args)
        {
            //logger.InfoFormat("ClientNumber[{0}] SyncAction: [{1}] Account [{2}] VlNetAlocado[{3}] QtNetAlocado [{4}]",
            //    clientNumber, args.SyncAction, args.RestrictionGlobal.ElementAt(0).Value.Account,
            //    args.RestrictionGlobal.ElementAt(0).Value.VolumeNetAlocado,
            //    args.RestrictionGlobal.ElementAt(0).Value.QuantidadeNetAlocada);
            switch (args.SyncAction)
            {
                case SyncMsgAction.SNAPSHOT:
                    _dicRestrGlobal.Clear();
                    _dicRestrGlobal = args.RestrictionGlobal;// .ToDictionary(entry => entry.Key, entry => entry.Value);
                    logger.Info("Recebeu snapshot de restriction global com " + args.RestrictionGlobal.Count + " itens.");
                    break;
                case SyncMsgAction.INSERT:
                case SyncMsgAction.UPDATE:
                    foreach (KeyValuePair<int, RestrictionGlobalInfo> item in args.RestrictionGlobal)
                        _dicRestrGlobal.AddOrUpdate(item.Key, item.Value, (key, oldValue) => item.Value);
                    break;
                case SyncMsgAction.DELETE:
                    RestrictionGlobalInfo info = null;
                    foreach (KeyValuePair<int, RestrictionGlobalInfo> item in args.RestrictionGlobal)
                        _dicRestrGlobal.TryRemove(item.Key, out info);
                    break;
                default:
                    logger.Error("Acao [" + args.SyncAction + "] invalida para dicionario/lista de restriction global...");
                    break;
            }
        }

        DateTime lastMsg = DateTime.MinValue;
        private void OnPositionClientSync(object sender, int clientNumber, Socket clientSocket, PositionClientSyncMsg args)
        {
            switch (args.SyncAction)
            {
                case SyncMsgAction.SNAPSHOT:
                    _dicPositionClient.Clear();
                    _dicPositionClient = args.PositionClient;//.ToDictionary(entry => entry.Key, entry => entry.Value);
                    logger.Info("Recebeu snapshot de position client com " + args.PositionClient.Count + " itens.");
                    break;
                case SyncMsgAction.INSERT:
                case SyncMsgAction.UPDATE:
                    foreach (KeyValuePair<int, List<PosClientSymbolInfo>> item in args.PositionClient)
                    {
                        List<PosClientSymbolInfo> lstOut = null;
                        if (_dicPositionClient.TryGetValue(item.Key, out lstOut))
                        {
                            PosClientSymbolInfo posCli = lstOut.FirstOrDefault(x=>x.Ativo.Equals(item.Value[0].Ativo) && 
                                 x.ExecBroker.Equals(item.Value[0].ExecBroker));
                            if (posCli != null)
                                posCli = item.Value[0];
                            else
                                lstOut.Add(item.Value[0]);
                        }
                        else
                            _dicPositionClient.AddOrUpdate(item.Key, item.Value, (key, oldValue) => item.Value);
                        TimeSpan t1 = new TimeSpan(item.Value[0].DtMovimento.Ticks);
                        TimeSpan t2 = new TimeSpan(lastMsg.Ticks);
                        //if ((t1 - t2).Milliseconds >= 50)
                        //{
                            logger.InfoFormat("UPDATE: Account:[{0}]  Symbol:[{1}] VlrUltimo:[{2}] DateTime[{3}] MsgId[{4}] EventSource[{5}]",
                                item.Value[0].Account, item.Value[0].Ativo, item.Value[0].UltPreco,
                                item.Value[0].DtMovimento.ToString("HH:mm:sss.fff"), item.Value[0].MsgId, item.Value[0].EventSource);
                            lastMsg = DateTime.Now;
                        //}
                        
                    }
                    break;
                case SyncMsgAction.DELETE:
                    List<PosClientSymbolInfo> info = null;
                    foreach (KeyValuePair<int, List<PosClientSymbolInfo>> item in args.PositionClient)
                        _dicPositionClient.TryRemove(item.Key, out info);
                    break;
                default:
                    logger.Error("Acao [" + args.SyncAction + "] invalida para dicionario/lista de restriction symbol...");
                    break;
            }
        }

        private void OnMaxLossSync(object sender, int clientNumber, Socket clientSocket, MaxLossSyncMsg args)
        {

            switch (args.SyncAction)
            {
                case SyncMsgAction.SNAPSHOT:
                    break;
                    // TODO[FF]: Verificar de atualizar SOMENTE os elementos de dentro da lista, para nao alterar informacoes
                    // de OperationLimit
                case SyncMsgAction.INSERT:
                case SyncMsgAction.UPDATE:
                    {
                        foreach (KeyValuePair<int, List<OperatingLimitInfo>> item in args.MaxLoss)
                        {
                            List<OperatingLimitInfo> lst = null;
                            if (_dicOperatingLimit.TryGetValue(item.Key, out lst))
                            {
                                for (int i = 0; i < item.Value.Count; i++)
                                {
                                    int aux = lst.FindIndex(x => x.TipoLimite == item.Value[i].TipoLimite);
                                    if (aux < 0)
                                        lst.Add(item.Value[i]);
                                    else
                                        lst[aux] = item.Value[i];
                                }
                            }
                            else
                            {
                                lst = new List<OperatingLimitInfo>();
                                lst.AddRange(item.Value);
                                _dicOperatingLimit.AddOrUpdate(item.Key, lst, (key, oldValue) => lst);
                            }
                            _dicOperatingLimit.AddOrUpdate(item.Key, item.Value, (key, oldValue) => item.Value);
                        }
                    }
                    break;
                case SyncMsgAction.DELETE:
                    {
                        List<OperatingLimitInfo> info = null;
                        foreach (KeyValuePair<int, List<OperatingLimitInfo>> item in args.MaxLoss)
                        {
                            if (_dicOperatingLimit.TryGetValue(item.Key, out info))
                            {
                                for (int i = 0; i < item.Value.Count; i++)
                                {
                                    int aux = info.FindIndex(x => x.TipoLimite == item.Value[i].TipoLimite);
                                    if (aux >= 0)
                                        info.RemoveAt(aux);
                                }
                            }
                        }
                    }
                    break;
                default:
                    logger.Error("Acao [" + args.SyncAction + "] invalida para dicionario/lista de Max Loss...");
                    break;
            }
        }


        private void OnConsolidatedRiskSync(object sender, int clientNumber, Socket clientSocket, ConsolidatedRiskSyncMsg args)
        {
            switch (args.SyncAction)
            {
                case SyncMsgAction.SNAPSHOT:
                    _dicConsolidatedRisk.Clear();
                    _dicConsolidatedRisk= args.ConsolidatedRisk;
                    logger.Info("Recebeu snapshot de risco connsolidado " + args.ConsolidatedRisk.Count + " itens.");
                    break;
                case SyncMsgAction.INSERT:
                case SyncMsgAction.UPDATE:
                    foreach (KeyValuePair<int, ConsolidatedRiskInfo> item in args.ConsolidatedRisk  )
                    {
                        ConsolidatedRiskInfo cr = null;
                        if (_dicConsolidatedRisk.TryGetValue(item.Key, out cr))
                        {
                            cr = item.Value;
                        }
                        else
                        {
                            _dicConsolidatedRisk.AddOrUpdate(item.Key, item.Value, (key, oldValue) => item.Value);
                            cr = item.Value;
                        }
                        logger.InfoFormat("UPDT CONSOLIDATED RISK Account[{0}] PlBov [{1}] PlBmf [{2}] PlTotal[{3}] SFP[{4}]", 
                            cr.Account, cr.PLBovespa, cr.PLBmf, cr.PLTotal, cr.SFP);
                    }
                    break;
                case SyncMsgAction.DELETE:
                    ConsolidatedRiskInfo info = null;
                    foreach (KeyValuePair<int, ConsolidatedRiskInfo> item in args.ConsolidatedRisk)
                        _dicConsolidatedRisk.TryRemove(item.Key, out info);
                    break;
                default:
                    logger.Error("Acao [" + args.SyncAction + "] invalida para dicionario/lista risco consolidado...");
                    break;
            }
        }


        public ValidarRiscoResponse ValidarPermissaoERisco(ValidarRiscoRequest request)
        {
            ValidarRiscoResponse response = new ValidarRiscoResponse();
            SpiderOrderInfo order = request.Ordem;

            response.ValidationResult = false;

            TipoLimiteEnum tpLimite = TipoLimiteEnum.INDEFINIDO;

            try
            {
                LimitResponse ret = new LimitResponse();
                int account = 0;
                int accType;
                if (order.Exchange.Equals(ExchangePrefixes.BOVESPA, StringComparison.InvariantCultureIgnoreCase))
                    account = order.Account;
                else
                {
                    account = this.ParseAccount(order.Account, out accType);
                }

                //int seqnum = msg.Header.GetInt(Tags.MsgSeqNum);
                if (String.IsNullOrEmpty(request.FixMsgType))
                {
                    response.ValidationResult = false;
                    response.RejectMessage = "request.FixMsgType must be filled";
                    return response;
                }
                string msgt = request.FixMsgType;
                string symbol = !String.IsNullOrEmpty(order.Symbol) ? order.Symbol.Trim().ToUpper() : string.Empty;
                Decimal orderQty = Decimal.Zero;
                //int acc = 0;
                string orderId = string.Empty;
                string origClOrdID = string.Empty;

                orderQty = order.OrderQty > 0  ? order.OrderQty : Decimal.Zero;
                orderId = !String.IsNullOrEmpty(order.OrderID) ? order.OrderID : string.Empty;
                origClOrdID = !String.IsNullOrEmpty(order.OrigClOrdID) ? order.OrigClOrdID : string.Empty;


                // Regra de exposicao patrimonial
                // Verificacao de papeis teste (bmf e bovespa)
                ret = this.VerifyTestInstrument(symbol);
                if (0 == ret.ErrorCode)
                {
                    response.ValidationResult = true;
                    return response;
                }

                // Permissao por bloqueio de envio de ordens no OMS
                ret = this.VerifyOMSOrderSend(account);
                if (0 != ret.ErrorCode)
                {
                    response.RejectMessage = ret.ErrorMessage;
                    return response;
                }


                // Permissao por segmento de mercado (ira retornar o SymbolInfo para futuras utilizacoes
                ret = this.VerifyMarketPermission(account, symbol, order.Exchange);
                SymbolInfo symbolInfo = ret.InfoObject as SymbolInfo;
                if (0 != ret.ErrorCode)
                {
                    response.RejectMessage = ret.ErrorMessage;
                    return response;
                }

                // Permissao global do instrumento
                string side = order.Side;
                ret = this.VerifyInstrumentGlobalPermission(symbolInfo.Instrumento, Convert.ToInt32(side.ToString()));
                if (0 != ret.ErrorCode)
                {
                    response.RejectMessage = ret.ErrorMessage;
                    return response;
                }
                // Permissao instrumento x grupo
                ret = this.VerifyInstrumentPerGroupPermission(symbolInfo.Instrumento, Convert.ToInt32(side.ToString()), account);
                if (0 != ret.ErrorCode)
                {
                    response.RejectMessage = ret.ErrorMessage;
                    return response;
                }
                // Permissao Instrumento x Cliente
                ret = this.VerifyInstrumentClientPermission(symbolInfo.Instrumento, Convert.ToInt32(side.ToString()), account);
                if (0 != ret.ErrorCode)
                {
                    response.RejectMessage = ret.ErrorMessage;
                    return response;
                }

                // FatFinger - nao se aplica a bmf 
                if (order.Exchange.Equals(ExchangePrefixes.BOVESPA, StringComparison.InvariantCultureIgnoreCase))
                {
                    ret = this.VerifyFatFinger(account, symbolInfo, orderQty);
                    if (0 != ret.ErrorCode)
                    {
                        response.RejectMessage = ret.ErrorMessage;
                        return response;
                    }

                    // TODO [FF]: Fazer a chamada do VerifyMaxLoss (com qual valor deverah ser chamado???)
                    ret = this.VerifyMaxLoss(account, symbolInfo, Decimal.Zero);
                    if (0 != ret.ErrorCode)
                    {
                        response.RejectMessage = ret.ErrorMessage;
                        return response;
                    }
                        
                }

                // Permissao de perfil institucional. Caso achado, ignora-se o 
                // o calculo de limite operacional (motivo pelo qual se retorna true a funcao)
                ret = this.VerifyInstitutionalProfile(account);
                if (0 != ret.ErrorCode)
                {
                    response.ValidationResult = true;
                    return response;
                }

                /////////////////////////// PARAMETROS
                // Limite Operacional (compra a vista, venda a vista, compra de opcao, venda de opcao)
                // Bovespa

                if (order.Exchange.Equals(ExchangePrefixes.BOVESPA, StringComparison.InvariantCultureIgnoreCase))
                {
                    // No original, utiliza valor da ultima cotacao
                    // Validar o tipo de Saldo operacional
                    // Compra
                    if ( side.Equals("1") )
                    {
                        switch (symbolInfo.SegmentoMercado)
                        {
                            case SegmentoMercadoEnum.AVISTA:
                            case SegmentoMercadoEnum.FRACIONARIO:
                            case SegmentoMercadoEnum.INTEGRALFRACIONARIO:
                                tpLimite = TipoLimiteEnum.COMPRAAVISTA;
                                break;
                            case SegmentoMercadoEnum.OPCAO:
                                {
                                    tpLimite = TipoLimiteEnum.COMPRAOPCOES;

                                    string strSerieOpcao = Regex.Replace(symbolInfo.Instrumento, "[^A-Za-z _]", string.Empty);
                                    strSerieOpcao = strSerieOpcao.Substring(strSerieOpcao.Length - 1, 1);
                                    ret = this.VerifiyOptionSeriesBlocked(strSerieOpcao);
                                    if (0 != ret.ErrorCode)
                                    {
                                        response.RejectMessage = ret.ErrorMessage;
                                        return response;
                                    }
                                }
                                break;
                            default:
                                tpLimite = TipoLimiteEnum.COMPRAAVISTA;
                                break;

                        }
                    }
                    // Venda
                    if (side.Equals("2"))
                    {
                        switch (symbolInfo.SegmentoMercado)
                        {
                            case SegmentoMercadoEnum.AVISTA:
                            case SegmentoMercadoEnum.FRACIONARIO:
                            case SegmentoMercadoEnum.INTEGRALFRACIONARIO:
                                tpLimite = TipoLimiteEnum.VENDAAVISTA;
                                break;
                            case SegmentoMercadoEnum.OPCAO:
                                {
                                    // Validar se a opcao esta bloqueada 
                                    tpLimite = TipoLimiteEnum.VENDAOPCOES;
                                    string strSerieOpcao = Regex.Replace(symbolInfo.Instrumento, "[^A-Za-z _]", string.Empty);
                                    strSerieOpcao = strSerieOpcao.Substring(strSerieOpcao.Length - 1, 1);
                                    ret = this.VerifiyOptionSeriesBlocked(strSerieOpcao);
                                    if (0 != ret.ErrorCode)
                                    {
                                        response.RejectMessage = ret.ErrorMessage;
                                        return response;
                                    }
                                }
                                break;
                            default:
                                tpLimite = TipoLimiteEnum.VENDAAVISTA;
                                break;
                        }
                    }

                    // new order
                    if (request.FixMsgType.Equals("D") )
                    {
                        // Fazer marcacao a mercado
                        decimal ultimoPreco = symbolInfo.VlrUltima;
                        if (Decimal.Zero == ultimoPreco)
                        {
                            // Erro de preco base de calculo
                            // Usando mensagem do "fat finger" somente para nao criar nova constante
                            logger.Info("Preco de cotacao e Valor de fechamento zerado - new order");
                            ret = _fillResponse(ErrorMessages.ERR_CODE_FAT_FINGER_BASE_PRICE_ZEROED, ErrorMessages.ERR_FAT_FINGER_BASE_PRICE_ZEROED);
                            response.RejectMessage = ret.ErrorMessage;
                            return response;
                        }
                        decimal volumeOrdem = orderQty * ultimoPreco;
                        ret = this.VerifyOperatingLimit(account, tpLimite, volumeOrdem);
                    }
                    // replace
                    else
                    {
                        decimal volumeOriginal = decimal.Zero;
                        // Fazer marcacao a mercado
                        decimal ultimoPreco = symbolInfo.VlrUltima;
                        if (Decimal.Zero == ultimoPreco)
                        {
                            // Erro de preco base de calculo
                            // Usando mensagem do "fat finger" somente para nao criar nova constante
                            logger.Info("Preco de cotacao e Valor de fechamento zerado - order cancel replace request");
                            ret = _fillResponse(ErrorMessages.ERR_CODE_FAT_FINGER_BASE_PRICE_ZEROED, ErrorMessages.ERR_FAT_FINGER_BASE_PRICE_ZEROED);
                            response.RejectMessage = ret.ErrorMessage;
                            return response;
                        }

                        decimal volumeAlteracao = orderQty * ultimoPreco;
                        decimal diferencial = decimal.Zero;

                        string chave = origClOrdID + "-" + account + "-" + order.Symbol;

                        if (null != request.OrdemOriginal)
                        {
                            volumeOriginal = (decimal) ( request.OrdemOriginal.OrderQty * ultimoPreco);
                            diferencial = volumeAlteracao - volumeOriginal;
                        }
                        else
                        {
                            ret = this._fillResponse(ErrorMessages.ERR_CODE_TO_ORDER_NOT_FOUND, ErrorMessages.ERR_TO_ORDER_NOT_FOUND + ": " + chave);
                            response.RejectMessage = ret.ErrorMessage;
                            return response;
                        }
                        ret = this.VerifyOperatingLimit(account, tpLimite, diferencial);
                    }
                    if (0 != ret.ErrorCode)
                    {
                        response.RejectMessage = ret.ErrorMessage;
                        return response;
                    }
                }
                // BMF
                else
                {
                    // Limite operacional BMF
                    if (request.FixMsgType.Equals("D") )
                    {
                        ret = this.VerifyClientBMFLimit(account, symbolInfo.Instrumento, side.ToString(), orderQty, orderQty);
                    }
                    else
                    {
                        decimal qtdOriginal = decimal.Zero;
                        decimal qtdAlteracao = order.OrderQty;
                        decimal diferencial = decimal.Zero;
                        string chave = order.OrigClOrdID + "-" + order.Account + "-" + order.Symbol;
                        string chaveExch = string.Empty;
                        
                        if ( !String.IsNullOrEmpty(order.OrderID) )
                            chaveExch = order.OrderID + "-" + order.Account + "-" + order.Symbol ;

                        if (null != request.OrdemOriginal )
                        {
                            qtdOriginal = request.OrdemOriginal.OrderQty;
                            diferencial = qtdAlteracao - qtdOriginal;
                        }
                        else
                        {
                            ret = this._fillResponse(ErrorMessages.ERR_CODE_TO_ORDER_NOT_FOUND, ErrorMessages.ERR_TO_ORDER_NOT_FOUND + ": " + chave);
                            response.RejectMessage = ret.ErrorMessage;
                            return response;
                        }
                        ret = this.VerifyClientBMFLimit(account, symbolInfo.Instrumento, side.ToString(), diferencial, qtdAlteracao);
                    }
                    if (0 != ret.ErrorCode)
                    {
                        response.RejectMessage = ret.ErrorMessage;
                        return response;
                    }
                }

                response.ValidationResult = true;
                response.TipoLimite = tpLimite;
            }
            catch (Exception ex)
            {
                logger.Error("Erro na verificacao do limite: " + ex.Message, ex);
            }

            return response;
        }

        /// <summary>
        /// Verifica se é um instrumento de teste TF/TZ
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public LimitResponse VerifyTestInstrument(string symbol)
        {
            LimitResponse ret = _fillResponse(ErrorMessages.OK, ErrorMessages.MSG_OK);
            try
            {
                if (_dicTestSymbols==null || _dicTestSymbols.Count==0)
                {
                    ret = _fillResponse(ErrorMessages.ERR_CODE_DATA_NOT_LOADED, ErrorMessages.ERR_DATA_NOT_LOADED);
                    return ret;
                }

                TestSymbolInfo aux = null;
                string key1 = ExchangePrefixes.BOVESPA + "/" + symbol;
                string key2 = ExchangePrefixes.BMF + "/" + symbol;
                if (null != _dicTestSymbols)
                {
                    if (_dicTestSymbols.TryGetValue(key1, out aux) || _dicTestSymbols.TryGetValue(key2, out aux))
                    {
                        return ret;
                    }
                    else
                    {
                        ret = _fillResponse(ErrorMessages.ERR_CODE_TEST_INSTRUMENT_NOT_FOUND, ErrorMessages.ERR_TEST_INSTRUMENT_NOT_FOUND);
                        return ret;
                    }
                }
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na validacao papeis de teste: " + ex.Message, ex);
                ret = _fillResponse(ErrorMessages.ERROR, ex.Message, ex.StackTrace);
                return ret;
            }
        }

        /// <summary>
        /// Verifica bloqueio para envio de ordens
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public LimitResponse VerifyOMSOrderSend(int account)
        {
            LimitResponse ret = _fillResponse(ErrorMessages.OK, ErrorMessages.MSG_OK);
            try
            {
                ClientParameterPermissionInfo clientPermission = null;
                if (_dicParameters==null || _dicParameters.Count==0)
                {
                    ret = _fillResponse(ErrorMessages.ERR_CODE_DATA_NOT_LOADED, ErrorMessages.ERR_DATA_NOT_LOADED);
                    return ret;
                }

                // Verificar se existe parametro / permissao do cliente 
                if (!_dicParameters.TryGetValue(account, out clientPermission))
                {
                    ret = _fillResponse(ErrorMessages.ERR_CODE_PARAM_PERM_NOT_FOUND, ErrorMessages.ERR_PARAM_PERM_NOT_FOUND);
                    return ret;
                }

                // Validacao de bloqueio de envio de Ordem OMS
                ParameterPermissionClientInfo item = null;
                item = clientPermission.Permissoes.Find(x => x.Permissao == RiscoPermissoesEnum.BloquearEnvioOrdemOMS);
                if (null != item)
                {
                    string msg = string.Format(ErrorMessages.ERR_OMS_SENDING_ORDER, account);
                    ret = _fillResponse(ErrorMessages.ERR_CODE_OMS_SENDING_ORDER, msg);
                    return ret;
                }
                // Validacao de permissao de roteamento de ordens spider
                ParameterPermissionClientInfo item2 = null;
                item2 = clientPermission.Permissoes.Find(x => x.Permissao == RiscoPermissoesEnum.RoteamentoOrdensSpider);
                if (null == item2)
                {
                    string msg = string.Format(ErrorMessages.ERR_SPIDER_PERMISSION, account);
                    ret = _fillResponse(ErrorMessages.ERR_CODE_SPIDER_PERMISSION, msg);
                    return ret;
                }
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na validacao de permissao de envio de ordens: " + ex.Message, ex);
                ret = _fillResponse(ErrorMessages.ERROR, ex.Message, ex.StackTrace);
                return ret;
            }
        }

        /// <summary>
        /// Verificar permissao de segmento mercado pelo cliente
        /// Baseado em ProcessarOrdem, region EXPOSICAO PATRIMONIAL E PERMISSOES POR SEGMENTO DE MERCADO.
        /// 
        /// </summary>
        /// <param name="account"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public LimitResponse VerifyMarketPermission(int account, string symbol, string exchange)
        {
            LimitResponse ret = _fillResponse(ErrorMessages.OK, ErrorMessages.MSG_OK);
            try
            {

                ClientParameterPermissionInfo clientPermission = null;
                if (_dicParameters==null || _dicSymbols == null || _dicParameters.Count==0)
                {
                    ret = _fillResponse(ErrorMessages.ERR_CODE_DATA_NOT_LOADED, ErrorMessages.ERR_DATA_NOT_LOADED);
                    return ret;
                }

                // Buscar informacoes do cadastro de papel
                SymbolInfo instrument = null;
                if (!_dicSymbols.TryGetValue(symbol, out instrument))
                {
                    ret = _fillResponse(ErrorMessages.ERR_CODE_SYMBOL_NOT_FOUND, ErrorMessages.ERR_SYMBOL_NOT_FOUND);
                    return ret;
                }

                //if (!exchange.Equals(ExchangePrefixes.BMF, StringComparison.InvariantCultureIgnoreCase))
                //    CotacaoManager.Instance.AddInstrument(symbol, instrument);

                // Verificar se existe parametro / permissao do cliente (ValidarExposicaoPatrimonial)
                if (!_dicParameters.TryGetValue(account, out clientPermission))
                {
                    ret = _fillResponse(ErrorMessages.ERR_CODE_PARAM_PERM_NOT_FOUND, ErrorMessages.ERR_PARAM_PERM_NOT_FOUND);
                    return ret;
                }
                ParameterPermissionClientInfo item = null;
                switch (instrument.SegmentoMercado)
                {
                    case SegmentoMercadoEnum.AVISTA:
                        item = clientPermission.Permissoes.Find(x => x.Permissao == RiscoPermissoesEnum.OperarMercadoAVista);
                        if (null == item)
                        {
                            string msg = string.Format(ErrorMessages.ERR_NO_PERM_VISTA, account);
                            ret = _fillResponse(ErrorMessages.ERR_CODE_NO_PERM_VISTA, msg);
                            return ret;
                        }
                        break;
                    case SegmentoMercadoEnum.OPCAO:
                        item = clientPermission.Permissoes.Find(x => x.Permissao == RiscoPermissoesEnum.OperarMercadoOpcoes);
                        if (null == item)
                        {
                            string msg = string.Format(ErrorMessages.ERR_NO_PERM_OPCAO, account);
                            ret = _fillResponse(ErrorMessages.ERR_CODE_NO_PERM_OPCAO, msg);
                            return ret;
                        }
                        break;
                    case SegmentoMercadoEnum.FUTURO:
                        item = clientPermission.Permissoes.Find(x => x.Permissao == RiscoPermissoesEnum.OperarMercadoFuturo);
                        if (null == item)
                        {
                            string msg = string.Format(ErrorMessages.ERR_NO_PERM_FUT, account);
                            ret = _fillResponse(ErrorMessages.ERR_CODE_NO_PERM_FUT, msg);
                            return ret;
                        }
                        break;
                }
                ret.InfoObject = instrument;
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na validacao de permissoes de mercado: " + ex.Message, ex);
                ret = _fillResponse(ErrorMessages.ERROR, ex.Message, ex.StackTrace);
                return ret;
            }
        }

        /// <summary>
        /// Verificar permissoes de negociacao do instrumento
        /// Baseado em ProcessarOrdens, region PERMISSAO GLOBAL DO INSTRUMENTO
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public LimitResponse VerifyInstrumentGlobalPermission(string symbol, int side)
        {
            LimitResponse ret = _fillResponse(ErrorMessages.OK, ErrorMessages.MSG_OK);
            try
            {
                if (_dicBlockedSymbolGroupClient==null)
                {
                    ret = _fillResponse(ErrorMessages.ERR_CODE_DATA_NOT_LOADED, ErrorMessages.ERR_DATA_NOT_LOADED);
                    return ret;
                }

                BlockedInstrumentInfo security = null;
                SymbolKey aux = new SymbolKey();
                aux.Instrument = symbol.ToUpper();
                switch (side)
                {
                    case 1:
                        aux.Side = SentidoBloqueioEnum.Compra; break;
                    case 2:
                        aux.Side = SentidoBloqueioEnum.Venda; break;
                    default:
                        aux.Side = SentidoBloqueioEnum.Ambos; break;
                }
                // Se nao encontrado, retorna ok, instrumento nao esta bloqueado
                if (null != _dicBlockedSymbolGlobal)
                {
                    if (!_dicBlockedSymbolGlobal.TryGetValue(aux, out security))
                    {
                        return ret;
                    }
                    // Encontrado, instrumento esta bloqueado
                    else
                    {
                        ret = _fillResponse(ErrorMessages.ERR_CODE_INSTRUMENT_GLOBAL_BLOCKED, ErrorMessages.ERR_INSTRUMENT_GLOBAL_BLOCKED);
                        return ret;
                    }
                }
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na validacao de permissao global de instrumento: " + ex.Message, ex);
                ret = _fillResponse(ErrorMessages.ERROR, ex.Message, ex.StackTrace);
                return ret;
            }
        }

        /// <summary>
        /// Validar bloqueio de instrumento por grupo
        /// Baseado em ProcessarOrdem, region PERMISSAO INSTRUMENTO X GRUPO
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="side"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public LimitResponse VerifyInstrumentPerGroupPermission(string symbol, int side, int account)
        {
            LimitResponse ret = _fillResponse(ErrorMessages.OK, ErrorMessages.MSG_OK);
            try
            {
                if (_dicBlockedSymbolGroupClient==null)
                {
                    ret = _fillResponse(ErrorMessages.ERR_CODE_DATA_NOT_LOADED, ErrorMessages.ERR_DATA_NOT_LOADED);
                    return ret;
                }

                BlockedInstrumentInfo security = null;
                SymbolKey aux = new SymbolKey();
                aux.Instrument = symbol.ToUpper();
                switch (side)
                {
                    case 1:
                        aux.Side = SentidoBloqueioEnum.Compra; break;
                    case 2:
                        aux.Side = SentidoBloqueioEnum.Venda; break;
                    default:
                        aux.Side = SentidoBloqueioEnum.Ambos; break;
                }
                aux.Account = account;
                // Se nao encontrado, retorna ok, instrumento nao esta bloqueado
                if (null != _dicBlockedSymbolGroupClient)
                {
                    if (!_dicBlockedSymbolGroupClient.TryGetValue(aux, out security))
                    {
                        return ret;
                    }
                    // instrumento encontrado, entao esta bloqueado por grupo
                    else
                    {
                        ret = _fillResponse(ErrorMessages.ERR_CODE_INSTRUMENT_GROUP_BLOCKED, ErrorMessages.ERR_INSTRUMENT_GROUP_BLOCKED);
                        return ret;
                    }
                }
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na validacao de permissao por grupo do instrumento: " + ex.Message, ex);
                ret = _fillResponse(ErrorMessages.ERROR, ex.Message, ex.StackTrace);
                return ret;
            }
        }

        /// <summary>
        /// Validar bloqueio de instrumento por cliente
        /// Baseado em ProcessarOrdem, region PERMISSAO INSTRUMENTO CLIENTE
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="side"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public LimitResponse VerifyInstrumentClientPermission(string symbol, int side, int account)
        {
            LimitResponse ret = _fillResponse(ErrorMessages.OK, ErrorMessages.MSG_OK);
            try
            {
                if (_dicBlockedSymbolClient==null)
                {
                    ret = _fillResponse(ErrorMessages.ERR_CODE_DATA_NOT_LOADED, ErrorMessages.ERR_DATA_NOT_LOADED);
                    return ret;
                }

                BlockedInstrumentInfo security = null;
                SymbolKey aux = new SymbolKey();
                aux.Instrument = symbol.ToUpper();
                switch (side)
                {
                    case 1:
                        aux.Side = SentidoBloqueioEnum.Compra; break;
                    case 2:
                        aux.Side = SentidoBloqueioEnum.Venda; break;
                    default:
                        aux.Side = SentidoBloqueioEnum.Ambos; break;
                }
                aux.Account = account;
                // Se nao encontrado, retorna ok, instrumento nao esta bloqueado
                if (null != _dicBlockedSymbolClient)
                {
                    if (!_dicBlockedSymbolClient.TryGetValue(aux, out security))
                    {
                        return ret;
                    }
                    // instrumento encontrado, entao esta bloqueado por grupo
                    else
                    {
                        ret = _fillResponse(ErrorMessages.ERR_CODE_INSTRUMENT_CLIENT_BLOCKED, ErrorMessages.ERR_INSTRUMENT_CLIENT_BLOCKED);
                        return ret;
                    }
                }
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na validacao de permissao por cliente do instrumento: " + ex.Message, ex);
                ret = _fillResponse(ErrorMessages.ERROR, ex.Message, ex.StackTrace);
                return ret;
            }
        }
        /// <summary>
        /// Validacao de regras de FatFinger (volume por boleta)
        /// Baseado em ProcessarOrdem, region FATFINGER
        /// </summary>
        /// <param name="account"></param>
        /// <param name="symbol"></param>
        /// <param name="orderQty"></param>
        /// <returns></returns>
        public LimitResponse VerifyFatFinger(int account, SymbolInfo symbol, decimal orderQty)
        {
            LimitResponse ret = _fillResponse(ErrorMessages.OK, ErrorMessages.MSG_OK);
            try
            {
                if (_dicOperatingLimit==null || _dicOperatingLimit.Count == 0)
                {
                    ret = _fillResponse(ErrorMessages.ERR_CODE_DATA_NOT_LOADED, ErrorMessages.ERR_DATA_NOT_LOADED);
                    return ret;
                }

                // Buscar lista de "limites"
                List<OperatingLimitInfo> lstOpLimit = null;
                if (!_dicOperatingLimit.TryGetValue(account, out lstOpLimit))
                {
                    ret = _fillResponse(ErrorMessages.ERR_CODE_OPERATING_LIMIT_NOT_FOUND, ErrorMessages.ERR_OPERATING_LIMIT_NOT_FOUND);
                    return ret;
                }

                OperatingLimitInfo fatFinger = lstOpLimit.FirstOrDefault(x => x.TipoLimite == TipoLimiteEnum.MAXIMOPORBOLETA);
                if (null == fatFinger)
                {
                    ret = _fillResponse(ErrorMessages.ERR_CODE_FAT_FINGER_NOT_FOUND, ErrorMessages.ERR_FAT_FINGER_NOT_FOUND);
                    return ret;
                }

                // Validar o volume da oferta
                decimal valorMaximo, quantidade, volume = decimal.Zero;
                valorMaximo = fatFinger.ValotTotal;
                quantidade = orderQty;
                decimal vlrCotacao = symbol.VlrUltima;

                if (vlrCotacao == Decimal.Zero)
                {
                    // Erro de preco base de calculo
                    ret = _fillResponse(ErrorMessages.ERR_CODE_FAT_FINGER_BASE_PRICE_ZEROED, ErrorMessages.ERR_FAT_FINGER_BASE_PRICE_ZEROED);
                    return ret;
                }

                if (symbol.LotePadrao > 100)
                    volume = (vlrCotacao * (quantidade / symbol.LotePadrao));
                else
                    volume = (vlrCotacao * quantidade);
                if (volume > valorMaximo)
                {
                    ret = _fillResponse(ErrorMessages.ERR_CODE_ORDER_LIMIT_EXCEEDED, ErrorMessages.ERR_ORDER_LIMIT_EXCEEDED);
                    return ret;
                }
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na validacao de fat finger por cliente: " + ex.Message, ex);
                ret = _fillResponse(ErrorMessages.ERROR, ex.Message, ex.StackTrace);
                return ret;
            }
        }

        /// <summary>
        /// Validacao de regras de FatFinger (volume por boleta)
        /// Baseado em ProcessarOrdem, region FATFINGER
        /// </summary>
        /// <param name="account"></param>
        /// <param name="symbol"></param>
        /// <param name="orderQty"></param>
        /// <returns></returns>
        public LimitResponse VerifyMaxLoss(int account, SymbolInfo symbol, decimal valueToCompare)
        {
            LimitResponse ret = _fillResponse(ErrorMessages.OK, ErrorMessages.MSG_OK);
            try
            {
                if (_dicOperatingLimit == null || _dicOperatingLimit.Count == 0)
                {
                    ret = _fillResponse(ErrorMessages.ERR_CODE_DATA_NOT_LOADED, ErrorMessages.ERR_DATA_NOT_LOADED);
                    return ret;
                }

                // Buscar informacoes da regra de operacao de limites
                List<OperatingLimitInfo> lstOpLimit = null;
                if (!_dicOperatingLimit.TryGetValue(account, out lstOpLimit))
                {
                    ret = _fillResponse(ErrorMessages.ERR_CODE_OPERATING_LIMIT_NOT_FOUND, ErrorMessages.ERR_OPERATING_LIMIT_NOT_FOUND);
				    return ret;
                }

                OperatingLimitInfo item = null;
                // Buscar o parametro de Perda Maxima
                if (symbol.SegmentoMercado == SegmentoMercadoEnum.AVISTA)
                    item = lstOpLimit.FirstOrDefault(x => x.TipoLimite == TipoLimiteEnum.PERDAMAXIMAAVISTA);    
                else
                    item = lstOpLimit.FirstOrDefault(x => x.TipoLimite == TipoLimiteEnum.PERDAMAXIMAOPCOES);    
                
                if (null == item)
                {
                    logger.Debug("Regra PerdaMaxima nao encontrada para " + account);
                    // Retorna ok
                    return ret;
                }

                if (valueToCompare > item.ValotTotal)
                {
                    ret = _fillResponse(ErrorMessages.ERR_CODE_MAX_LOSS_EXCEEDED, ErrorMessages.ERR_MAX_LOSS_EXCEEDED);
                    return ret;
                }
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na validacao de fat finger por cliente: " + ex.Message, ex);
                ret = _fillResponse(ErrorMessages.ERROR, ex.Message, ex.StackTrace);
                return ret;
            }
        }


        /// <summary>
        /// Verifica se a serie da opcao esta bloqueada
        /// baseado em ProcessarOrdem, region VALIDA SE A SERIE DE OPCAO ESTA BLOQUEADA
        /// </summary>
        /// <param name="serie"></param>
        /// <returns></returns>
        public LimitResponse VerifiyOptionSeriesBlocked(string serie)
        {
            LimitResponse ret = _fillResponse(ErrorMessages.OK, ErrorMessages.MSG_OK);
            try
            {
                if (_dicOptionSeriesBlocked==null)
                {
                    ret = _fillResponse(ErrorMessages.ERR_CODE_DATA_NOT_LOADED, ErrorMessages.ERR_DATA_NOT_LOADED);
                    return ret;
                }

                // Buscar informacoes da regra FatFinger
                OptionBlockInfo optionBlock = null;
                if (null != _dicOptionSeriesBlocked)
                {
                    if (_dicOptionSeriesBlocked.TryGetValue(serie, out optionBlock))
                    {
                        string msg = string.Format(ErrorMessages.ERR_SERIES_OPTION_BLOCKED, serie);
                        ret = _fillResponse(ErrorMessages.ERR_CODE_SERIES_OPTION_BLOCKED, msg);
                        return ret;
                    }
                }
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na validacao de bloqueio de serie de opcao: " + ex.Message, ex);
                ret = _fillResponse(ErrorMessages.ERROR, ex.Message, ex.StackTrace);
                return ret;
            }
        }

        /// <summary>
        /// Permissao de perfil institucional
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public LimitResponse VerifyInstitutionalProfile(int account)
        {
            LimitResponse ret = _fillResponse(ErrorMessages.OK, ErrorMessages.MSG_OK);
            try
            {
                ClientParameterPermissionInfo clientPermission = null;
                if (_dicParameters == null)
                {
                    ret = _fillResponse(ErrorMessages.ERR_CODE_DATA_NOT_LOADED, ErrorMessages.ERR_DATA_NOT_LOADED);
                    return ret;
                }

                // Verificar se existe parametro / permissao do cliente 
                if (!_dicParameters.TryGetValue(account, out clientPermission))
                {
                    ret = _fillResponse(ErrorMessages.ERR_CODE_PARAM_PERM_NOT_FOUND, ErrorMessages.ERR_PARAM_PERM_NOT_FOUND);
                    return ret;
                }
                // Verificar se o perfil institucional esta atribuido
                ParameterPermissionClientInfo item = null;
                item = clientPermission.Permissoes.Find(x => x.Permissao == RiscoPermissoesEnum.PerfilInstitucional);
                if (null != item)
                {
                    string msg = string.Format(ErrorMessages.ERR_INSTITUTIONAL_PROFILE_FOUND, account);
                    ret = _fillResponse(ErrorMessages.ERR_CODE_INSTITUTIONAL_PROFILE_FOUND, msg);
                    return ret;
                }
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na validacao de permissao institucional: " + ex.Message, ex);
                ret = _fillResponse(ErrorMessages.ERROR, ex.Message, ex.StackTrace);
                return ret;
            }
        }


        public ValidarContaRepasseResponse VerifyGiveUpAccount(ValidarContaRepasseRequest req)
        {
            ValidarContaRepasseResponse ret = new ValidarContaRepasseResponse();
            try
            {
                ClientParameterPermissionInfo clientPermission = null;
                if (null== _dicParameters)
                {
                    ret.ValidationResult = false;
                    ret.RejectMessage = ErrorMessages.ERR_DATA_NOT_LOADED;
                    return ret;
                }

                // Verificar se existe parametro / permissao do cliente 
                if (!_dicParameters.TryGetValue(req.Account, out clientPermission))
                {
                    ret.ValidationResult = false;
                    ret.RejectMessage = ErrorMessages.ERR_PARAM_PERM_NOT_FOUND;
                    return ret;
                }
                // Verificar se a permissao de conta de repasse esta atribuida
                ParameterPermissionClientInfo item = null;
                item = clientPermission.Permissoes.Find(x => x.Permissao == RiscoPermissoesEnum.ContaRepasse);
                if (null != item)
                {
                    string msg = string.Format(ErrorMessages.ERR_GIVE_UP_ACCOUNT, req.Account);
                    ret.ValidationResult = true;
                    ret.RejectMessage = msg;
                    return ret;
                }
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na validacao de permissao de conta de repasse: " + ex.Message, ex);
                ret.ValidationResult = false;
                ret.RejectMessage = ex.Message;
                return ret;
            }
        }

        /// <summary>
        /// Verificacao dos limites de operacao
        /// 
        /// </summary>
        /// <param name="account"></param>
        /// <param name="tpLimite"></param>
        /// <param name="valueToCompare"></param>
        /// <returns></returns>
        public LimitResponse VerifyOperatingLimit(int account, TipoLimiteEnum tpLimite, decimal valueToCompare)
        {
            LimitResponse ret = _fillResponse(ErrorMessages.OK, ErrorMessages.MSG_OK);
            try
            {
                if (_dicOperatingLimit == null || _dicOperatingLimit.Count == 0)
                {
                    ret = _fillResponse(ErrorMessages.ERR_CODE_DATA_NOT_LOADED, ErrorMessages.ERR_DATA_NOT_LOADED);
                    return ret;
                }

                // Buscar informacoes da regra de operacao de limites
                List<OperatingLimitInfo> opLimitLst = null;
                if (null != _dicOperatingLimit)
                {
                    if (!_dicOperatingLimit.TryGetValue(account, out opLimitLst))
                    {
                        string msg = string.Format(ErrorMessages.ERR_OPERATING_LIMIT_NOT_FOUND, tpLimite.ToString());
                        ret = _fillResponse(ErrorMessages.ERR_CODE_OPERATING_LIMIT_NOT_FOUND, msg);
                        return ret;
                    }


                    OperatingLimitInfo opLimit = opLimitLst.Find(x => x.TipoLimite == tpLimite);
                    OperatingLimitInfo opLimitDual = opLimitLst.Find(x => x.TipoLimite == _dicDualTipoLimite[tpLimite]);
                    // Valida se existe os 2 lados do limite (ex... compra a vista e venda a vista)
                    // OBS: regra baseada em ProcessarOrdem.cs
                    if (null != opLimit && opLimitDual != null)
                    {
                        if (valueToCompare > opLimit.ValorDisponivel)
                        {
                            ret = _fillResponse(ErrorMessages.ERR_CODE_OPERATING_LIMIT_EXCEEDS, ErrorMessages.ERR_OPERATING_LIMIT_EXCEEDS);
                            return ret;
                        }
                    }
                    else
                    {
                        string msg = string.Format(ErrorMessages.ERR_OPERATING_LIMIT_NOT_FOUND, tpLimite.ToString());
                        ret = _fillResponse(ErrorMessages.ERR_CODE_OPERATING_LIMIT_NOT_FOUND, msg);
                        return ret;
                    }
                    ret.InfoObject = opLimit;
                }
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na validacao no limite operacional: " + ex.Message, ex);
                ret = _fillResponse(ErrorMessages.ERROR, ex.Message, ex.StackTrace);
                return ret;
            }
        }

        public LimitResponse VerifyClientBMFLimit(int account, string symbol, string side, decimal orderQty, decimal originalQty)
        {
            LimitResponse ret = _fillResponse(ErrorMessages.OK, ErrorMessages.MSG_OK);
            try
            {
                if (_dicClientLimitBMF==null || _dicClientLimitBMF.Count==0)
                {
                    ret = _fillResponse(ErrorMessages.ERR_CODE_DATA_NOT_LOADED, ErrorMessages.ERR_DATA_NOT_LOADED);
                    return ret;
                }

                int qtdContrato = 0;
                int qtdInstrumento = 0;
                int idClienteParametroBmf = 0;
                string instrument = string.Empty;
                char stUtilizacaoInstrumento = 'N';

                string sentido = side.Equals("1") ? "C" : "V";

                // Buscar informacoes da regra BMF
                ClientLimitBMFInfo bmfLimit = null;
                if (null != _dicClientLimitBMF)
                {
                    if (!_dicClientLimitBMF.TryGetValue(account, out bmfLimit))
                    {
                        ret = _fillResponse(ErrorMessages.ERR_CODE_BMF_LIMIT_NOT_FOUND, ErrorMessages.ERR_BMF_LIMIT_NOT_FOUND);
                        return ret;
                    }

                    // Contrato base
                    string contract = symbol.Substring(0, 3);
                    ClientLimitContractBMFInfo contractLimit = bmfLimit.ContractLimit.Find(x => x.Contrato == contract && x.Sentido == sentido);
                    if (null == contractLimit)
                    {
                        ret = _fillResponse(ErrorMessages.ERR_CODE_BMF_LIMIT_CONTRACT_EXCEEDS, ErrorMessages.ERR_BMF_LIMIT_CONTRACT_EXCEEDS);
                        return ret;
                    }

                    // Contrato pai
                    idClienteParametroBmf = contractLimit.IdClienteParametroBMF;
                    qtdContrato = contractLimit.QuantidadeDisponivel;

                    //int QuantidadeContrato = contractLimit.QuantidadeDisponivel;
                    if (originalQty > contractLimit.QuantidadeMaximaOferta)
                    {
                        string msg = string.Format(ErrorMessages.ERR_BMF_LIMIT_QTD_EXCEEDS, contractLimit.QuantidadeMaximaOferta);
                        ret = _fillResponse(ErrorMessages.ERR_CODE_BMF_LIMIT_QTD_EXCEEDS, msg);
                        return ret;
                    }

                    if (orderQty > contractLimit.QuantidadeDisponivel)
                    {
                        ret = _fillResponse(ErrorMessages.ERR_CODE_BMF_LIMIT_OPERATING_EXCEEDS_CONTRACT, ErrorMessages.ERR_BMF_LIMIT_OPERATING_EXCEEDS_CONTRACT);
                        return ret;
                    }

                    if (bmfLimit.InstrumentLimit.Count > 0)
                    {
                        ClientLimitInstrumentBMFInfo instLimit = bmfLimit.InstrumentLimit.Find(x => x.Instrumento == symbol && x.Sentido == sentido);
                        if (null != instLimit)
                        {
                            if (originalQty > instLimit.QuantidadeMaximaOferta)
                            {
                                string msg = string.Format(ErrorMessages.ERR_BMF_LIMIT_QTD_EXCEEDS, instLimit.QuantidadeMaximaOferta);
                                ret = _fillResponse(ErrorMessages.ERR_CODE_BMF_LIMIT_QTD_EXCEEDS, msg);
                                return ret;
                            }

                            if (orderQty > instLimit.QtDisponivel)
                            {
                                ret = _fillResponse(ErrorMessages.ERR_CODE_BMF_LIMIT_OPERATING_EXCEEDS_INST, ErrorMessages.ERR_BMF_LIMIT_OPERATING_EXCEEDS_INST);
                                return ret;
                            }
                        }
                        //else
                        //{
                        //    ret = _fillResponse(ErrorMessages.ERR_CODE_BMF_LIMIT_INSTRUMENT_NOT_FOUND, ErrorMessages.ERR_BMF_LIMIT_INSTRUMENT_NOT_FOUND);
                        //    return ret;
                        //}
                    }
                }
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na validacao no limite operacional: " + ex.Message, ex);
                ret = _fillResponse(ErrorMessages.ERROR, ex.Message, ex.StackTrace);
                return ret;
            }
        }

        private LimitResponse _fillResponse(int code, string msg, string stack = "")
        {
            LimitResponse ret = new LimitResponse();
            ret.ErrorMessage = msg;
            ret.ErrorCode = code;
            ret.ErrorStack = stack;
            return ret;
        }

        /// <summary>
        /// Busca a leitura a partir do parametro para conta bovespa e bmf diferentes
        /// Caso seja passado a conta bovespa, retorna bmf. Caso passe bmf, retorna o codigo bovespa
        /// 
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        /// marketAcc = 0 -> conta retorno eh bovespa que eh igual a bmf
        /// marketAcc = 1 -> conta retorno eh bovespa
        /// marketAcc = 2 -> conta retorno eh bmf
        public int ParseAccount(int account, out int marketAcc)
        {

            int conta;
            marketAcc = AccType.BVSP_BMF;
            // Bovespa e retorna bmf
            if (_dicAccBvspBmf.TryGetValue(account, out conta))
            {
                marketAcc = AccType.BMF;
                return conta;
            }
            if (_dicAccBvspBmf.ContainsValue(account))
            {
                KeyValuePair<int, int> key = _dicAccBvspBmf.FirstOrDefault(x => x.Value == account);
                conta = key.Key;
                marketAcc = AccType.BVSP;
                return conta;
            }
            conta = account;
            return conta;
        }


    }
}
