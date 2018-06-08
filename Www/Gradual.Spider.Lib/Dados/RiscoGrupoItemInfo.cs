using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.Lib.Dados
{
    public class RiscoGrupoItemInfo
    {
        

        public RiscoGrupoItemInfo(RiscoGrupoInfo pGrupo) 
        {
            this.CodigoGrupoItem = 0;
            this.NomeGrupoItem   = string.Empty;
            this.CodigoGrupo     = pGrupo.CodigoGrupo;
            this.NomeGrupo       = pGrupo.NomeDoGrupo;
        }

        public int? CodigoGrupoItem { get; set; }

        public string NomeGrupoItem { get; set; }

        public int? CodigoGrupo { get; set; }

        public string NomeGrupo { get; set; }

        public RiscoEnumRegra.TipoGrupo? TipoGrupo { get; set; }

        public override string ToString()
        {
            return string.Format(" ; {[CodigoGrupoItem] {0} ; [NomeGrupoItem] {1} ; [CodigoGrupo] {2}}", this.CodigoGrupoItem.ToString(), this.NomeGrupoItem.ToString(), this.CodigoGrupo.ToString());
        }

        #region Construtores
        public RiscoGrupoItemInfo() { }
        #endregion
    }
}
