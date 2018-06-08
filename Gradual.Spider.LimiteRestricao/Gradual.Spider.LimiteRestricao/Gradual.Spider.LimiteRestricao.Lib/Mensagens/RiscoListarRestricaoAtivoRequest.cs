using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.OMS.Library;
using Gradual.Spider.LimiteRestricao.Lib.Dados;
using System.Runtime.Serialization;

namespace Gradual.Spider.LimiteRestricao.Lib.Mensagens
{
    [Serializable]
    [DataContract]
    public class RiscoListarRestricaoAtivoRequest : MensagemRequestBase
    {
        public RiscoRestricaoAtivoInfo RestricaoAtivo { get; set; }
    }
}
