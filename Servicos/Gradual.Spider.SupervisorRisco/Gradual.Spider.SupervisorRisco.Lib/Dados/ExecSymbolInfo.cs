using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.SupervisorRisco.Lib.Dados
{
    public class ExecSymbolInfo
    {
        public int Id { get; set; }
        public int Side { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public string Symbol { get; set; }
        public int Account { get; set; }
        public ExecSymbolInfo()
        {
            this.Side = 0;
            this.Qty = 0;
            this.Price = decimal.Zero;
            this.Symbol = string.Empty;
            this.Account = 0;
        }
    }
}
