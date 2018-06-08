using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.OMS.Library;
using System.Runtime.Serialization;

namespace Gradual.Spider.LimiteRestricao.Lib.Dados
{
    [DataContract]
    public class RiscoRegraGrupoItemInfo : ICodigoEntidade
    {
        public RiscoRegraGrupoItemInfo() { }

        [DataMember]
        public int CodigoGrupoRegra { get; set; }

        [DataMember]
        public int CodigoAcao { get; set; }

        [DataMember]
        public string Sentido { get; set; }

        [DataMember]
        public int? CodigoGrupo { get; set; }

        [DataMember]
        public string NomeAcao { get; set; }

        [DataMember]
        public string NomeGrupo { get; set; }

        [DataMember]
        public int? CodigoCliente { get; set; }

        [DataMember]
        public int? CodigoUsuario { get; set; }

        //public EnumRiscoRegra.TipoGrupo? TipoGrupo { get; set; }

        //public GrupoInfo Grupo { get; set; }
        [DataMember]
        public List<RiscoClienteBloqueioRegraInfo> ListaClienteBloqueio { get; set; }

        public string ReceberCodigo()
        {
            throw new NotImplementedException();
        }
    }
}
