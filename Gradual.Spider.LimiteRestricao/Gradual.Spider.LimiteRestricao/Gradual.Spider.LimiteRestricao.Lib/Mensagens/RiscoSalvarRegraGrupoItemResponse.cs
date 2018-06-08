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
    public class RiscoSalvarRegraGrupoItemResponse : MensagemResponseBase
    {
        [DataMember]
        public RiscoRegraGrupoItemInfo RegraGrupoItem { get; set; }
    }
}
