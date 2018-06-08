using Gradual.Spider.SupervisorRisco.Lib.Dados;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Gradual.Spider.PositionClient.Monitor.Transporte
{
    /// <summary>
    /// Classe responsável pela camada de transporte de operações intraday
    /// </summary>
    public class TransporteOperacoesIntraday
    {
        private CultureInfo gCultura = new CultureInfo("en-US");

        /// <summary>
        /// Código de cliente
        /// </summary>
        public int CodigoCliente = 0;

        /// <summary>
        /// Código de instrumento
        /// </summary>
        public string CodigoInstrumento = string.Empty;

        /// <summary>
        /// Tipo de Mercado
        /// </summary>
        public string Mercado = string.Empty;

        /// <summary>
        /// Vencimento do papel
        /// </summary>
        public DateTime Vencimento = DateTime.MinValue;

        /// <summary>
        /// Quantidade de abertura do papel do cliente
        /// </summary>
        public decimal QuantAbertura = 0;

        /// <summary>
        /// Quantidade total [quantAbertura + Net Total Operado]
        /// </summary>
        public decimal QuantTotal = 0;

        /// <summary>
        /// Lucro prejuízo calculado do cliente
        /// </summary>
        public decimal PL = 0;

        /// <summary>
        /// Quantidade executada de compra do papel do cliente
        /// </summary>
        public decimal QuantExecutadaCompra = 0;

        /// <summary>
        /// Quantidade executada de venda do papel do cliente
        /// </summary>
        public decimal QuantExecutadaVenda = 0;
        public decimal QuantExecutadaNet = 0;
        public decimal QuantAbertaCompra = 0;
        public decimal QuantAbertaVenda = 0;
        public decimal QuantAbertaNet = 0;
        public decimal PrecoMedioCompra = 0;
        public decimal PrecoMedioVenda = 0;
        public decimal VolumeCompra = 0;
        public decimal VolumeVenda = 0;
        public decimal VolumeTotal = 0;
        public string NomeCliente = string.Empty;
        public decimal Cotacao = 0;
        
        public string Bolsa = "";

        public List<TransporteOperacoesIntradayDet> TradeDetail = new List<TransporteOperacoesIntradayDet>();

        public List<TransporteOperacoesIntraday> ListaTransporte = new List<TransporteOperacoesIntraday>();

        public TransporteOperacoesIntraday() { }

        public TransporteOperacoesIntraday(List<PosClientSymbolInfo> pLista)
        {
            TransporteOperacoesIntraday lTrans = new TransporteOperacoesIntraday();
            try
            {
                foreach (PosClientSymbolInfo pos in pLista)
                {
                    lTrans = new TransporteOperacoesIntraday();

                    lTrans.CodigoCliente        = pos.Account;
                    lTrans.CodigoInstrumento    = pos.Ativo;
                    lTrans.Mercado              = pos.TipoMercado;
                    lTrans.Vencimento           = pos.DtVencimento;

                    if (pos.Ativo == "OZ1D")
                    {
                        lTrans.QuantAbertura    = decimal.Parse(pos.QtdAbertura.ToString());
                        lTrans.QuantTotal       = decimal.Parse((pos.QtdAbertura + pos.NetExec).ToString());
                    }
                    else
                    {
                        lTrans.QuantAbertura    = int.Parse(pos.QtdAbertura.ToString());
                        lTrans.QuantTotal       = int.Parse((pos.QtdAbertura + pos.NetExec).ToString());
                    }
                    lTrans.PL                   = pos.LucroPrej;
                    lTrans.QuantExecutadaCompra = pos.QtdExecC;
                    lTrans.QuantExecutadaVenda  = pos.QtdExecV;
                    lTrans.QuantExecutadaNet    = pos.NetExec;
                    lTrans.QuantAbertaCompra    = pos.QtdAbC;
                    lTrans.QuantAbertaVenda     = pos.QtdAbV;
                    lTrans.QuantAbertaNet       = pos.NetAb;
                    lTrans.PrecoMedioCompra     = pos.PcMedC;
                    lTrans.PrecoMedioVenda      = pos.PcMedV;
                    lTrans.VolumeCompra         = pos.VolCompra; //(pos.QtdExecC).ToString();
                    lTrans.VolumeVenda          = pos.VolVenda; //pos.QtdExecV.ToString();
                    lTrans.VolumeTotal          = pos.VolTotal; //(pos.QtdExecC + pos.QtdExecV).ToString();
                    lTrans.NomeCliente          = pos.NomeCliente;
                    lTrans.Cotacao              = pos.UltPreco;
                    lTrans.Bolsa                = pos.Bolsa;
                    
                    pos.TradeDetails.ForEach(trade => 
                    { 
                        var lDet = new TransporteOperacoesIntradayDet();
                        
                        lDet.CodigoCliente  =   trade.CodigoCliente;
                        lDet.Instrumento    =   trade.Instrumento ;
                        lDet.LP             =   trade.LP;
                        lDet.PrecoMercado   =   trade.PrecoMercado;
                        lDet.PrecoCliente   =   trade.PrecoCliente;
                        lDet.Qtde           =   trade.Qtde;
                        lDet.Sentido        =   trade.Sentido;
                        lDet.TotalMercado   =   trade.TotalMercado;
                        lDet.TotalCliente   =   trade.TotalCliente;
                        lDet.Horario        =   trade.Horario;
                        lDet.Diferencial    =   (trade.PrecoMercado - trade.PrecoCliente);

                        lTrans.TradeDetail.Add(lDet);
                    });

                    ListaTransporte.Add(lTrans);
                }
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }
    }

    /// <summary>
    /// Classe para gerenciar os detalhes(negociações) da posição do papel do cliente intraday
    /// </summary>
    public class TransporteOperacoesIntradayDet
    {
        public int      CodigoCliente    { get; set; }
        public string   Instrumento   { get; set; }
        public decimal LP           { get; set; }
        public decimal PrecoMercado { get; set; }
        public decimal PrecoCliente { get; set; }
        public decimal Qtde         { get; set; }
        public string   Sentido       { get; set; }
        public decimal TotalMercado { get; set; }
        public decimal TotalCliente { get; set; }
        public string   Horario       { get; set; }
        public decimal Diferencial { get; set; }

        public List<TransporteOperacoesIntradayDet> ListaTransporte = new List<TransporteOperacoesIntradayDet>();

        public TransporteOperacoesIntradayDet()
        { }

        public TransporteOperacoesIntradayDet(List<PosClientSymbolDetailInfo> pLista)
        {
            pLista.ForEach(pos=>
            {
                var lTrans = new TransporteOperacoesIntradayDet();

                lTrans.CodigoCliente    = pos.CodigoCliente;
                lTrans.Instrumento      = pos.Instrumento;
                lTrans.LP               = pos.LP;
                lTrans.PrecoMercado     = pos.PrecoMercado;
                lTrans.PrecoCliente     = pos.PrecoCliente;
                lTrans.Qtde             = pos.Qtde;
                lTrans.Sentido          = pos.Sentido;
                lTrans.TotalCliente     = pos.TotalCliente;
                lTrans.TotalMercado     = pos.TotalMercado;
                lTrans.Horario          = pos.Horario;
                lTrans.Diferencial      = (pos.PrecoMercado - pos.PrecoCliente);

                ListaTransporte.Add(lTrans);
            });
        }

    }
}
