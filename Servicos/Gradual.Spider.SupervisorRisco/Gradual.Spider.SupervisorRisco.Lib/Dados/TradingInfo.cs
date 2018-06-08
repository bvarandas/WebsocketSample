using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Gradual.Spider.SupervisorRisco.Lib.Dados
{
    [Serializable]
    [ProtoContract]
    public class TradingInfo
    {
        [ProtoMember(1)]
        public string Instrumento { get; set; }
        [ProtoMember(2)]
        public Nullable<DateTime> DtNegocio { get; set; }
        [ProtoMember(3)]
        public Nullable<DateTime> DtAtualizacao { get; set; }
        [ProtoMember(4)]
        public decimal VlrUltima;
        [ProtoMember(5)]
        public decimal VlrOscilacao;
        [ProtoMember(6)]
        public decimal VlrFechamento;
    }
}
