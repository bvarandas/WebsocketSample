using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Gradual.Spider.SupervisorRisco.Lib.Dados
{
    [Serializable]
    [ProtoContract]
    public class OperatingLimitInfo
    {
        [ProtoMember(1)]
        public TipoLimiteEnum TipoLimite { set; get; }

        [ProtoMember(2, IsRequired=true)]
        public int CodigoCliente { set; get; }

        [ProtoMember(3, IsRequired = true)]
        public int CodigoParametroCliente { set; get; }

        [ProtoMember(4)]
        public decimal ValorDisponivel { set; get; }

        [ProtoMember(5)]
        public decimal ValorAlocado { set; get; }
        
        [ProtoMember(6)]
        public decimal ValotTotal { set; get; }

        [ProtoMember(7)]
        public decimal PrecoBase { get; set; }

        [ProtoMember(8)]
        public DateTime DataValidade { set; get; }

        [ProtoMember(9)]
        public decimal ValorMovimento { get; set; }

        [ProtoMember(10)]
        public string StNatureza { get; set; }

    }
}
