using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using Gradual.Core.Spider.OrderFixProcessing.Lib.Dados;
using System.Collections.Concurrent;
using Gradual.Spider.Acompanhamento4Socket.Lib.Dados;
using Gradual.Spider.Acompanhamento4Socket.Lib.Util;
using log4net;

namespace Gradual.Spider.SupervisorRisco.DB.Lib
{
    public class DbAc4Socket
    {
        #region log4net declaration
        public static readonly log4net.ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region Private Variables
        private SqlConnection _sqlConn;
        private SqlCommand _sqlCommand;
        string _strConnectionStringDefault;
        #endregion

        #region Properties
        public bool ConexaoAberta
        {
            get
            {
                return (_sqlConn != null && _sqlConn.State == System.Data.ConnectionState.Open);
            }
        }
        #endregion 

        #region Connection functions
        private void _abrirConexao()
        {
            this._abrirConexao(_strConnectionStringDefault);
        }

        private void _abrirConexao(string strConnectionString)
        {
            if (!this.ConexaoAberta)
            {
                _sqlConn = new SqlConnection(strConnectionString);
                _sqlConn.Open();
            }
        }

        private void _fecharConexao()
        {
            try
            {
                _sqlConn.Close();
                _sqlConn.Dispose();
            }
            catch { }
        }
        #endregion


        public DbAc4Socket()
        {
            _sqlConn = null;
            _sqlCommand = null;
            _strConnectionStringDefault = ConfigurationManager.ConnectionStrings["GradualSpider"].ConnectionString;
        }

        ~DbAc4Socket()
        {
            if (null != _sqlCommand)
            {
                _sqlCommand.Dispose();
                _sqlCommand = null;
            }
            if (null != _sqlConn)
            {
                _fecharConexao();
            }
        }

        public bool LoadOrderSnapshot(DateTime dtReg, out ConcurrentDictionary<int, ConcurrentDictionary<int, SpiderOrderInfo>> orders,
                                      out ConcurrentDictionary<int, List<SpiderOrderDetailInfo>> orderdetails)
        {
            try
            {
                // Carregar as informacoes de Integracao para compor os registros
                Dictionary<int, SpiderIntegration> dic = new Dictionary<int, SpiderIntegration>();
                dic = this.LoadIntegrationInfo();

                Dictionary<int, int> dicAux = new Dictionary<int, int>();

                orders = new ConcurrentDictionary<int, ConcurrentDictionary<int, SpiderOrderInfo>>();
                orderdetails = new ConcurrentDictionary<int, List<SpiderOrderDetailInfo>>();
                SqlDataAdapter lAdapter;
                _abrirConexao(_strConnectionStringDefault);
                DataSet lDataSet = new DataSet();

                _sqlCommand = new SqlCommand("prc_spider_ac_ordens4socket", _sqlConn);
                _sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                _sqlCommand.Parameters.Add(new SqlParameter("@DataRegistro", dtReg));
                

                lAdapter = new SqlDataAdapter(_sqlCommand);
                lAdapter.SelectCommand.CommandTimeout = 0;
                lAdapter.Fill(lDataSet);
                int currentAccount = -1;
                int currentOrderID = 0;
                ConcurrentDictionary<int, SpiderOrderInfo> currentItem = null;
                SpiderOrderInfo currentOrder = null;
                logger.Info("=================> Inicio montagem memoria...");
                if (lDataSet.Tables.Count < 2)
                    throw new Exception("Numero invalido de tables na query!!");
                int len = lDataSet.Tables[0].Rows == null ? 0 : lDataSet.Tables[0].Rows.Count;

                for (int i = 0; i < len; i++)
                {
                    DataRow lRow = lDataSet.Tables[0].Rows[i];
                    int acc = lRow["Account"].DBToInt32();
                    int ordid = lRow["OrderID"].DBToInt32();

                    if (currentAccount != acc)
                    {
                        ConcurrentDictionary<int, SpiderOrderInfo> item = new ConcurrentDictionary<int, SpiderOrderInfo>();
                        orders.AddOrUpdate(acc, item, (key, oldValue) => item);
                        currentItem = item;
                    }
                    if (currentOrderID != ordid)
                    {
                        currentOrder = new SpiderOrderInfo();
                        currentOrder.OrderID = lRow["OrderID"].DBToInt32();
                        currentOrder.OrigClOrdID = lRow["OrigClOrdID"].DBToString();
                        currentOrder.ExchangeNumberID = lRow["ExchangeNumberID"].DBToString();
                        currentOrder.ClOrdID = lRow["ClOrdID"].DBToString();
                        currentOrder.Account = lRow["Account"].DBToInt32();
                        currentOrder.Symbol = lRow["Symbol"].DBToString();
                        currentOrder.SecurityExchangeID = lRow["SecurityExchangeID"].DBToString();
                        currentOrder.StopStartID = lRow["StopStartID"].DBToInt32();
                        currentOrder.OrdTypeID = lRow["OrdTypeID"].DBToString();
                        currentOrder.OrdStatus = lRow["OrdStatusID"].DBToInt32();
                        currentOrder.RegisterTime = lRow["RegisterTime"].DBToDateTime();
                        currentOrder.TransactTime = lRow["TransactTime"].DBToDateTime();
                        currentOrder.ExpireDate = lRow["ExpireDate"].DBToDateTime();
                        currentOrder.TimeInForce = lRow["TimeInForce"].DBToString();
                        currentOrder.ChannelID = lRow["ChannelID"].DBToInt32();
                        currentOrder.ExecBroker = lRow["ExecBroker"].DBToString();
                        currentOrder.Side = lRow["Side"].DBToInt32();
                        currentOrder.OrderQty = lRow["OrderQty"].DBToInt32();
                        currentOrder.OrderQtyRemaining = lRow["OrderQtyRemaining"].DBToInt32();
                        currentOrder.OrderQtyMin = lRow["MinQty"].DBToDecimal();
                        currentOrder.OrderQtyApar = lRow["MaxFloor"].DBToDecimal();
                        currentOrder.Price = lRow["Price"].DBToDecimal();
                        currentOrder.StopPx = lRow["StopPx"].DBToDecimal();
                        currentOrder.CumQty = lRow["CumQty"].DBToInt32();
                        currentOrder.FixMsgSeqNum = lRow["FixMsgSeqNum"].DBToInt32();
                        currentOrder.SystemID = lRow["SystemID"].DBToString();
                        currentOrder.Memo = lRow["Memo"].DBToString();
                        currentOrder.SessionID = lRow["SessionID"].DBToString();
                        currentOrder.SessionIDOriginal = lRow["SessionIDOrigin"].DBToString();
                        currentOrder.IdFix = lRow["IdSession"].DBToInt32();
                        currentOrder.MsgFix = lRow["MsgFix"].DBToString();
                        currentOrder.Msg42Base64 = lRow["Msg42Base64"].DBToString();
                        currentOrder.HandlInst = lRow["HandlInst"].DBToString();
                        SpiderIntegration integ = null;
                        if (dic.TryGetValue(currentOrder.IdFix, out integ))
                        {
                            currentOrder.IntegrationName = integ.IntegrationName;
                            currentOrder.Exchange = integ.Exchange;
                        }
                        currentItem.AddOrUpdate(ordid, currentOrder, (key, oldValue) => currentOrder);
                        dicAux.Add(ordid, acc);
                    }
                    currentAccount = acc;
                    currentOrderID = ordid;
                }

                // Processar os details e adicionar no order id correto
                int currentOrderID_Det = -1;

                ConcurrentDictionary<int, List<SpiderOrderDetailInfo>> currentDetail = new ConcurrentDictionary<int, List<SpiderOrderDetailInfo>>();
                List<SpiderOrderDetailInfo> lstDetail = new List<SpiderOrderDetailInfo>();
                //SpiderOrderInfo currentSpiderOrder = null;
                bool firstReg = true;
                len = lDataSet.Tables[1].Rows == null ? 0 : lDataSet.Tables[1].Rows.Count;
                for (int i = 0; i < len; i++)
                {
                    DataRow lRow2 = lDataSet.Tables[1].Rows[i];
                    int ordid_det = lRow2["OrderID"].DBToInt32();

                    // Se diferente, mudou de registro e deve buscar o novo account, dicionario de ordens e afins
                    if (currentOrderID_Det != ordid_det && currentOrderID_Det != -1)
                    {
                        if (ordid_det == 0)
                        {
                            logger.Error("Order ID ZERADO!!!");
                            continue;
                        }
                        int accountAux = -1;
                        if (!dicAux.TryGetValue(currentOrderID_Det, out accountAux))
                        {
                            logger.Error("Account NOT FOUND!!! OrderID: " + currentOrderID_Det);
                            continue;
                        }

                        // Adicionar o elemento anterior e zerar a lista
                        orderdetails.AddOrUpdate(currentOrderID_Det, lstDetail, (key, oldValue) => lstDetail);

                        // Calcular o preco medio 
                        decimal avgPxW = Calculator.CalculateWeightedAvgPx(lstDetail);
                        decimal avgPx = Calculator.CalculateAvgPx(lstDetail);
                        // Buscar o order ID para atualizar o order info "header"
                        ConcurrentDictionary<int, SpiderOrderInfo> xx = null;
                        if (orders.TryGetValue(accountAux, out xx))
                        {
                            SpiderOrderInfo xx2 = null;
                            if (xx.TryGetValue(currentOrderID_Det, out xx2))
                            {
                                xx2.AvgPxW = avgPxW;
                                xx2.AvgPx = avgPx;
                            }
                        }

                        lstDetail = null;
                        lstDetail = new List<SpiderOrderDetailInfo>();
                    }

                    // Processar o detalhe da ordem

                    SpiderOrderDetailInfo detailOrder = new SpiderOrderDetailInfo();
                    detailOrder.OrderDetailID = lRow2["OrderDetailID"].DBToInt32();
                    detailOrder.TransactID = lRow2["TransactID"].DBToString();
                    detailOrder.OrderID = lRow2["OrderID"].DBToInt32();
                    detailOrder.OrderQty = lRow2["OrderQty"].DBToInt32();
                    detailOrder.OrderQtyRemaining = lRow2["OrdQtyRemaining"].DBToInt32();
                    detailOrder.Price = lRow2["Price"].DBToDecimal();
                    detailOrder.StopPx = lRow2["StopPx"].DBToDecimal();
                    detailOrder.OrderStatusID = lRow2["OrderStatusID"].DBToInt32();
                    detailOrder.TransactTime = lRow2["TransactTime"].DBToDateTime();
                    detailOrder.Description = lRow2["Description"].DBToString();
                    detailOrder.TradeQty = lRow2["TradeQty"].DBToInt32();
                    detailOrder.CumQty = lRow2["CumQty"].DBToInt32();
                    detailOrder.FixMsgSeqNum = lRow2["FixMsgSeqNum"].DBToInt32();
                    detailOrder.CxlRejResponseTo = lRow2["CxlRejResponseTo"].DBToString();
                    detailOrder.CxlRejReason = lRow2["CxlRejReason"].DBToInt32();
                    detailOrder.MsgFixDetail = lRow2["MsgFixDetail"].DBToString();

                    // Adicionar o detail no currentOrder
                    lstDetail.Add(detailOrder);

                    //currentSpiderOrder.Details.Add(detailOrder);
                    currentOrderID_Det = ordid_det;
                }

                foreach (KeyValuePair<int, List<SpiderOrderDetailInfo>> itemDet in orderdetails)
                {
                    SpiderOrderInfo aux = null;
                    foreach (KeyValuePair<int, ConcurrentDictionary<int, SpiderOrderInfo>> itemOrd in orders)
                    {
                        // Procurar SpiderOrderInfo em cada conta
                        if (itemOrd.Value.TryGetValue(itemDet.Key, out aux))
                            break;
                    }

                    if (aux != null && aux.Details != null)
                        aux.Details.Clear();
                    else
                        logger.Error("Erro na montagem da memoria. OrderID " + itemDet.Key);
                    aux.Details.AddRange(itemDet.Value);
                }
                logger.Info("=========================> Fim de montagem de memoria..");

                lDataSet.Clear();
                lDataSet.Dispose();
                lAdapter.Dispose();
                _fecharConexao();
                return true;
            }
            catch (Exception ex)
            {
                _fecharConexao();
                logger.Error("LoadOrderSnapshot(): " + ex.Message, ex);
                orders = null;
                orderdetails = null;
                return false;
            }
        }

        public Dictionary<int, SpiderIntegration> LoadIntegrationInfo()
        {
            try
            {
                Dictionary<int, SpiderIntegration> ret = new Dictionary<int, SpiderIntegration>();
                SqlDataAdapter lAdapter;
                _abrirConexao(_strConnectionStringDefault);
                DataSet lDataSet = new DataSet();

                _sqlCommand = new SqlCommand("prc_spider_fix_integration_name_lst", _sqlConn);
                _sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;

                lAdapter = new SqlDataAdapter(_sqlCommand);
                lAdapter.Fill(lDataSet);

                foreach (DataRow lRow in lDataSet.Tables[0].Rows)
                {
                    int idSessaoFix = lRow["IdSessaoFix"].DBToInt32();
                    SpiderIntegration item = new SpiderIntegration();
                    item.IntegrationId = lRow["id_integration"].DBToInt32();
                    item.IntegrationName = lRow["IntegrationName"].DBToString();
                    item.Exchange = lRow["Bolsa"].DBToString();
                    ret.Add(idSessaoFix, item);
                }
                _fecharConexao();
                return ret;
            }
            catch (Exception ex)
            {
                _fecharConexao();
                logger.Error("LoadIntegrationInfo() " + ex.Message, ex);
                return null;
            }
        }
    }
}
