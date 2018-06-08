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
    public class RiscoSelecionarPlataformaClienteResponse : MensagemResponseBase
    {
        [DataMember]
        public RiscoPlataformaClienteInfo Resultado { get; set; }

        [DataMember]
        public List<RiscoPlataformaInfo> ListaPlataforma { get; set; }

        public RiscoSelecionarPlataformaClienteResponse()
        {
            Resultado = new RiscoPlataformaClienteInfo();
        }
    }
}
