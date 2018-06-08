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
    public class RiscoSalvarRegraGrupoItemRequest : MensagemRequestBase
    {
        [DataMember]
        public RiscoRegraGrupoItemInfo RegraGrupoItem { get; set; }

        public override string ToString()
        {
            string lRetorno = "{";

            if (null != RegraGrupoItem)
            {
                lRetorno += this.RegraGrupoItem.ToString();
            }

            lRetorno += "}";
            return lRetorno;
        }
    }
}
