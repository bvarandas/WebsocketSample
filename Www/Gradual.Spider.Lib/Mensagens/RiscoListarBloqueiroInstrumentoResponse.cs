using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.OMS.Library;
using Gradual.Spider.Lib.Dados;

namespace Gradual.Spider.Lib.Mensagens
{
    public class RiscoListarBloqueiroInstrumentoResponse : MensagemResponseBase
    {
        public List<RiscoBloqueioInstrumentoInfo> Resultado { get; set; }

        public RiscoListarBloqueiroInstrumentoResponse()
        {
            this.Resultado = new List<RiscoBloqueioInstrumentoInfo>();
        }
    }
}
