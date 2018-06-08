using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.Lib.Dados
{
    public class RiscoGrupoInfo
    {
        #region Propriedades
        
        public int CodigoGrupo { get; set; }

        public string NomeDoGrupo { get; set; }

        public List<RiscoGrupoItemInfo> GrupoItens { get; set; }

        public RiscoEnumRegra.TipoGrupo TipoGrupo { get; set; }
        #endregion

        #region Construtores
        public RiscoGrupoInfo() { }
        #endregion

    }
}
