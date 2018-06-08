using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.SupervisorRisco.Lib.Dados
{
    public class ContaCorrenteInfo
    {
        public int CodCliente { get;set;}
        public decimal VlrTotal { get; set; }


        public ContaCorrenteInfo()
        {
            this.CodCliente = 0;
            this.VlrTotal = decimal.Zero;
        }
    }
}
