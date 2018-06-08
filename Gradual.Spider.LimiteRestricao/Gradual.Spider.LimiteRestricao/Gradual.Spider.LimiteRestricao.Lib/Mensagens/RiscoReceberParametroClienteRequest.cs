using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.OMS.Library;
using System.Runtime.Serialization;

namespace Gradual.Spider.LimiteRestricao.Lib.Mensagens
{
    [DataContract]
    [Serializable]
    public class RiscoReceberParametroClienteRequest : MensagemRequestBase
    {
        public RiscoReceberParametroClienteRequest() { }

        ~RiscoReceberParametroClienteRequest() { }

        [DataMember]
		public int CodigoParametroRiscoCliente { get; set; }

        public override string ToString()
        {
            return " ; {[CodigoParametroRiscoCliente] " + this.CodigoParametroRiscoCliente.ToString() + "}";
        }
    }
}
