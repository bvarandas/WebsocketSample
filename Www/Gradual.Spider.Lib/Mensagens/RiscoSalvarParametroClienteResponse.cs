using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.OMS.Library;
using Gradual.Spider.Lib.Dados;

namespace Gradual.Spider.Lib.Mensagens
{
    public class RiscoSalvarParametroClienteResponse : MensagemResponseBase
    {
        public RiscoSalvarParametroClienteResponse() { }
        
        public RiscoParametroClienteInfo ParametroRiscoCliente { get; set; }

        ~RiscoSalvarParametroClienteResponse() { }

        public override string ToString()
        {
            string lRetorno = "{";

            if (null != ParametroRiscoCliente)
            {
                lRetorno += this.ParametroRiscoCliente.ToString();
            }

            lRetorno += "}";
            return lRetorno;
        }
    }
}
