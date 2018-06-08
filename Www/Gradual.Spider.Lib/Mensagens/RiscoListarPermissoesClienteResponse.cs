using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.OMS.Library;
using Gradual.Spider.Lib.Dados;

namespace Gradual.Spider.Lib.Mensagens
{
    public class RiscoListarPermissoesClienteResponse : MensagemResponseBase
    {
        public RiscoListarPermissoesClienteResponse()
        {
            this.PermissoesAssociadas = new List<RiscoPermissaoAssociadaInfo>();
        }

        public List<RiscoPermissaoAssociadaInfo> PermissoesAssociadas { get; set; }

        public override string ToString()
        {
            string lRetorno = "{";

            if (null != PermissoesAssociadas) {
                foreach (RiscoPermissaoAssociadaInfo item in PermissoesAssociadas)
                {
                    lRetorno += item.ToString();
                }
            }

            lRetorno += "}";
            return lRetorno;
        }
    }
}
