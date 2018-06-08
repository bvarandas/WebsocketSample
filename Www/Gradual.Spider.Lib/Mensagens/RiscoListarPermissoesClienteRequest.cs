using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.OMS.Library;

namespace Gradual.Spider.Lib.Mensagens
{
    public class RiscoListarPermissoesClienteRequest : MensagemRequestBase
    {
        public int CodigoCliente { get; set; }

        public override string ToString()
        {
            return " ; {[CodigoCliente] " + this.CodigoCliente.ToString() +
                "}";
        }
    }
}
