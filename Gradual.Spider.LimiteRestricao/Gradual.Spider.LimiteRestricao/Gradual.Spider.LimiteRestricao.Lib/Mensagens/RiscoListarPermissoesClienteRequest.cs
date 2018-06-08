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
    public class RiscoListarPermissoesClienteRequest : MensagemRequestBase
    {
        [DataMember]
        public int CodigoCliente { get; set; }

        public override string ToString()
        {
            return " ; {[CodigoCliente] " + this.CodigoCliente.ToString() +
                "}";
        }
    }
}
