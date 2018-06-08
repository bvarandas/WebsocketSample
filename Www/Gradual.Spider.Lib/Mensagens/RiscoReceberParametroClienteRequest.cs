using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.OMS.Library;

namespace Gradual.Spider.Lib.Mensagens
{
    public class RiscoReceberParametroClienteRequest : MensagemRequestBase
    {
        public RiscoReceberParametroClienteRequest() { }

        ~RiscoReceberParametroClienteRequest() { }

		public int CodigoParametroRiscoCliente { get; set; }

        public override string ToString()
        {
            return " ; {[CodigoParametroRiscoCliente] " + this.CodigoParametroRiscoCliente.ToString() + "}";
        }
    }
}
