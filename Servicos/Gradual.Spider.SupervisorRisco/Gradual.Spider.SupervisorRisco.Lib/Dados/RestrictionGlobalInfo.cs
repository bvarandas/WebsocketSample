using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Gradual.Spider.SupervisorRisco.Lib.Dados
{
    [Serializable]
    [ProtoContract]
    public class RestrictionGlobalInfo
    {
        [ProtoMember(1, IsRequired = true)]
        public int Account { get; set; }
        [ProtoMember(2, IsRequired = true)]
        public Decimal LimiteVolumeNet { get; set; }
        [ProtoMember(3, IsRequired = true)]
        public Decimal QuantidadeNet { get; set; }
        [ProtoMember(4, IsRequired = true)]
        public Decimal LimiteMaxOfertaVolume { get; set; }
        [ProtoMember(5, IsRequired = true)]
        public Decimal LimiteMaxOfertaQtde { get; set; }
        [ProtoMember(6, IsRequired = true)]
        public bool StAtivo { get; set; }
        [ProtoMember(7, IsRequired = true)]
        public DateTime DtAtualizacao { get; set; }

        [ProtoMember(8, IsRequired = true)]
        public Decimal VolumeNetAlocado { get; set; }

        [ProtoMember(9, IsRequired = true)]
        public Decimal QuantidadeNetAlocada { get; set; }

        
        public RestrictionGlobalInfo()
        {
            this.Account = -1;
            this.LimiteVolumeNet = decimal.Zero;
            this.QuantidadeNet = decimal.Zero;
            this.LimiteMaxOfertaVolume = decimal.Zero;
            this.LimiteMaxOfertaQtde = decimal.Zero;
            this.StAtivo = false;
            this.DtAtualizacao = DateTime.MinValue;
            this.VolumeNetAlocado = decimal.Zero;
            this.QuantidadeNetAlocada = decimal.Zero;
            this.VolumeNetAlocado = decimal.Zero;
        }
    }
}
