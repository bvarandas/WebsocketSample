using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Gradual.Spider.SupervisorRisco.Lib.Dados
{
    [Serializable]
    [ProtoContract]
    public class ConsolidatedRiskInfo
    {
        [ProtoMember(1, IsRequired = true)]
        public int Account { get; set; }
        [ProtoMember(2, IsRequired = true)]
        public decimal TotalCustodiaAbertura { get; set; }
        [ProtoMember(3, IsRequired = true)]
        public decimal TotalContaCorrenteOnline { get; set; }
        [ProtoMember(4, IsRequired = true)]
        public decimal TotalGarantias { get; set; }
        [ProtoMember(5, IsRequired = true)]
        public decimal TotalProdutos { get; set; }
        [ProtoMember(6, IsRequired = true)]
        public decimal SaldoTotalAbertura { get;set;}
        [ProtoMember(7, IsRequired = true)]
        public decimal PLBovespa { get; set; }
        [ProtoMember(8, IsRequired = true)]
        public decimal PLBmf { get; set; }
        [ProtoMember(9, IsRequired = true)]
        public decimal PLTotal { get; set; }
        [ProtoMember(10, IsRequired = true)]
        public decimal SFP { get; set; }
        [ProtoMember(11, IsRequired = true)]
        public decimal TotalPercentualAtingido { get; set; }
        [ProtoMember(12, IsRequired = true)]
        public DateTime DtMovimento { get; set; }
        
        [ProtoMember(13, IsRequired = true)]
        public decimal TotalCustodiaBvsp { get; set; }
        
        [ProtoMember(14, IsRequired = true)]
        public decimal TotalCustodiaBmf { get; set; }
        
        [ProtoMember(15, IsRequired = true)]
        public decimal TotalCustodiaTesouroDireto { get; set; }

        [ProtoMember(16, IsRequired = true)]
        public decimal TotalCustodiaAberturaFixa { get; set; }

        [ProtoMember(17, IsRequired = true)]
        public string NomeCliente { get; set; }

        [ProtoMember(18, IsRequired = true)]
        public int CodigoAssessor { get; set; }

        [ProtoMember(19, IsRequired = true)]
        public decimal PLBovespaSemAbertura { get; set; }

        [ProtoMember(20, IsRequired = true)]
        public bool OperouIntraday { get; set; }

        [ProtoMember(21, IsRequired = true)]
        public bool Zerado { get; set; }

        [ProtoMember(22, IsRequired=true)]
        public DateTime HorarioCalculo { get; set; }

        [ProtoMember(23, IsRequired = true)]
        public decimal TotalContaCorrenteAbertura { get; set; }
        
        [ProtoMember(24, IsRequired= true)]
        public decimal TotalBtcTomador { get; set; }

        [ProtoMember(25, IsRequired = true)]
        public decimal TotalGarantiaARemoverCustodia { get; set; }

        public ConsolidatedRiskInfo()
        {
            this.Account                        = 0;
            this.TotalCustodiaAbertura          = decimal.Zero;
            this.TotalContaCorrenteOnline       = decimal.Zero;
            this.TotalGarantias                 = decimal.Zero;
            this.TotalProdutos                  = decimal.Zero;
            this.SaldoTotalAbertura             = decimal.Zero;
            this.PLBovespa                      = decimal.Zero;
            this.PLBmf                          = decimal.Zero;
            this.PLTotal                        = decimal.Zero;
            this.SFP                            = decimal.Zero;
            this.TotalPercentualAtingido        = decimal.Zero;
            this.DtMovimento                    = DateTime.MinValue;
            this.TotalCustodiaBvsp              = decimal.Zero;
            this.TotalCustodiaBmf               = decimal.Zero;
            this.TotalCustodiaTesouroDireto     = decimal.Zero;
            this.TotalCustodiaAberturaFixa      = decimal.Zero;
            this.CodigoAssessor                 = 0;
            this.PLBovespaSemAbertura           = decimal.Zero;
            this.OperouIntraday                 = false;
            this.Zerado                         = true;
            this.HorarioCalculo                 = DateTime.MinValue;
            this.TotalContaCorrenteAbertura     = decimal.Zero;
            this.TotalBtcTomador                = decimal.Zero;
            this.TotalGarantiaARemoverCustodia  = decimal.Zero;
        }
    }
}
