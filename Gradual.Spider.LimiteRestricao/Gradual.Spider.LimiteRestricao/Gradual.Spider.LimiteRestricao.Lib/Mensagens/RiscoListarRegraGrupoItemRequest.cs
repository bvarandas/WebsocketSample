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
    public class RiscoListarRegraGrupoItemRequest : MensagemRequestBase
    {
        [DataMember]
        public RiscoRegraGrupoItemInfo Objeto { get; set; }

        public RiscoListarRegraGrupoItemRequest()
        {
            this.Objeto = new RiscoRegraGrupoItemInfo();
        }
    }
}
