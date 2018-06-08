using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.SupervisorRisco.Lib.Dados
{
    public class ProdutosInfo
    {
        public int CodCliente { get; set; }
        public decimal SaldoLiquido { get; set; }

        public ProdutosInfo()
        {
            this.CodCliente = 0;
            this.SaldoLiquido = decimal.Zero;
        }

    }
}
