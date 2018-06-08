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
    public class RiscoRemoverRegraGrupoItemResponse : MensagemResponseBase
    {
        public RiscoRemoverRegraGrupoItemResponse() { }

        ~RiscoRemoverRegraGrupoItemResponse() { }
        
        [DataMember]
        public RiscoRegraGrupoItemInfo Objeto { get; set; }

        [DataMember]
        public Boolean BusinessException { get; set; }

        [DataMember]
        public string MessageException { get; set; }

        public override string ToString()
        {
            return " ; {[BusinessException] " + this.BusinessException.ToString() +
                " ; [MessageException] " + this.MessageException.ToString() +
                "}";
        }
    }
}
