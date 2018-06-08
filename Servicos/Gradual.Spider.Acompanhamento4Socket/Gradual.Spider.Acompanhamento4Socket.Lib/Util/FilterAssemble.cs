using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Linq.Expressions;
using Gradual.Core.Spider.OrderFixProcessing.Lib.Dados;
using Gradual.Spider.Acompanhamento4Socket.Lib.Dados;
using System.Collections.Concurrent;

namespace Gradual.Spider.Acompanhamento4Socket.Lib.Util
{
    public static class FilterAssemble
    {
        #region log4net declaration
        public static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region Predicate SpiderOrderInfo
        public static Expression<Func<SpiderOrderInfo, bool>> AssembleOrderFilter(FilterSpiderOrder filter)
        {
            try
            {
                var predicate = PredicateBuilder.True<SpiderOrderInfo>();
                var predAux = _predicateInt("OrderID", filter.OrderID);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateString("OrigClOrdID", filter.OrigClOrdID);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateString("ExchangeNumberID", filter.ExchangeNumberID);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateString("ClOrdID", filter.ClOrdID);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateInt("Account", filter.Account);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateString("Symbol", filter.Symbol);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateString("SecurityExchangeID", filter.SecurityExchangeID);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateInt("StopStartID", filter.StopStartID);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateString("OrdTypeID", filter.OrdTypeID);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateInt("OrdStatus", filter.OrdStatus);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateDate("RegisterTime", filter.RegisterTime);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateDate("TransactTime", filter.TransactTime);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateDate("ExpireDate", filter.ExpireDate);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateString("TimeInForce", filter.TimeInForce);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateInt("ChannelID", filter.ChannelID);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateString("ExecBroker", filter.ExecBroker);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateInt("Side", filter.Side);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateInt("OrderQty", filter.OrderQty);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateDecimal("OrderQtyMin", filter.OrderQtyMin);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateDecimal("OrderQtyApar", filter.OrderQtyApar);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateInt("OrderQtyRemaining", filter.OrderQtyRemaining);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateDecimal("Price", filter.Price);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateDecimal("StopPx", filter.StopPx);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateString("Description", filter.Description);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateString("SystemID", filter.SystemID);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateString("Memo", filter.Memo);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateInt("CumQty", filter.CumQty);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateInt("FixMsgSeqNum", filter.FixMsgSeqNum);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateString("SessionID", filter.SessionID);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateString("SessionIDOriginal", filter.SessionIDOriginal);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateInt("IdFix", filter.IdFix);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateString("MsgFix", filter.MsgFix);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateString("Msg42Base64", filter.Msg42Base64);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateString("HandlInst", filter.HandlInst);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateString("IntegrationName", filter.IntegrationName);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateString("Exchange", filter.Exchange);
                if (null != predAux) predicate = predicate.And(predAux);
                
                // Retirar todos os details de cada ordem
                // predAux = _predicateOrderDetail();
                // if (null != predAux) predicate = predicate.And(predAux);

                return predicate;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na montagem do filtro: " + ex.Message, ex);
                return null;
            }
        }


        private static Expression<Func<SpiderOrderInfo, bool>> _predicateString(string strPropName, FilterStringVal strVal)
        {
            try
            {
                var predRet = PredicateBuilder.True<SpiderOrderInfo>();
                var predAux = PredicateBuilder.False<SpiderOrderInfo>();
                var prop = typeof(SpiderOrderInfo).GetProperty(strPropName);
                if (null == prop)
                {
                    predRet = null;
                    return predRet;
                }
                switch (strVal.Compare)
                {
                    case TypeCompare.EQUAL:
                        // predRet = predRet.And(x => (x.GetType().GetProperty(strPropName).GetValue(x, null) as string) == strVal.Value);
                        predRet = predRet.And(x =>  ((string) prop.GetValue(x, null)).Equals(strVal.Value));
                        break;
                    case TypeCompare.IN_STR:
                        predRet = predRet.And(x =>  ((string) prop.GetValue(x, null)).IndexOf(strVal.Value)>=0);
                        break;
                    case TypeCompare.LIST_VALUE:
                        {
                            
                            int len = strVal.ListValue.Count();
                            for (int i = 0; i < len; i++)
                            {
                                string valor = strVal.ListValue[i];
                                predAux = predAux.Or(x => ((string)prop.GetValue(x, null)).Equals(valor));
                            }
                            if (len > 0)
                                predRet = predRet.And(predAux);
                        }
                        break;
                    default:
                        predRet = null;
                        break;
                }
                return predRet;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na montagem do predicate string: " + ex.Message, ex);
                return null;
            }
        }

        private static Expression<Func<SpiderOrderInfo, bool>> _predicateInt(string strPropName, FilterIntVal intVal)
        {
            try
            {
                var predRet = PredicateBuilder.True<SpiderOrderInfo>();
                var predAux = PredicateBuilder.False<SpiderOrderInfo>();
                var prop = typeof(SpiderOrderInfo).GetProperty(strPropName);
                if (null == prop)
                {
                    predRet = null;
                    return predRet;
                }

                switch (intVal.Compare)
                {
                    case TypeCompare.EQUAL:
                        // predRet = predRet.And(x => ( (int) x.GetType().GetProperty(strPropName).GetValue(x, null)  == intVal.Value)) ;
                        predRet = predRet.And(x => ((int)prop.GetValue(x, null) == intVal.Value));
                        break;
                    case TypeCompare.GREATER:
                        predRet = predRet.And(x => ((int)prop.GetValue(x, null) > intVal.Value));
                        break;
                    case TypeCompare.GREATER_OR_EQUAL:
                        predRet = predRet.And(x => ((int)prop.GetValue(x, null) >= intVal.Value));
                        break;
                    case TypeCompare.LOWER:
                        predRet = predRet.And(x => ((int)prop.GetValue(x, null) < intVal.Value));
                        break;
                    case TypeCompare.LOWER_OR_EQUAL:
                        predRet = predRet.And(x => ((int)prop.GetValue(x, null) <= intVal.Value));
                        break;
                    case TypeCompare.BETWEEN:
                        predRet = predRet.And(x => ((int)prop.GetValue(x, null) > intVal.Value));
                        predRet = predRet.And(x => ((int)prop.GetValue(x, null) < intVal.Value2));
                        break;
                    case TypeCompare.BETWEEN_EQUAL:
                        predRet = predRet.And(x => ((int)prop.GetValue(x, null) >= intVal.Value));
                        predRet = predRet.And(x => ((int)prop.GetValue(x, null) <= intVal.Value2));
                        break;
                    case TypeCompare.GREATER_EQUAL_AND_LOWER:
                        predRet = predRet.And(x => ((int)prop.GetValue(x, null) >= intVal.Value));
                        predRet = predRet.And(x => ((int)prop.GetValue(x, null) < intVal.Value2));
                        break;
                    case TypeCompare.GREATER_AND_LOWER_EQUAL:
                        predRet = predRet.And(x => ((int)prop.GetValue(x, null) > intVal.Value));
                        predRet = predRet.And(x => ((int)prop.GetValue(x, null) <= intVal.Value2));
                        break;
                    case TypeCompare.LIST_VALUE:
                        {
                            
                            int len = intVal.ListValue.Count();
                            for (int i = 0; i < len; i++)
                            {
                                int valor = intVal.ListValue[i];
                                predAux = predAux.Or(x => ((int)prop.GetValue(x, null) == valor));
                                // predAux = predAux.Or(x => x.OrdStatus.Eq == intVal.ListValue[i]);
                            }
                            if (len > 0)
                                predRet = predRet.And(predAux);
                        }
                        break;
                    default:
                        predRet = null;
                        break;
                }
                return predRet;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na montagem do predicate Int: " + ex.Message, ex);
                return null;
            }
        }

        private static Expression<Func<SpiderOrderInfo, bool>> _predicateDecimal(string strPropName, FilterDecimalVal decVal)
        {
            try
            {
                var predRet = PredicateBuilder.True<SpiderOrderInfo>();
                var predAux = PredicateBuilder.False<SpiderOrderInfo>();
                var prop = typeof(SpiderOrderInfo).GetProperty(strPropName);
                if (null == prop)
                {
                    predRet = null;
                    return predRet;
                }

                switch (decVal.Compare)
                {
                    case TypeCompare.EQUAL:
                        predRet = predRet.And(x => ((Decimal) prop.GetValue(x, null) == decVal.Value));
                        break;
                    case TypeCompare.GREATER:
                        predRet = predRet.And(x => ((Decimal)prop.GetValue(x, null) > decVal.Value));
                        break;
                    case TypeCompare.GREATER_OR_EQUAL:
                        predRet = predRet.And(x => ((Decimal)prop.GetValue(x, null) >= decVal.Value));
                        break;
                    case TypeCompare.LOWER:
                        predRet = predRet.And(x => ((Decimal)prop.GetValue(x, null) < decVal.Value));
                        break;
                    case TypeCompare.LOWER_OR_EQUAL:
                        predRet = predRet.And(x => ((Decimal)prop.GetValue(x, null) <= decVal.Value));
                        break;
                    case TypeCompare.BETWEEN:
                        predRet = predRet.And(x => ((Decimal)prop.GetValue(x, null) > decVal.Value));
                        predRet = predRet.And(x => ((Decimal)prop.GetValue(x, null) < decVal.Value2));
                        break;
                    case TypeCompare.BETWEEN_EQUAL:
                        predRet = predRet.And(x => ((Decimal)prop.GetValue(x, null) >= decVal.Value));
                        predRet = predRet.And(x => ((Decimal)prop.GetValue(x, null) <= decVal.Value2));
                        break;
                    case TypeCompare.GREATER_AND_LOWER_EQUAL:
                        predRet = predRet.And(x => ((Decimal)prop.GetValue(x, null) > decVal.Value));
                        predRet = predRet.And(x => ((Decimal)prop.GetValue(x, null) <= decVal.Value2));
                        break;
                    case TypeCompare.GREATER_EQUAL_AND_LOWER:
                        predRet = predRet.And(x => ((Decimal)prop.GetValue(x, null) >= decVal.Value));
                        predRet = predRet.And(x => ((Decimal)prop.GetValue(x, null) < decVal.Value2));
                        break;
                    case TypeCompare.LIST_VALUE:
                        {
                            int len = decVal.ListValue.Count();
                            for (int i = 0; i < len; i++)
                            {
                                decimal valor = decVal.ListValue[i];
                                predAux = predAux.Or(x => ((Decimal)prop.GetValue(x, null) == valor));
                            }
                            if (len > 0)
                                predRet = predRet.And(predAux);
                        }
                        break;
                    default:
                        predRet = null;
                        break;
                }
                return predRet;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na montagem do predicate Decimal: " + ex.Message, ex);
                return null;
            }
        }


        private static Expression<Func<SpiderOrderInfo, bool>> _predicateDate(string strPropName, FilterDateVal dateVal)
        {
            try
            {
                var predRet = PredicateBuilder.True<SpiderOrderInfo>();
                var predAux = PredicateBuilder.False<SpiderOrderInfo>();
                var prop = typeof(SpiderOrderInfo).GetProperty(strPropName);
                if (null == prop)
                {
                    predRet = null;
                    return predRet;
                }

                switch (dateVal.Compare)
                {
                    case TypeCompare.EQUAL:
                        predRet = predRet.And(x => ((DateTime) prop.GetValue(x, null) == dateVal.Value));
                        break;
                    case TypeCompare.GREATER:
                        predRet = predRet.And(x => ((DateTime)prop.GetValue(x, null) > dateVal.Value));
                        break;
                    case TypeCompare.GREATER_OR_EQUAL:
                        predRet = predRet.And(x => ((DateTime)prop.GetValue(x, null) >= dateVal.Value));
                        break;
                    case TypeCompare.LOWER:
                        predRet = predRet.And(x => ((DateTime)prop.GetValue(x, null) < dateVal.Value));
                        break;
                    case TypeCompare.LOWER_OR_EQUAL:
                        predRet = predRet.And(x => ((DateTime)prop.GetValue(x, null) <= dateVal.Value));
                        break;
                    case TypeCompare.BETWEEN:
                        predRet = predRet.And(x => ((DateTime)prop.GetValue(x, null) > dateVal.Value));
                        predRet = predRet.And(x => ((DateTime)prop.GetValue(x, null) < dateVal.Value2));
                        break;
                    case TypeCompare.BETWEEN_EQUAL:
                        predRet = predRet.And(x => ((DateTime)prop.GetValue(x, null) >= dateVal.Value));
                        predRet = predRet.And(x => ((DateTime)prop.GetValue(x, null) <= dateVal.Value2));
                        break;
                    case TypeCompare.GREATER_AND_LOWER_EQUAL:
                        predRet = predRet.And(x => ((DateTime)prop.GetValue(x, null) > dateVal.Value));
                        predRet = predRet.And(x => ((DateTime)prop.GetValue(x, null) <= dateVal.Value2));
                        break;
                    case TypeCompare.GREATER_EQUAL_AND_LOWER:
                        predRet = predRet.And(x => ((DateTime)prop.GetValue(x, null) >= dateVal.Value));
                        predRet = predRet.And(x => ((DateTime)prop.GetValue(x, null) < dateVal.Value2));
                        break;
                    case TypeCompare.LIST_VALUE:
                        {
                            
                            int len = dateVal.ListValue.Count();
                            for (int i = 0; i < len; i++)
                            {
                                DateTime valor = dateVal.ListValue[i];
                                predAux = predAux.Or(x => ((DateTime)prop.GetValue(x, null) == valor));
                            }
                            if (len > 0 )
                                predRet = predRet.And(predAux);
                        }
                        break;
                    default:
                        predRet = null;
                        break;
                }
                return predRet;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na montagem do predicate Date: " + ex.Message, ex);
                return null;
            }
        }
        #endregion

        #region Predicate SpiderOrderDetailInfo
        private static Expression<Func<SpiderOrderInfo, bool>> _predicateOrderDetail()
        {
            try
            {
                var predRet = PredicateBuilder.True<SpiderOrderInfo>();
                predRet = predRet.And(x => x.Details.RemoveAll(y => y.OrderStatusID >= 0) >= 0);

                //predRet = predRet.And(x => x.Details = x.Details.Where(y=> y.OrderStatusID == 0 || y.OrderStatusID ==1 ||
                //                                            y.OrderStatusID == 2 || y.OrderStatusID == 4 || y.OrderStatusID == 5 || y.OrderStatusID == 8)
                //                                            .Select().ToList().Count >= 0);

                return predRet;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na montagem do predicate OrderDetail: " + ex.Message, ex);
                return null;
            }

        }
        /*
        public static Expression<Func<SpiderOrderDetailInfo, bool>> AssembleOrderDetailFilter()
        {
            try
            {
                var predicate = PredicateBuilder.True<SpiderOrderDetailInfo>();
                predicate = predicate.And(x => (x.OrderStatusID != 100) && (x.OrderStatusID != 101) && (x.OrderStatusID != 102));
                return predicate;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na montagem do filtro OrderDetail: " + ex.Message, ex);
                return null;
            }
        }
         */
        #endregion

        /*
        #region Predicate Dictionary
        public static Expression<Func<ConcurrentDictionary<int, SpiderOrderInfo>, bool>> AssembleOrderDictFilter(FilterSpiderOrder filter)
        {
            try
            {
                var predicate = PredicateBuilder.True<ConcurrentDictionary<int, SpiderOrderInfo>>();
                var predAux = _predicateDictInt("OrderID", filter.OrderID);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateDictString("OrigClOrdID", filter.OrigClOrdID);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateDictString("ExchangeNumberID", filter.ExchangeNumberID);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateDictString("ClOrdID", filter.ClOrdID);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateDictInt("Account", filter.Account);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateDictString("Symbol", filter.Symbol);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateDictString("SecurityExchangeID", filter.SecurityExchangeID);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateDictInt("StopStartID", filter.StopStartID);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateDictString("OrdTypeID", filter.OrdTypeID);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateDictInt("OrdStatus", filter.OrdStatus);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateDictDate("RegisterTime", filter.RegisterTime);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateDictDate("TransactTime", filter.TransactTime);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateDictDate("ExpireDate", filter.ExpireDate);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateDictString("TimeInForce", filter.TimeInForce);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateDictInt("ChannelID", filter.ChannelID);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateDictString("ExecBroker", filter.ExecBroker);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateDictInt("Side", filter.Side);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateDictInt("OrderQty", filter.OrderQty);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateDictDecimal("OrderQtyMin", filter.OrderQtyMin);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateDictDecimal("OrderQtyApar", filter.OrderQtyApar);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateDictInt("OrderQtyRemaining", filter.OrderQtyRemaining);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateDictDecimal("Price", filter.Price);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateDictDecimal("StopPx", filter.StopPx);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateDictString("Description", filter.Description);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateDictString("SystemID", filter.SystemID);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateDictString("Memo", filter.Memo);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateDictInt("CumQty", filter.CumQty);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateDictInt("FixMsgSeqNum", filter.FixMsgSeqNum);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateDictString("SessionID", filter.SessionID);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateDictString("SessionIDOriginal", filter.SessionIDOriginal);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateDictInt("IdFix", filter.IdFix);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateDictString("MsgFix", filter.MsgFix);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateDictString("Msg42Base64", filter.Msg42Base64);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateDictString("HandlInst", filter.HandlInst);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateDictString("IntegrationName", filter.IntegrationName);
                if (null != predAux) predicate = predicate.And(predAux);
                predAux = _predicateDictString("Exchange", filter.Exchange);
                if (null != predAux) predicate = predicate.And(predAux);

                // Retirar todos os details de cada ordem
                // predAux = _predicateOrderDetail();
                // if (null != predAux) predicate = predicate.And(predAux);

                return predicate;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na montagem do filtro: " + ex.Message, ex);
                return null;
            }
        }

        private static Expression<Func<ConcurrentDictionary<int, SpiderOrderInfo>, bool>> _predicateDictString(string strPropName, FilterStringVal strVal)
        {
            try
            {
                var predRet = PredicateBuilder.True<ConcurrentDictionary<int, SpiderOrderInfo>>();
                Type[] arguments = typeof(ConcurrentDictionary<int, SpiderOrderInfo>).GetGenericArguments();
                var prop = arguments[1].GetProperty(strPropName);
                if (null == prop)
                {
                    predRet = null;
                    return predRet;
                }
                switch (strVal.Compare)
                {
                    case TypeCompare.EQUAL:
                        // predRet = predRet.And(x => (x.GetType().GetProperty(strPropName).GetValue(x, null) as string) == strVal.Value);
                        predRet = predRet.And(x => ((string)prop.GetValue(x, null)).Equals(strVal.Value));
                        break;
                    case TypeCompare.IN_STR:
                        predRet = predRet.And(x => ((string)prop.GetValue(x, null)).IndexOf(strVal.Value) >= 0);
                        break;
                    case TypeCompare.LIST_VALUE:
                        {
                            var predAux = PredicateBuilder.False<ConcurrentDictionary<int, SpiderOrderInfo>>();
                            int len = strVal.ListValue.Count();
                            for (int i = 0; i < len; i++)
                            {
                                predAux = predAux.Or(x => ((string)prop.GetValue(x, null)).Equals(strVal.ListValue[i]));
                            }
                            predRet = predRet.And(predAux);
                        }
                        break;
                    default:
                        predRet = null;
                        break;
                }
                return predRet;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na montagem do predicate string: " + ex.Message, ex);
                return null;
            }
        }

        private static Expression<Func<ConcurrentDictionary<int, SpiderOrderInfo>, bool>> _predicateDictInt(string strPropName, FilterIntVal intVal)
        {
            try
            {
                var predRet = PredicateBuilder.True<ConcurrentDictionary<int, SpiderOrderInfo>>();
                Type[] arguments = typeof(ConcurrentDictionary<int, SpiderOrderInfo>).GetGenericArguments();
                var prop = arguments[1].GetProperty(strPropName);
                if (null == prop)
                {
                    predRet = null;
                    return predRet;
                }

                switch (intVal.Compare)
                {
                    case TypeCompare.EQUAL:
                        // predRet = predRet.And(x => ( (int) x.GetType().GetProperty(strPropName).GetValue(x, null)  == intVal.Value)) ;
                        predRet = predRet.And(x => ((int)prop.GetValue(x, null) == intVal.Value));
                        break;
                    case TypeCompare.GREATER:
                        predRet = predRet.And(x => ((int)prop.GetValue(x, null) > intVal.Value));
                        break;
                    case TypeCompare.GREATER_OR_EQUAL:
                        predRet = predRet.And(x => ((int)prop.GetValue(x, null) >= intVal.Value));
                        break;
                    case TypeCompare.LOWER:
                        predRet = predRet.And(x => ((int)prop.GetValue(x, null) < intVal.Value));
                        break;
                    case TypeCompare.LOWER_OR_EQUAL:
                        predRet = predRet.And(x => ((int)prop.GetValue(x, null) <= intVal.Value));
                        break;
                    case TypeCompare.BETWEEN:
                        predRet = predRet.And(x => ((int)prop.GetValue(x, null) > intVal.Value));
                        predRet = predRet.And(x => ((int)prop.GetValue(x, null) < intVal.Value2));
                        break;
                    case TypeCompare.BETWEEN_EQUAL:
                        predRet = predRet.And(x => ((int)prop.GetValue(x, null) >= intVal.Value));
                        predRet = predRet.And(x => ((int)prop.GetValue(x, null) <= intVal.Value2));
                        break;
                    case TypeCompare.GREATER_EQUAL_AND_LOWER:
                        predRet = predRet.And(x => ((int)prop.GetValue(x, null) >= intVal.Value));
                        predRet = predRet.And(x => ((int)prop.GetValue(x, null) < intVal.Value2));
                        break;
                    case TypeCompare.GREATER_AND_LOWER_EQUAL:
                        predRet = predRet.And(x => ((int)prop.GetValue(x, null) > intVal.Value));
                        predRet = predRet.And(x => ((int)prop.GetValue(x, null) <= intVal.Value2));
                        break;
                    case TypeCompare.LIST_VALUE:
                        {
                            var predAux = PredicateBuilder.False<ConcurrentDictionary<int, SpiderOrderInfo>>();
                            int len = intVal.ListValue.Count();
                            for (int i = 0; i < len; i++)
                            {
                                predAux = predAux.Or(x => ((int)prop.GetValue(x, null) == intVal.ListValue[i]));
                            }
                            predRet = predRet.And(predAux);
                        }
                        break;
                    default:
                        predRet = null;
                        break;
                }
                return predRet;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na montagem do predicate Int: " + ex.Message, ex);
                return null;
            }
        }

        private static Expression<Func<ConcurrentDictionary<int, SpiderOrderInfo>, bool>> _predicateDictDecimal(string strPropName, FilterDecimalVal decVal)
        {
            try
            {
                var predRet = PredicateBuilder.True<ConcurrentDictionary<int, SpiderOrderInfo>>();
                Type[] arguments = typeof(ConcurrentDictionary<int, SpiderOrderInfo>).GetGenericArguments();
                var prop = arguments[1].GetProperty(strPropName);
                if (null == prop)
                {
                    predRet = null;
                    return predRet;
                }

                switch (decVal.Compare)
                {
                    case TypeCompare.EQUAL:
                        predRet = predRet.And(x => ((Decimal)prop.GetValue(x, null) == decVal.Value));
                        break;
                    case TypeCompare.GREATER:
                        predRet = predRet.And(x => ((Decimal)prop.GetValue(x, null) > decVal.Value));
                        break;
                    case TypeCompare.GREATER_OR_EQUAL:
                        predRet = predRet.And(x => ((Decimal)prop.GetValue(x, null) >= decVal.Value));
                        break;
                    case TypeCompare.LOWER:
                        predRet = predRet.And(x => ((Decimal)prop.GetValue(x, null) < decVal.Value));
                        break;
                    case TypeCompare.LOWER_OR_EQUAL:
                        predRet = predRet.And(x => ((Decimal)prop.GetValue(x, null) <= decVal.Value));
                        break;
                    case TypeCompare.BETWEEN:
                        predRet = predRet.And(x => ((Decimal)prop.GetValue(x, null) > decVal.Value));
                        predRet = predRet.And(x => ((Decimal)prop.GetValue(x, null) < decVal.Value2));
                        break;
                    case TypeCompare.BETWEEN_EQUAL:
                        predRet = predRet.And(x => ((Decimal)prop.GetValue(x, null) >= decVal.Value));
                        predRet = predRet.And(x => ((Decimal)prop.GetValue(x, null) <= decVal.Value2));
                        break;
                    case TypeCompare.GREATER_AND_LOWER_EQUAL:
                        predRet = predRet.And(x => ((Decimal)prop.GetValue(x, null) > decVal.Value));
                        predRet = predRet.And(x => ((Decimal)prop.GetValue(x, null) <= decVal.Value2));
                        break;
                    case TypeCompare.GREATER_EQUAL_AND_LOWER:
                        predRet = predRet.And(x => ((Decimal)prop.GetValue(x, null) >= decVal.Value));
                        predRet = predRet.And(x => ((Decimal)prop.GetValue(x, null) < decVal.Value2));
                        break;
                    case TypeCompare.LIST_VALUE:
                        {
                            var predAux = PredicateBuilder.False<ConcurrentDictionary<int, SpiderOrderInfo>>();
                            int len = decVal.ListValue.Count();
                            for (int i = 0; i < len; i++)
                            {
                                predAux = predAux.Or(x => ((Decimal)prop.GetValue(x, null) == decVal.ListValue[i]));
                            }
                            predRet = predRet.And(predAux);
                        }
                        break;
                    default:
                        predRet = null;
                        break;
                }
                return predRet;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na montagem do predicate Decimal: " + ex.Message, ex);
                return null;
            }
        }


        private static Expression<Func<ConcurrentDictionary<int, SpiderOrderInfo>, bool>> _predicateDictDate(string strPropName, FilterDateVal dateVal)
        {
            try
            {
                var predRet = PredicateBuilder.True<ConcurrentDictionary<int, SpiderOrderInfo>>();
                Type[] arguments = typeof(ConcurrentDictionary<int, SpiderOrderInfo>).GetGenericArguments();
                var prop = arguments[1].GetProperty(strPropName);
                if (null == prop)
                {
                    predRet = null;
                    return predRet;
                }

                switch (dateVal.Compare)
                {
                    case TypeCompare.EQUAL:
                        predRet = predRet.And(x => ((DateTime)prop.GetValue(x, null) == dateVal.Value));
                        break;
                    case TypeCompare.GREATER:
                        predRet = predRet.And(x => ((DateTime)prop.GetValue(x, null) > dateVal.Value));
                        break;
                    case TypeCompare.GREATER_OR_EQUAL:
                        predRet = predRet.And(x => ((DateTime)prop.GetValue(x, null) >= dateVal.Value));
                        break;
                    case TypeCompare.LOWER:
                        predRet = predRet.And(x => ((DateTime)prop.GetValue(x, null) < dateVal.Value));
                        break;
                    case TypeCompare.LOWER_OR_EQUAL:
                        predRet = predRet.And(x => ((DateTime)prop.GetValue(x, null) <= dateVal.Value));
                        break;
                    case TypeCompare.BETWEEN:
                        predRet = predRet.And(x => ((DateTime)prop.GetValue(x, null) > dateVal.Value));
                        predRet = predRet.And(x => ((DateTime)prop.GetValue(x, null) < dateVal.Value2));
                        break;
                    case TypeCompare.BETWEEN_EQUAL:
                        predRet = predRet.And(x => ((DateTime)prop.GetValue(x, null) >= dateVal.Value));
                        predRet = predRet.And(x => ((DateTime)prop.GetValue(x, null) <= dateVal.Value2));
                        break;
                    case TypeCompare.GREATER_AND_LOWER_EQUAL:
                        predRet = predRet.And(x => ((DateTime)prop.GetValue(x, null) > dateVal.Value));
                        predRet = predRet.And(x => ((DateTime)prop.GetValue(x, null) <= dateVal.Value2));
                        break;
                    case TypeCompare.GREATER_EQUAL_AND_LOWER:
                        predRet = predRet.And(x => ((DateTime)prop.GetValue(x, null) >= dateVal.Value));
                        predRet = predRet.And(x => ((DateTime)prop.GetValue(x, null) < dateVal.Value2));
                        break;
                    case TypeCompare.LIST_VALUE:
                        {
                            var predAux = PredicateBuilder.False<ConcurrentDictionary<int, SpiderOrderInfo>>();
                            int len = dateVal.ListValue.Count();
                            for (int i = 0; i < len; i++)
                            {
                                predAux = predAux.Or(x => ((DateTime)prop.GetValue(x, null) == dateVal.ListValue[i]));
                            }
                            predRet = predRet.And(predAux);
                        }
                        break;
                    default:
                        predRet = null;
                        break;
                }
                return predRet;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na montagem do predicate Date: " + ex.Message, ex);
                return null;
            }
        }

        #endregion
        */

        

    }
}
