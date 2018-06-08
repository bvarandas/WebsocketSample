using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.OMS.Library;
using System.Runtime.Serialization;

namespace Gradual.Spider.LimiteRestricao.Lib.Mensagens
{
    [Serializable]
    [DataContract]
    public class RiscoRemoveLimiteBMFResponse : MensagemResponseBase
    {
        [DataMember]
        public int IdClienteParametroBMF { set; get; }

        [DataMember]
        public bool bSucesso { set; get; }
    }
}
