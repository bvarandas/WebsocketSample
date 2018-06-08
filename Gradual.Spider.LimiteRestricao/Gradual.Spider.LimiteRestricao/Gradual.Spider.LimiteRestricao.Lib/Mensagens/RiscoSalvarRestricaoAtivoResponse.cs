using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Gradual.OMS.Library;
using Gradual.Spider.LimiteRestricao.Lib.Dados;

namespace Gradual.Spider.LimiteRestricao.Lib.Mensagens
{
    [DataContract]
    [Serializable]
    public class RiscoSalvarRestricaoAtivoResponse : MensagemResponseBase
    {
        [DataMember]
        public List<RiscoRestricaoAtivoInfo> ListaRestricaoAtivo { get; set; }
    }
}
