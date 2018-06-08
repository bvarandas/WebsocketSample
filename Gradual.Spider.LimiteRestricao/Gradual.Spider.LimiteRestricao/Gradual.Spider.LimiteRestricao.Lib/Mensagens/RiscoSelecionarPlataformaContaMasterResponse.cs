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
    public class RiscoSelecionarPlataformaContaMasterResponse :  MensagemResponseBase
    {
        [DataMember]
        public RiscoPlataformaContaMasterInfo Resultado { get; set; }

        public RiscoSelecionarPlataformaContaMasterResponse()
        {
            Resultado = new RiscoPlataformaContaMasterInfo();
        }
    }
}
