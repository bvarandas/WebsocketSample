using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.OMS.Library;
using Gradual.Spider.LimiteRestricao.Lib.Dados;
using System.Runtime.Serialization;

namespace Gradual.Spider.LimiteRestricao.Lib.Mensagens
{
    [DataContract]
    [Serializable]
    public class RiscoSalvarParametroClienteResponse : MensagemResponseBase
    {
        //public RiscoSalvarParametroClienteResponse() { }
        
        [DataMember]
        public RiscoParametroClienteInfo ParametroRiscoCliente { get; set; }

        //~RiscoSalvarParametroClienteResponse() { }

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
