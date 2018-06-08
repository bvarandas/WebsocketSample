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
    public class RiscoReceberParametroClienteResponse : MensagemResponseBase
    {
        #region Propriedades
        
        [DataMember]
        public RiscoParametroClienteInfo ParametroRiscoCliente{ get; set; }

        public override string ToString()
        {
            string lRetorno = "{";

            if (null != ParametroRiscoCliente) {
                lRetorno += ParametroRiscoCliente.ToString();
            }
            lRetorno += "}";
            return lRetorno;

        }
        #endregion

        #region Construtores

        public RiscoReceberParametroClienteResponse() { }

        ~RiscoReceberParametroClienteResponse() { }
        #endregion
    }
}
