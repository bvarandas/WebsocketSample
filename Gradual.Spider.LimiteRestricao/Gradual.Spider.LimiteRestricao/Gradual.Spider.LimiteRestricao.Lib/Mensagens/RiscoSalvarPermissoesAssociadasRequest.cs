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
    public class RiscoSalvarPermissoesAssociadasRequest : MensagemRequestBase
    {
        [DataMember]
        public List<RiscoPermissaoAssociadaInfo> PermissoesAssociadas { get; set; }

        public override string ToString()
        {
            string lRetorno = "{";

            if (null != PermissoesAssociadas)
            {
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
