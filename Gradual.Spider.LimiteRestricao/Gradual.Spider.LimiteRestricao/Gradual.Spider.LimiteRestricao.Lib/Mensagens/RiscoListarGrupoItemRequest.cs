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
    public class RiscoListarGrupoItemRequest : MensagemRequestBase
    {
        [DataMember]
        public int? FiltroIdGrupo { get; set; }

        [DataMember]
        public int? FiltroIdGrupoItem { get; set; }

        [DataMember]
        public EnumRiscoRegra.TipoGrupo? FiltroTipoGrupo { get; set; }
    }
}
