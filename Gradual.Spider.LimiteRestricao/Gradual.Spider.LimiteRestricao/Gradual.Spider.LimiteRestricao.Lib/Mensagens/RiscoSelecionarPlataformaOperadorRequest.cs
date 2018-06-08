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
    public class RiscoSelecionarPlataformaOperadorRequest : MensagemRequestBase
    {
        [DataMember]
        public RiscoPlataformaOperadorInfo Objeto { get; set; }

        public RiscoSelecionarPlataformaOperadorRequest()
        {
            this.Objeto = new RiscoPlataformaOperadorInfo();
        }
    }
}
