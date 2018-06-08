using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.SupervisorRisco.Lib.Dados
{
    public class GarantiaInfo
    {
        public int CodCliente               { get; set; }

        public decimal GarantiaDisponivel   { get; set; }
        
        public string Bolsa                 { get; set; }
        
        public int Quantidade               { get; set; }

        public string Instrumento           { get; set; }

        public GarantiaInfo()
        {
            this.CodCliente         = 0;
            this.GarantiaDisponivel = decimal.Zero;
            this.Bolsa              = string.Empty;
            this.Quantidade         = 0;
            this.Instrumento        = string.Empty;
        }
    }
}
