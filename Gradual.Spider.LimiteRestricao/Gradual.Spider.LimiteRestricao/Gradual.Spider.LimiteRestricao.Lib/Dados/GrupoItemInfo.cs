using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Gradual.Spider.LimiteRestricao.Lib.Dados
{
    [DataContract]
    public class GrupoItemInfo
    {
        public GrupoItemInfo() { }

        public GrupoItemInfo(GrupoInfo pGrupo)
        {
            this.CodigoGrupoItem = 0;
            this.NomeGrupoItem = string.Empty;
            this.CodigoGrupo = pGrupo.CodigoGrupo;
            this.NomeGrupo = pGrupo.NomeDoGrupo;
        }

        [DataMember]
        public int? CodigoGrupoItem { get; set; }

        [DataMember]
        public string NomeGrupoItem { get; set; }

        [DataMember]
        public int? CodigoGrupo { get; set; }

        [DataMember]
        public string NomeGrupo { get; set; }

        [DataMember]
        public EnumRiscoRegra.TipoGrupo? TipoGrupo { get; set; }

        public override string ToString()
        {
            return string.Format(" ; {[CodigoGrupoItem] {0} ; [NomeGrupoItem] {1} ; [CodigoGrupo] {2}}", this.CodigoGrupoItem.ToString(), this.NomeGrupoItem.ToString(), this.CodigoGrupo.ToString());
        }
    }
}
