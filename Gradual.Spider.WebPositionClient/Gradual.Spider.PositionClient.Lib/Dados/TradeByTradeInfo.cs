using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gradual.Spider.PositionClient.Lib
{
    /// <summary>
    /// Classe info do objeto de Trade by Trade para fluxo de informações de objetos através das camadas
    /// de banco de dados e mensagens
    /// </summary>
    public class TradeByTradeInfo
    {
        /// <summary>
        /// Valor de corretagem da gradual
        /// </summary>
        public decimal GradualFees      {get; set;}

        /// <summary>
        /// Impostos e taxas da negociação
        /// </summary>
        public decimal ExchangeFees      { get; set; }

        /// <summary>
        /// Campo de designation
        /// </summary>
        public string Designation       { get; set; }
        
        /// <summary>
        /// Operador DMA da operação.
        /// </summary>
        public string OperadorDMA       { get; set; }

        /// <summary>
        /// TRade Id da Operação 
        /// </summary>
        public string TradeID           { get; set; }
        
        /// <summary>
        /// Código do cliente
        /// </summary>
        public int Account              { get; set; }
        
        /// <summary>
        /// Ativo 
        /// </summary>
        public string Ativo             { get; set; }
        
        /// <summary>
        /// Bolsa
        /// </summary>
        public string Bolsa             { get; set; }
        
        /// <summary>
        /// Código da carteira
        /// </summary>
        public int CodCarteira          { get; set; }
        
        /// <summary>
        /// Papel base- Nocaso de Mercado a vista, o  papel base é o ativo da opção.
        /// No caso de opção, o papel base é o ativo em qual a opção trabalha como base.
        /// </summary>
        public string CodPapelObjeto    { get; set; }
        
        /// <summary>
        /// Data da ultima movimentação 
        /// </summary>
        public DateTime DtMovimento     { get; set; }
        
        /// <summary>
        /// Data que a posição fo aberta
        /// </summary>
        public DateTime DtPosicao       { get; set; }
        
        /// <summary>
        /// Data de vencimento da opção ou bmf
        /// </summary>
        public DateTime DtVencimento    { get; set; }
        
        /// <summary>
        /// Financeiro Net é o valor de quanto o o cliente está netado em valores financeiros
        /// </summary>
        public decimal FinancNet        { get; set; }
        
        /// <summary>
        /// Lucro Prejuízo é quanto o cliente está tendo de lucro ou prejuízo em cima dessa posição
        /// </summary>
        public decimal LucroPrej        { get; set; }
        
        /// <summary>
        /// Net das quantidades de abertura
        /// </summary>
        public decimal NetAb            { get; set; }
        
        /// <summary>
        /// Net das quantidade executadas no intraday
        /// </summary>
        public decimal NetExec          { get; set; }
        
        /// <summary>
        /// Preço médio de compra
        /// </summary>
        public decimal PcMedC           { get; set; }
        
        /// <summary>
        /// Preço médio de venda
        /// </summary>
        public decimal PcMedV           { get; set; }
        
        /// <summary>
        /// Preço de fechamento do papel
        /// </summary>
        public decimal PrecoFechamento  { get; set; }
        
        /// <summary>
        /// Quantidade de aberta de compra
        /// </summary>
        public decimal QtdAbC           { get; set; }
        
        /// <summary>
        /// Quantidade de abertura
        /// </summary>
        public decimal QtdAbertura      { get; set; }
        
        /// <summary>
        /// Quantidade de aberto de venda
        /// </summary>
        public decimal QtdAbV           { get; set; }
        
        /// <summary>
        /// Quantidade D1
        /// </summary>
        public decimal QtdD1            { get; set; }
        
        /// <summary>
        /// Quantidade D2
        /// </summary>
        public decimal QtdD2            { get; set; }
        
        /// <summary>
        /// Quantidade D3
        /// </summary>
        public decimal QtdD3            { get; set; }
        
        /// <summary>
        /// Quantidade executada de compra
        /// </summary>
        public decimal QtdExecC         { get; set; }
        
        /// <summary>
        /// Quantidade executada de venda
        /// </summary>
        public decimal QtdExecV         { get; set; }

        /// <summary>
        /// Segmento de mercado
        /// </summary>
        public string SegmentoMercado   { get; set; }

        /// <summary>
        /// Tipo de mercado
        /// </summary>
        public string TipoMercado       { get; set; }
        
        /// <summary>
        /// Ultimo Preço de Cotação
        /// </summary>
        public decimal UltPreco         { get; set; }
        
        /// <summary>
        /// Variação do preço de mercado
        /// </summary>
        public decimal Variacao         { get; set; }
    }
}
