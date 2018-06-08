using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;


namespace Gradual.Spider.SupervisorRisco.Lib.Dados
{
    [Serializable]
    [ProtoContract]
    public class SymbolInfo
    {
        [ProtoMember(1)]
        public string Instrumento { get; set; }

        [ProtoMember(2, IsRequired=true)]
        public int FormaCotacao { get; set; }

        [ProtoMember(3, IsRequired = true)]
        public int LotePadrao { get; set; }

        [ProtoMember(4)]
        public DateTime DtNegocio { get; set; }

        [ProtoMember(5)]
        public DateTime DtAtualizacao { get; set; }

        [ProtoMember(6)]
        public decimal VlrUltima;

        [ProtoMember(7)]
        public decimal VlrOscilacao;

        [ProtoMember(8)]
        public decimal VlrFechamento;

        [ProtoMember(9,IsRequired = true)]
        public SegmentoMercadoEnum SegmentoMercado { get; set; }

        [ProtoMember(10, IsRequired = true)]
        public string GrupoCotacao { get; set; }

        [ProtoMember(11, IsRequired = true)]
        public decimal CoeficienteMultiplicacao { get; set; }

        [ProtoMember(12, IsRequired = true)]
        public string IndicadorOpcao { get; set; }
        
        [ProtoMember(13, IsRequired = true)]
        public string SegmentoMercadoValor{ get; set; }

        [ProtoMember(14, IsRequired = true)]
        public DateTime DtVencimento { get; set; }

        [ProtoMember(15, IsRequired = true)]
        public string DescAtivo { get; set; }

        [ProtoMember(16, IsRequired = true)]
        public string CodigoPapelObjeto { get; set; }

        [ProtoMember(17, IsRequired = true)]
        public string CodigoISIN { get; set; }

        [ProtoMember(18, IsRequired = true)]
        public decimal VlrAjuste { get; set; }


        public SymbolInfo()
        {
            this.Instrumento = string.Empty;
            this.FormaCotacao = -1;
            this.LotePadrao = -1;
            this.DtNegocio = DateTime.MinValue;
            this.DtAtualizacao = DateTime.MinValue;
            this.VlrUltima = decimal.Zero;
            this.VlrOscilacao = decimal.Zero;
            this.VlrFechamento = decimal.Zero;
            this.SegmentoMercado = SegmentoMercadoEnum.INDEFINIDO;
            this.GrupoCotacao = string.Empty;
            this.CoeficienteMultiplicacao = decimal.One;
            this.IndicadorOpcao = string.Empty;
            this.SegmentoMercadoValor = string.Empty;
            this.DtVencimento = DateTime.MinValue;
            this.DescAtivo = string.Empty;
            this.CodigoPapelObjeto = string.Empty;
            this.CodigoISIN = string.Empty;
            this.VlrAjuste = decimal.Zero;
        }

        ~SymbolInfo()
        {
        }

    }
}
