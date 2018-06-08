using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Gradual.OMS.Library;
using Gradual.Spider.LimiteRestricao.Lib.Dados;

namespace Gradual.Spider.LimiteRestricao.Lib.Mensagens
{
    [Serializable]
    [DataContract]
    public class RiscoListarPlataformaResponse : MensagemResponseBase
    {
        [DataMember]
        public List<RiscoPlataformaInfo> ListaPlataforma { get; set; }
    }
}
