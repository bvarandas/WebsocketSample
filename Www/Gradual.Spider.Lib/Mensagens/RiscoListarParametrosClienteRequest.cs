using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.OMS.Library;

namespace Gradual.Spider.Lib.Mensagens
{
    public class RiscoListarParametrosClienteRequest : MensagemRequestBase
    {
        #region Propriedades
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
