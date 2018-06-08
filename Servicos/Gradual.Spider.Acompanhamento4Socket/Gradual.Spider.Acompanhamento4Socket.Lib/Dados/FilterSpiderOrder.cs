using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Gradual.Spider.Acompanhamento4Socket.Lib.Dados
{
    [ProtoContract]
    [Serializable]
    public class FilterSpiderOrder
    {
        [ProtoMember(1)]
        public FilterIntVal OrderID { get; set; }
        [ProtoMember(2)]
        public FilterStringVal OrigClOrdID { get; set; }
        [ProtoMember(3)]
        public FilterStringVal ExchangeNumberID { get; set; }
        [ProtoMember(4)]
        public FilterStringVal ClOrdID { get; set; }
        [ProtoMember(5)]
        public FilterIntVal Account { get; set; }
        [ProtoMember(6)]
        public FilterStringVal Symbol { get; set; }
        [ProtoMember(7)]
        public FilterStringVal SecurityExchangeID { get; set; }
        [ProtoMember(8)]
        public FilterIntVal StopStartID { get; set; }
        [ProtoMember(9)]
        public FilterStringVal OrdTypeID { get; set; }
        [ProtoMember(10)]
        public FilterIntVal OrdStatus { get; set; }
        [ProtoMember(11)]
        public FilterDateVal RegisterTime { get; set; }
        [ProtoMember(12)]
        public FilterDateVal TransactTime { get; set; }
        [ProtoMember(13)]
        public FilterDateVal ExpireDate { get; set; }
        [ProtoMember(14)]
        public FilterStringVal TimeInForce { get; set; }
        [ProtoMember(15)]
        public FilterIntVal ChannelID { get; set; }
        [ProtoMember(16)]
        public FilterStringVal ExecBroker { get; set; }
        [ProtoMember(17)]
        public FilterIntVal Side { get; set; }
        [ProtoMember(18)]
        public FilterIntVal OrderQty { get; set; }
        [ProtoMember(19)]
        public FilterDecimalVal OrderQtyMin { get; set; }
        [ProtoMember(20)]
        public FilterDecimalVal OrderQtyApar { get; set; }
        [ProtoMember(21)]
        public FilterIntVal OrderQtyRemaining { get; set; }
        [ProtoMember(22)]
        public FilterDecimalVal Price { get; set; }
        [ProtoMember(23)]
        public FilterDecimalVal StopPx { get; set; }
        [ProtoMember(24)]
        public FilterStringVal Description { get; set; }
        [ProtoMember(25)]
        public FilterStringVal SystemID { get; set; }
        [ProtoMember(26)]
        public FilterStringVal Memo { get; set; }
        [ProtoMember(27)]
        public FilterIntVal CumQty { get; set; }
        [ProtoMember(28)]
        public FilterIntVal FixMsgSeqNum { get; set; }
        [ProtoMember(29)]
        public FilterStringVal SessionID { get; set; }
        [ProtoMember(30)]
        public FilterStringVal SessionIDOriginal { get; set; }
        [ProtoMember(31)]
        public FilterIntVal IdFix { get; set; }
        [ProtoMember(32)]
        public FilterStringVal MsgFix { get; set; }
        [ProtoMember(33)]
        public FilterStringVal Msg42Base64 { get; set; }
        [ProtoMember(34)]
        public FilterStringVal HandlInst { get; set; }
        [ProtoMember(35)]
        public FilterStringVal IntegrationName { get; set; }
        [ProtoMember(36)]
        public FilterStringVal Exchange { get; set; }

        public FilterSpiderOrder()
        {
            this.OrderID = new FilterIntVal();
            this.OrigClOrdID = new FilterStringVal();
            this.ExchangeNumberID = new FilterStringVal();
            this.ClOrdID = new FilterStringVal();
            this.Account = new FilterIntVal();
            this.Symbol = new FilterStringVal();
            this.SecurityExchangeID = new FilterStringVal();
            this.StopStartID = new FilterIntVal();
            this.OrdTypeID = new FilterStringVal();
            this.OrdStatus = new FilterIntVal();
            this.RegisterTime = new FilterDateVal();
            this.TransactTime = new FilterDateVal();
            this.ExpireDate = new FilterDateVal();
            this.TimeInForce = new FilterStringVal();
            this.ChannelID = new FilterIntVal();
            this.ExecBroker = new FilterStringVal();
            this.Side = new FilterIntVal();
            this.OrderQty = new FilterIntVal();
            this.OrderQtyMin = new FilterDecimalVal();
            this.OrderQtyApar = new FilterDecimalVal();
            this.OrderQtyRemaining = new FilterIntVal();
            this.Price = new FilterDecimalVal();
            this.StopPx = new FilterDecimalVal();
            this.Description = new FilterStringVal();
            this.SystemID = new FilterStringVal();
            this.Memo = new FilterStringVal();
            this.CumQty = new FilterIntVal();
            this.FixMsgSeqNum = new FilterIntVal();
            this.SessionID = new FilterStringVal();
            this.SessionIDOriginal = new FilterStringVal();
            this.IdFix = new FilterIntVal();
            this.MsgFix = new FilterStringVal();
            this.Msg42Base64 = new FilterStringVal();
            this.HandlInst = new FilterStringVal();

            this.IntegrationName = new FilterStringVal();
            this.Exchange = new FilterStringVal();
        }

        public override string ToString()
        {
            return string.Format(@"this.OrderID =  [{0}] 
this.OrigClOrdID =  [{1}]
this.ExchangeNumberID =  [{2}]
this.ClOrdID =  [{3}]
this.Account =  [{4}]
this.Symbol =  [{5}]
this.SecurityExchangeID = [{6}]
this.StopStartID =  [{7}]
this.OrdTypeID =  [{8}]
this.OrdStatus =  [{9}]
this.RegisterTime =  [{10}]
this.TransactTime =  [{11}]
this.ExpireDate =  [{12}]
this.TimeInForce =  [{13}]
this.ChannelID =  [{14}]
this.ExecBroker =  [{15}]
this.Side =  [{16}]
this.OrderQty =  [{17}]
this.OrderQtyMin =  [{18}]
this.OrderQtyApar =  [{19}]
this.OrderQtyRemaining =  [{20}]
this.Price =  [{21}]
this.StopPx =  [{22}]
this.Description =  [{23}]
this.SystemID =  [{24}]
this.Memo =  [{25}]
this.CumQty =  [{26}]
this.FixMsgSeqNum =  [{27}]
this.SessionID =  [{28}]
this.SessionIDOriginal =  [{29}]
this.IdFix =  [{30}]
this.MsgFix =  [{31}]
this.Msg42Base64 =  [{32}]
this.HandlInst =  [{33}]
this.IntegrationName =  [{34}]
this.Exchange = [{35}]", 
this.OrderID.Value, 
this.OrigClOrdID.Value, 
this.ExchangeNumberID.Value, 
this.ClOrdID.Value, 
this.Account.Value, 
this.Symbol.Value, 
this.SecurityExchangeID.Value, 
this.StopStartID.Value, 
this.OrdTypeID.Value, 
this.OrdStatus.Value, 
this.RegisterTime.Value, 
this.TransactTime.Value, 
this.ExpireDate.Value, 
this.TimeInForce.Value, 
this.ChannelID.Value, 
this.ExecBroker.Value, 
this.Side.Value, 
this.OrderQty.Value, 
this.OrderQtyMin.Value, 
this.OrderQtyApar.Value, 
this.OrderQtyRemaining.Value, 
this.Price.Value, 
this.StopPx.Value, 
this.Description.Value, 
this.SystemID.Value, 
this.Memo.Value, 
this.CumQty.Value, 
this.FixMsgSeqNum.Value, 
this.SessionID.Value, 
this.SessionIDOriginal.Value, 
this.IdFix.Value, 
this.MsgFix.Value, 
this.Msg42Base64.Value, 
this.HandlInst.Value,
this.IntegrationName.Value, 
this.Exchange.Value);
        }
    }
}
