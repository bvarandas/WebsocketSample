using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.SupervisorRisco.Lib.Dados
{
    public class TOSymbolInfo
    {
        public SymbolInfo Instrumento { get; set; }
        // public SymbolInfo InstrumentoAnterior { get; set; }
        public decimal VlrUltimoAnterior { get; set; }
        public DateTime DtUpdated { get; set; }

        public TOSymbolInfo()
        {
            this.Instrumento = null;
            this.VlrUltimoAnterior = decimal.Zero;
            this.DtUpdated = DateTime.MinValue;
        }
    }
}
