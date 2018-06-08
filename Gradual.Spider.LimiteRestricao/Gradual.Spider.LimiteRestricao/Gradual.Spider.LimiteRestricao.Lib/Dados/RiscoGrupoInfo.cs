using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Gradual.Spider.LimiteRestricao.Lib.Dados
{
    [DataContract]
    public class RiscoGrupoInfo
    {
        #region Propriedades
        
        [DataMember]
        public int CodigoGrupo { get; set; }

        [DataMember]
        public string NomeDoGrupo { get; set; }

        [DataMember]
        public List<RiscoGrupoItemInfo> GrupoItens { get; set; }

        [DataMember]
        public EnumRiscoRegra.TipoGrupo TipoGrupo { get; set; }
        #endregion

        #region Construtores
        public RiscoGrupoInfo() { }
        #endregion

    }
}
