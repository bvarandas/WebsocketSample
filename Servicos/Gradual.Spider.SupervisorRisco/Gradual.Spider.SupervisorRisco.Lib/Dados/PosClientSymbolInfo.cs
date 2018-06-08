using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using Gradual.Core.Spider.OrderFixProcessing.Lib.Dados;
using System.Collections.Concurrent;

namespace Gradual.Spider.SupervisorRisco.Lib.Dados
{
    [Serializable]
    [ProtoContract]
    public class PosClientSymbolInfo
    {
        [ProtoMember(1, IsRequired = true)]
        public int Account { get; set; }
        [ProtoMember(2, IsRequired = true)]
        public string Ativo { get; set; }
        [ProtoMember(3, IsRequired = true)]
        public decimal Variacao { get; set; }
        [ProtoMember(4, IsRequired = true)]
        public decimal UltPreco { get;set; }
        [ProtoMember(5, IsRequired = true)]
        public decimal PrecoFechamento { get; set; }
        [ProtoMember(6, IsRequired = true)]
        public decimal QtdAbertura { get; set; }
        [ProtoMember(7, IsRequired = true)]
        public decimal QtdExecC { get; set; }
        [ProtoMember(8, IsRequired = true)]
        public decimal QtdExecV { get; set; }
        [ProtoMember(9, IsRequired = true)]
        public decimal NetExec { get; set; }
        [ProtoMember(10, IsRequired = true)]
        public decimal QtdAbC { get; set; }
        [ProtoMember(11, IsRequired = true)]
        public decimal QtdAbV { get; set; }
        [ProtoMember(12, IsRequired = true)]
        public decimal NetAb { get; set; }
        [ProtoMember(13, IsRequired = true)]
        public decimal PcMedC { get; set; }
        [ProtoMember(14, IsRequired = true)]
        public decimal PcMedV { get; set; }
        [ProtoMember(15, IsRequired = true)]
        public decimal FinancNet { get; set; }
        [ProtoMember(16, IsRequired = true)]
        public decimal LucroPrej { get; set; }
        
        [ProtoMember(17, IsRequired = true)]
        public DateTime DtPosicao { get; set; }
        
        [ProtoMember(18, IsRequired = true)]
        public DateTime DtMovimento { get; set; }

        [ProtoMember(19, IsRequired = true)]
        public decimal QtdD1 { get; set; }

        [ProtoMember(20, IsRequired = true)]
        public decimal QtdD2 { get; set; }
        
        [ProtoMember(21, IsRequired = true)]
        public decimal QtdD3 { get; set; }

        [ProtoMember(22, IsRequired = true)]
        public SegmentoMercadoEnum SegmentoMercado{ get; set; }

        [ProtoMember(23, IsRequired = true)]
        public string Bolsa { get; set; }
        
        [ProtoMember(24, IsRequired = true)]
        public int CodCarteira { get; set; }

        [ProtoMember(25, IsRequired = true)]
        public string TipoMercado { get; set; }

        [ProtoMember(26, IsRequired = true)]
        public DateTime DtVencimento { get; set; }
        
        [ProtoMember(27, IsRequired = true)]
        public string CodPapelObjeto { get; set; }

        [ProtoMember(28, IsRequired = true)]
        public string ExecBroker {get;set;}

        [ProtoMember(29, IsRequired = true)]
        public long MsgId { get; set; }

        [ProtoMember(30, IsRequired = true)]
        public string EventSource { get; set; }

        [ProtoMember(31, IsRequired = true)]
        public decimal QtdTotal { get; set; }

        [ProtoMember(32, IsRequired = true)]
        public decimal QtdDisponivel { get; set; }

        [ProtoMember(33, IsRequired = true)]
        public SpiderOrderInfo OrderInfoCalc;

        [ProtoMember(34, IsRequired = true)]
        public decimal VolCompra { get; set; }

        [ProtoMember(35, IsRequired = true)]
        public decimal VolVenda { get; set; }

        [ProtoMember(36, IsRequired = true)]
        public decimal VolTotal{ get; set; }

        [ProtoMember(37, IsRequired = true)]
        public PositionTypeEnum TypePosition { get; set; }
        
        [ProtoMember(38, IsRequired = true)]
        public decimal FinancAbertura {get; set;}

        [ProtoMember(39, IsRequired = true)]
        public string NomeCliente { get; set; }

        [ProtoMember(40, IsRequired = true)]
        public decimal LucroPrejuizoSemAbertura { get; set; }

        [ProtoMember(41, IsRequired = true, OverwriteList=true)]
        public List<PosClientSymbolDetailInfo> TradeDetails { get; set; }

        public PosClientSymbolInfo()
        {
            this.Account = -1;
            this.Ativo = string.Empty;
            this.Variacao = decimal.Zero;
            this.UltPreco = decimal.Zero;
            this.PrecoFechamento = decimal.Zero;
            this.QtdAbertura = decimal.Zero;
            this.QtdExecC = decimal.Zero;
            this.QtdExecV = decimal.Zero;
            this.NetExec = decimal.Zero;
            this.QtdAbC = decimal.Zero;
            this.QtdAbV = decimal.Zero;
            this.NetAb = decimal.Zero;
            this.PcMedC = decimal.Zero;
            this.PcMedV = decimal.Zero;
            this.FinancNet = decimal.Zero;
            this.LucroPrej = decimal.Zero;
            // this.ValOuro = decimal.Zero;
            // this.ValTesouro = decimal.Zero;
            this.DtPosicao = DateTime.MinValue;
            this.DtMovimento = DateTime.MinValue;
            this.QtdD1 = decimal.Zero;
            this.QtdD2 = decimal.Zero;
            this.QtdD3 = decimal.Zero;
            this.SegmentoMercado = SegmentoMercadoEnum.INDEFINIDO;
            this.Bolsa = string.Empty;
            this.CodCarteira = 0;
            this.TipoMercado = string.Empty;

            this.DtVencimento = DateTime.MinValue;

            this.CodPapelObjeto = string.Empty;
            this.ExecBroker = string.Empty;

            this.MsgId = 0;
            this.EventSource = string.Empty;

            this.QtdTotal = decimal.Zero;
            this.QtdDisponivel = decimal.Zero;

            this.OrderInfoCalc = null;

            this.VolTotal = decimal.Zero;
            this.VolCompra = decimal.Zero;
            this.VolVenda = decimal.Zero;

            this.TypePosition = PositionTypeEnum.Intraday;
            this.FinancAbertura = decimal.Zero;
            this.NomeCliente = string.Empty;
            this.LucroPrejuizoSemAbertura = decimal.Zero;

            this.TradeDetails = new List<PosClientSymbolDetailInfo>();
        }

    }
}
