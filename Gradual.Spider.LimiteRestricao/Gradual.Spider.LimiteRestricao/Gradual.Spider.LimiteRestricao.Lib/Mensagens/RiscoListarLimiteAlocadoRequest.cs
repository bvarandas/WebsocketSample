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
    public class RiscoListarLimiteAlocadoRequest : MensagemRequestBase
    {
        [DataMember]
        public RiscoLimiteInfo Objeto { get; set; }

        public RiscoListarLimiteAlocadoRequest()
        {
            this.Objeto = new RiscoLimiteInfo();
        }
    }
}
