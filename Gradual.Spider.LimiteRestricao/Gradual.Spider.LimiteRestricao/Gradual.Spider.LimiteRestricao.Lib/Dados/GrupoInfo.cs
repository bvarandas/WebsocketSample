using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Gradual.Spider.LimiteRestricao.Lib.Dados
{
    [DataContract]
    public class GrupoInfo
    {

        public GrupoInfo() { }

        [DataMember]
        public int CodigoGrupo { get; set; }

        [DataMember]
        public string NomeDoGrupo { get; set; }

        [DataMember]
        public List<GrupoItemInfo> GrupoItens { get; set; }

        [DataMember]
        public EnumRiscoRegra.TipoGrupo TipoGrupo { get; set; }

        public override string ToString()
        {
            string lRetorno = string.Format(" ; {[CodigoGrupo] {0} ; [NomeDoGrupo] {1} }", this.CodigoGrupo.ToString(), this.NomeDoGrupo.ToString());

            if (null != GrupoItens)
            {
                foreach (GrupoItemInfo item in GrupoItens)
                {
                    lRetorno += item.ToString();
                }
            }

            return lRetorno;
        }
    }
}
