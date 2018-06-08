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
    public class RiscoListarGruposRequest : MensagemRequestBase
    {
        public RiscoListarGruposRequest() { }

        ~RiscoListarGruposRequest() { }

        [DataMember]
        public string FiltroNomeGrupo { get; set; }

        [DataMember]
        public int? FiltroIdGrupo { get; set; }

        [DataMember]
        public EnumRiscoRegra.TipoGrupo FiltroTipoGrupo { get; set; }

        public override string ToString()
        {
            return " ; {[FiltroNomeGrupo] " + this.FiltroNomeGrupo.ToString() + "}";
        }
    }
}
