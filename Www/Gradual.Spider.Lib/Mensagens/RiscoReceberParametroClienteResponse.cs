using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.OMS.Library;
using Gradual.Spider.Lib.Dados;

namespace Gradual.Spider.Lib.Mensagens
{
    public class RiscoReceberParametroClienteResponse : MensagemResponseBase
    {
        #region Propriedades
        

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
