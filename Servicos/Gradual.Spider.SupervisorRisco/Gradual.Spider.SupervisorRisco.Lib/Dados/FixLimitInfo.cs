using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Gradual.Spider.SupervisorRisco.Lib.Dados
{
    [Serializable]
    [ProtoContract]
    public class FixLimitInfo
    {
        [ProtoMember(1, IsRequired=true)]
        public int IdSessaoFix { get; set; }
        [ProtoMember(2, IsRequired = true)]
        public string Descricao { get; set; }
        [ProtoMember(3, IsRequired = true)]
        public decimal VlDisponivel { get; set; }
        [ProtoMember(4, IsRequired = true)]
        public decimal VlTotal { get; set; }
        [ProtoMember(5, IsRequired = true)]
        public decimal VlMvto { get; set; }
        [ProtoMember(6, IsRequired = true)]
        public int Side { get; set; }
        [ProtoMember(7, IsRequired = true)]
        public char Status { get; set; }
        
        public FixLimitInfo()
        {
            this.IdSessaoFix = 0;
            this.Descricao = string.Empty;
            this.VlDisponivel = decimal.Zero;
            this.VlTotal = decimal.Zero;
            this.VlMvto = decimal.Zero;
            this.Side = 0;
            this.Status = ' ';
        }
    }
}
