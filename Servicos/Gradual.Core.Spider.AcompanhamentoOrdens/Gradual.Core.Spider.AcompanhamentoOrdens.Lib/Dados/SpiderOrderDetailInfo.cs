using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Core.Spider.AcompanhamentoOrdens.Lib.Dados
{
    public class SpiderOrderDetailInfo
    {

        public int OrderDetailID { get; set; }
        public string TransactID { get; set; }
        public int OrderID { get; set; }
        public int OrderQty { get; set; }
        public int OrderQtyRemaining { get; set; }
        public decimal Price { get; set; }
        public decimal StopPx { get; set; }
        public int OrderStatusID { get; set; }
        public DateTime TransactTime { get; set; }
        public string Description { get; set; }
        public int TradeQty { get; set; }
        public int CumQty { get; set; }
        public int FixMsgSeqNum { get; set; }
        public string CxlRejResponseTo { get; set; }
        public int CxlRejReason { get; set; }
        public string MsgFixDetail { get; set; }

        public SpiderOrderDetailInfo()
        {
            this.OrderDetailID = 0;
            this.TransactID = string.Empty;
            this.OrderID = 0;
            this.OrderQty = 0;
            this.OrderQtyRemaining = 0;
            this.Price = Decimal.Zero;
            this.StopPx = Decimal.Zero;
            this.OrderStatusID = 0;
            this.TransactTime = DateTime.MinValue;
            this.Description = string.Empty;
            this.TradeQty = 0;
            this.CumQty = 0;
            this.FixMsgSeqNum = 0;

            // Used for reject reasons
            this.CxlRejResponseTo = string.Empty;
            this.CxlRejReason = 0;
            this.MsgFixDetail = string.Empty;
        }

    }
}
