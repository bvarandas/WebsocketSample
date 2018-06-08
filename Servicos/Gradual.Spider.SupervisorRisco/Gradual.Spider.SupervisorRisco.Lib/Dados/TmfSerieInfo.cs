using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.SupervisorRisco.Lib.Dados
{
    public class TmfSerieInfo
    {
        public string CdCommod { get; set; }
        public string CdMercad { get; set; }
        public string CdSerie { get; set; }
        public string CdCodNeg { get; set; }

        public TmfSerieInfo()
        {
            this.CdCommod = string.Empty;
            this.CdMercad = string.Empty;
            this.CdSerie = string.Empty;
            this.CdCodNeg = string.Empty;
        }

    }
}
