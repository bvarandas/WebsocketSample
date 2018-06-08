using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.JaneStreet.Report.Lib.Dados
{
    public class ReportDCInfo
    {
        public string DateOrder { get; set; }
        public int Client { get; set; }
        public string HourOrder { get; set; }
        public string Side { get; set; }
        public string Symbol { get; set; }
        public int Filled { get; set; }
        public decimal Price { get; set; }
        public DateTime RegisterTime { get; set; }
        public int OrderQty { get; set; }
        public decimal OrderPx { get; set; }
        public string ContraBroker { get; set; }


        public ReportDCInfo()
        {
            this.DateOrder = string.Empty;
            this.Client = 0;
            this.HourOrder = string.Empty;
            this.Side = string.Empty;
            this.Symbol = string.Empty;
            this.Filled = 0;
            this.Price = decimal.Zero;
            this.RegisterTime = DateTime.MinValue;
            this.OrderQty = 0;
            this.OrderPx = decimal.Zero;
            this.ContraBroker = string.Empty;

        }

    }
}
