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
    public class RiscoSalvarPlataformaClienteResponse : MensagemResponseBase
    {
        [DataMember]
        public RiscoPlataformaClienteInfo Resultado { get; set; }

        [DataMember]
        public List<RiscoPlataformaInfo> ListaPlataforma { get; set; }

        public RiscoSalvarPlataformaClienteResponse()
        {
            Resultado = new RiscoPlataformaClienteInfo();
        }
    }
}
