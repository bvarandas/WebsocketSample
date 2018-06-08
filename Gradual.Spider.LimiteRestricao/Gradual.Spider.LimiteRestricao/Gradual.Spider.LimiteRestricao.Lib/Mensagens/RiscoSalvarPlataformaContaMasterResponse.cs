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
    public class RiscoSalvarPlataformaContaMasterResponse : MensagemResponseBase
    {
        [DataMember]
        public RiscoPlataformaContaMasterInfo Resultado { get; set; }

        [DataMember]
        public List<RiscoPlataformaInfo> ListaPlataforma { get; set; }

        public RiscoSalvarPlataformaContaMasterResponse()
        {
            Resultado = new RiscoPlataformaContaMasterInfo();
        }
    }
}
