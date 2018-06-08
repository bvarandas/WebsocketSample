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
    public class RiscoListarParametrosClienteRequest : MensagemRequestBase
    {
        #region Propriedades
        [DataMember]
        public int CodigoCliente { get; set; }
        #endregion

        #region Construtores
        public RiscoListarParametrosClienteRequest(){

		}

        ~RiscoListarParametrosClienteRequest(){		}

        public override string ToString()
        {
            return " ; {[CodigoCliente] " + this.CodigoCliente.ToString() + "}";
        }
        #endregion
    }
}
