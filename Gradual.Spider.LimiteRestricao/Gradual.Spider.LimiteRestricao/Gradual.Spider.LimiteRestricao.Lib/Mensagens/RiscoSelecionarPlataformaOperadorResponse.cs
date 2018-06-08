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
    public class RiscoSelecionarPlataformaOperadorResponse : MensagemResponseBase
    {
        [DataMember]
        public RiscoPlataformaOperadorInfo Resultado { get; set; }

        public RiscoSelecionarPlataformaOperadorResponse()
        {
            Resultado = new RiscoPlataformaOperadorInfo();
        }
    }
}
