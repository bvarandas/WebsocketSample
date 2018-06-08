using Gradual.Spider.SupervisorRisco.Lib.Dados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.PositionClient.Monitor.Transporte
{
    public class TransporteRiscoResumido
    {
        public int     CodigoCliente        { get; set; }
        public decimal CustodiaAbertura     { get; set; }
        public decimal CCAbertura           { get; set; }
        public decimal Garantias            { get; set; }
        public decimal Produtos             { get; set; }
        public decimal TotalAbertura        { get; set; }
        public decimal PLBovespa            { get; set; }
        public decimal PLBmf                { get; set; }
        public decimal PLTotal              { get; set; }
        public decimal SFP                  { get; set; }
        public decimal PercAtingido         { get; set; }
        public string  NomeCliente          { get; set; }

        public decimal PatrimonioAbertura   { get; set; }
        public decimal PatrimonioOnline     { get; set; }
        public decimal CustodiaOnline       { get; set; }
        public decimal PatrimonioDiferenca  { get; set; }
        public decimal CodigoAssessor       { get; set; }
        public decimal PLBovespaSemAbertura { get; set; }
        public decimal TotalBTCTomador      { get; set; }

        public List<TransporteRiscoResumido> ListaTransporte = new List<TransporteRiscoResumido>();

        public TransporteRiscoResumido() { }

        public TransporteRiscoResumido(List<ConsolidatedRiskInfo> pLista)
        {
            TransporteRiscoResumido lTrans = new TransporteRiscoResumido();

            foreach (ConsolidatedRiskInfo pos in pLista)
            {
                lTrans = new TransporteRiscoResumido();

                lTrans.CodigoCliente        = pos.Account; 
                lTrans.PLBmf                = pos.PLBmf ;
                lTrans.PLBovespa            = pos.PLBovespaSemAbertura;
                lTrans.PLTotal              = (pos.PLBovespaSemAbertura + pos.PLBmf);// pos.PLTotal;
                lTrans.TotalAbertura        = pos.SaldoTotalAbertura;
                lTrans.SFP                  = pos.SFP;
                lTrans.CCAbertura           = pos.TotalContaCorrenteOnline;
                lTrans.CustodiaAbertura     = pos.TotalCustodiaAbertura;
                lTrans.Garantias            = pos.TotalGarantias;
                lTrans.PercAtingido         = pos.TotalPercentualAtingido;
                lTrans.Produtos             = pos.TotalProdutos;
                lTrans.NomeCliente          = pos.NomeCliente;

                lTrans.PatrimonioAbertura   = (pos.TotalCustodiaAberturaFixa + pos.TotalGarantias + pos.TotalProdutos + pos.TotalContaCorrenteOnline - pos.TotalBtcTomador);
                lTrans.PatrimonioOnline     = (pos.TotalCustodiaAbertura + pos.PLTotal + pos.TotalContaCorrenteOnline + pos.TotalGarantias + pos.TotalProdutos - pos.TotalBtcTomador);
                lTrans.CustodiaOnline       = (pos.TotalCustodiaAbertura - pos.TotalBtcTomador);
                lTrans.PatrimonioDiferenca  = (lTrans.PatrimonioOnline - lTrans.PatrimonioAbertura);
                lTrans.CodigoAssessor       = pos.CodigoAssessor;
                lTrans.PLBovespaSemAbertura = pos.PLBovespaSemAbertura;
                lTrans.TotalBTCTomador      = pos.TotalBtcTomador;

                ListaTransporte.Add(lTrans);
            }
        }
    }
}
