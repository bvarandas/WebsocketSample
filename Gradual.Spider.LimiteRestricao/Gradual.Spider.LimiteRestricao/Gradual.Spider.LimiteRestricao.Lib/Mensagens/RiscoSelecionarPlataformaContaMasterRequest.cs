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
    public class RiscoSelecionarPlataformaContaMasterRequest : MensagemRequestBase
    {
        [DataMember]
        public RiscoPlataformaContaMasterInfo Objeto { get; set; }

        public RiscoSelecionarPlataformaContaMasterRequest()
        {
            this.Objeto = new RiscoPlataformaContaMasterInfo();
        }
    }
}
