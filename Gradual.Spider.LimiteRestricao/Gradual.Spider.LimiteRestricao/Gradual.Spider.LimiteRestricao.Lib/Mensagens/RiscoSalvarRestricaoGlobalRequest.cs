using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.OMS.Library;
using System.Runtime.Serialization;
using Gradual.Spider.LimiteRestricao.Lib.Dados;

namespace Gradual.Spider.LimiteRestricao.Lib.Mensagens
{
    [Serializable]
    [DataContract]
    public class RiscoSalvarRestricaoGlobalRequest : MensagemRequestBase
    {
        [DataMember]
        public List<RiscoRestricaoGlobalInfo> ListaRestricoesGlobal { get; set; }
    }
}
