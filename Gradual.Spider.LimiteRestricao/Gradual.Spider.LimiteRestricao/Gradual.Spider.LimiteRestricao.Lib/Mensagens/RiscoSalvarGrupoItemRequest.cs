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
    public class RiscoSalvarGrupoItemRequest : MensagemRequestBase
    {
        [DataMember]
        public List<RiscoGrupoItemInfo> GrupoItemLista { get; set; }

        public override string ToString()
        {
            var lRetorno = new StringBuilder();

            if (null != this.GrupoItemLista)
            {
                GrupoItemLista.ForEach(grup =>
                {
                    lRetorno.Append("{");
                    lRetorno.Append(grup.ToString());
                    lRetorno.Append("}");
                });

            }

            return lRetorno.ToString();
        }
    }
}
