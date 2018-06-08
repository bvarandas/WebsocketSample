using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Gradual.Spider.LimiteRestricao.Lib.Mensagens
{
    [DataContract]
    [Serializable]
    public class RiscoRemoverGrupoRequest
    {
        public RiscoRemoverGrupoRequest() { }

        ~RiscoRemoverGrupoRequest() { }
        
        [DataMember]
        public int CodigoGrupo { get; set; }

        public override string ToString()
        {
            return " ; {[CodigoGrupo] " + this.CodigoGrupo.ToString() + "}";
        }
    }
}
