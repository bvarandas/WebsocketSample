using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Gradual.Spider.LimiteRestricao.Lib.Dados
{
    [DataContract]
    public class RiscoMonitoramentoInfo
    {
        [DataMember]
        public int IdCliente { get; set; }

        [DataMember]
        public string NmCliente { get; set; }

        [DataMember]
        public int CdAssessor { get; set; }

        [DataMember]
        public int IdParametro { get; set; }

        [DataMember]
        public string DsParametro { get; set; }

        [DataMember]
        public int IdGrupo { get; set; }

        [DataMember]
        public string DsGrupo { get; set; }

        [DataMember]
        public decimal VlLimite { get; set; }

        [DataMember]
        public decimal VlAlocado { get; set; }

        [DataMember]
        public decimal VlDisponivel { get; set; }
    }
}
