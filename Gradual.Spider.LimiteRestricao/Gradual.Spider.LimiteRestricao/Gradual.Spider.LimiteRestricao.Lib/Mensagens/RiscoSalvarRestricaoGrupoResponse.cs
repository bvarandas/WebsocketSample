using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.OMS.Library;
using System.Runtime.Serialization;
using Gradual.Spider.LimiteRestricao.Lib.Dados;

namespace Gradual.Spider.LimiteRestricao.Lib.Mensagens
{
    [DataContract]
    [Serializable]
    public class RiscoSalvarRestricaoGrupoResponse : MensagemResponseBase
    {
        [DataMember]
        public List<RiscoRestricaoGrupoInfo> ListaRestricaoGrupo { get; set; }
    }
}
