using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.SupervisorRisco.Lib.Dados
{
    public class CustodiaInfo
    {
        public int Account { get; set; }
        public string Symbol { get; set; }
        public int Qty { get; set; }
        public decimal ValorCustodia { get; set; }
    }
}
