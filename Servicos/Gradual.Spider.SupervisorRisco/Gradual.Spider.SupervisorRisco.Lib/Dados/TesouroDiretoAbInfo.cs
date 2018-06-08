using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.SupervisorRisco.Lib.Dados
{
    public class TesouroDiretoAbInfo
    {
        public int CodCliente{ get; set; }
        public decimal ValPosi { get; set; }

        public TesouroDiretoAbInfo()
        {
            this.CodCliente = 0;
            this.ValPosi = decimal.Zero;
        }
    }
}
