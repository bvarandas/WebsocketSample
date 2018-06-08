using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.Spider.LimiteRestricao.Lib.Dados;
using Gradual.OMS.Library;
using System.Runtime.Serialization;

namespace Gradual.Spider.LimiteRestricao.Lib.Mensagens
{
    [DataContract]
    [Serializable]
    public class RiscoListarGruposResponse : MensagemResponseBase
    {
        [DataMember]
        public RiscoGrupoInfo m_GrupoInfo;

        public RiscoListarGruposResponse() { }

        ~RiscoListarGruposResponse() { }

        [DataMember]
        public List<RiscoGrupoInfo> Grupos
        {
            get;
            set;
        }

        public override string ToString()
        {
            string lRetorno = "{";

            lRetorno += this.m_GrupoInfo.ToString();

            if (null != this.Grupos)
            {
                foreach (RiscoGrupoInfo item in this.Grupos)
                {
                    lRetorno += item.ToString();
                }
            }

            lRetorno += "}";

            return lRetorno;
        }

    }
}
