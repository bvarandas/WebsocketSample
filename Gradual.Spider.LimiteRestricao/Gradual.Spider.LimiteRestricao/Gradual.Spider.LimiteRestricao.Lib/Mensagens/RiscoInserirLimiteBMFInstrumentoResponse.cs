using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Gradual.OMS.Library;

namespace Gradual.Spider.LimiteRestricao.Lib.Mensagens
{
    [Serializable]
    [DataContract]
    public class RiscoInserirLimiteBMFInstrumentoResponse : MensagemResponseBase
    {
        [DataMember]
        public int IdClienteParametroBMF { set; get; }

        [DataMember]
        public bool bSucesso { set; get; }
    }
}
