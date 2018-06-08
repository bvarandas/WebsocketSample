using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.SupervisorRisco.Lib.Dados
{
    /// <summary>
    /// Classe para gerenciar 
    /// </summary>
    [ProtoContract]
    [Serializable]
    public class PosClientSymbolDetailInfo
    {
        [ProtoMember(1 , IsRequired = true)]
        public string ClOrdID       { get; set; }

        [ProtoMember(2, IsRequired = true)]
        public int CodigoCliente    { get; set; }

        [ProtoMember(3, IsRequired = true)]
        public string Instrumento   { get; set; }

        [ProtoMember(4, IsRequired = true)]
        public decimal LP           { get; set; }

        [ProtoMember(5, IsRequired = true)]
        public decimal PrecoMercado { get; set; }

        [ProtoMember(6, IsRequired = true)]
        public decimal PrecoCliente { get; set; }

        [ProtoMember(7, IsRequired = true)]
        public decimal Qtde         { get; set; }

        [ProtoMember(8, IsRequired = true)]
        public string Sentido       { get; set; }

        [ProtoMember(9, IsRequired = true)]
        public decimal TotalMercado { get; set; }

        [ProtoMember(10,  IsRequired = true)]
        public decimal TotalCliente { get; set; }

        [ProtoMember(11,  IsRequired = true)]
        public string Horario       { get; set; }

        [ProtoMember(12, IsRequired = true)]
        public string Bolsa         { get; set; }

    }
}
