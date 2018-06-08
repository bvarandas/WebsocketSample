using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Core.Spider.AcompanhamentoOrdens.Lib.Dados
{
    public class IntegrationInfo
    {

        public int IntegrationId { get; set; }
        public string IntegrationName { get; set; }
        public string Bolsa { get; set; }
        public IntegrationInfo()
        {
            this.IntegrationId = 0;
            this.IntegrationName = string.Empty;
            this.Bolsa = string.Empty;
        }

    }
}
