using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Core.Spider.AcompanhamentoOrdens.Lib.Dados
{
    public class SpiderOrderInfo
    {

        public int OrderID { get; set; }
        public string OrigClOrdID { get; set; }
        public string ExchangeNumberID { get; set; }
        public string ClOrdID { get; set; }
        public int Account { get; set; }
        public string Symbol { get; set; }
        public string SecurityExchangeID { get; set; }
        public Nullable<int> StopStartID { get; set; }
        public string OrdTypeID { get; set; }
        public int OrdStatusID { get; set; }
        public DateTime RegisterTime { get; set; }
        public DateTime TransactTime { get; set; }
        public DateTime ExpireDate { get; set; }
        public string TimeInForce { get; set; }
        public int ChannelID { get; set; }
        public string ExecBroker { get; set; }
        public int Side { get; set; }
        public int OrderQty { get; set; }
        public int OrderQtyRemaining { get; set; }
        public decimal MinQty { get; set; }
        public decimal MaxFloor { get; set; }
        public decimal Price { get; set; }
        public decimal StopPx { get; set; }
        public string Description { get; set; }
        public int CumQty { get; set; }
        public int FixMsgSeqNum { get; set; }
        public string SystemID { get; set; }
        public string Memo { get; set; }
        public string SessionID { get; set; }
        public string SessionIDOrigin { get; set; }
        public int IdSession { get; set; }
        public string MsgFix { get; set; }
        public string Msg42Base64 { get; set; }
        public string HandlInst { get; set; }


        // "join" fields
        public string IntegrationName { get; set; }
        public List<SpiderOrderDetailInfo> Details;
        public string Exchange { get; set; }

        public SpiderOrderInfo()
        {
            this.OrderID  = 0;
            this.OrigClOrdID  = string.Empty;
            this.ExchangeNumberID = string.Empty;
            this.ClOrdID  = string.Empty;
            this.Account  = 0;
            this.Symbol  = string.Empty;
            this.SecurityExchangeID  = string.Empty;
            this.StopStartID  = 0;
            this.OrdTypeID  = string.Empty;
            this.OrdStatusID  = -1;
            this.RegisterTime  = DateTime.MinValue;
            this.TransactTime  = DateTime.MinValue;
            this.ExpireDate  = DateTime.MinValue;
            this.TimeInForce  = string.Empty;
            this.ChannelID  = 0;
            this.ExecBroker  = string.Empty;
            this.Side  = -1;
            this.OrderQty  = 0;
            this.OrderQtyRemaining  = 0;
            this.MinQty  = Decimal.Zero;
            this.MaxFloor  = Decimal.Zero;
            this.Price  = Decimal.Zero;
            this.StopPx  = Decimal.Zero;
            this.Description  = string.Empty;
            this.CumQty  = 0;
            this.FixMsgSeqNum = 0;
            this.SystemID  = string.Empty;
            this.Memo  = string.Empty;
            this.SessionID  = string.Empty;
            this.SessionIDOrigin  = string.Empty;
            this.IdSession  = 0;
            this.MsgFix = string.Empty; // may not be necessary to attrib
            this.Msg42Base64 = string.Empty; // may not be necessary to attrib
            this.HandlInst = string.Empty; // may not be necessary to attrib


            this.IntegrationName = string.Empty;
            this.Details = new List<SpiderOrderDetailInfo>();
            this.Exchange = string.Empty;
        }

    }
}
