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
    public class RiscoSalvarGrupoItemResponse : MensagemResponseBase
    {
        [DataMember]
        public RiscoGrupoItemInfo GrupoItem { get; set; }

        [DataMember]
        public List<RiscoGrupoItemInfo> ObjetoDeRetorno { get; set; }

        public override string ToString()
        {
            string lRetorno = "{";

            if (null != GrupoItem)
            {
                lRetorno += this.GrupoItem.ToString();
            }

            lRetorno += "}";

            return lRetorno;
        }
    }
}
