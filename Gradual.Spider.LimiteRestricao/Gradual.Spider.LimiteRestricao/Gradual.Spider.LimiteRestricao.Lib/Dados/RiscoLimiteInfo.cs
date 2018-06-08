using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Gradual.Spider.LimiteRestricao.Lib.Dados
{
    [DataContract]
    public class RiscoLimiteInfo
    {
        [DataMember]
        public int ConsultaIdCliente { get; set; }

        [DataMember]
        public int IdParametro      { get; set; }

        [DataMember]
        public string DsParametro   { get; set; }

        [DataMember]
        public decimal VlParametro  { get; set; }

        [DataMember]
        public decimal VlAlocado    { get; set; }

        [DataMember]
        public decimal VlDisponivel { get; set; }

        [DataMember]
        public bool NovoOMS         { get; set; }

        [DataMember]
        public bool Spider          { get; set; }

        [DataMember]
        public List<RiscoLimiteInfo> Resultado { get; set; }
    }
}
