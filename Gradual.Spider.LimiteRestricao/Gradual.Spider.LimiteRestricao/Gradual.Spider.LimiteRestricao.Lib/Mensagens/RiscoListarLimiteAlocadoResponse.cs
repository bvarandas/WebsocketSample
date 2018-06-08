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
    public class RiscoListarLimiteAlocadoResponse : MensagemResponseBase
    {
        [DataMember]
        public List<RiscoLimiteInfo> Resultado { get; set; }

        public RiscoListarLimiteAlocadoResponse()
        {
            this.Resultado = new List<RiscoLimiteInfo>();
        }
    }
}
