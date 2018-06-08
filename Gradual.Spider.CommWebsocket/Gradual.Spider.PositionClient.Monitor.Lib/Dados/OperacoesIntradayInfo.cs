using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Gradual.Spider.PositionClient.Monitor.Lib.Dados
{
    /// <summary>
    /// Classe de gerencimaneto de dados de Operações intraday para preencher as tela de 
    /// gerenciamento de risco de Operações Intraday
    /// </summary>
    [DataContract]
    public class OperacoesIntradayInfo
    {
        /// <summary>
        /// Código do cliente Bovespa
        /// </summary>
        [DataMember]
        public int CodigoClienteBovespa { get; set; }
        
        /// <summary>
        /// Código de intrumento operado
        /// </summary>
        [DataMember]
        public string CodigoInstrumento { get; set;}   
        
        /// <summary>
        /// Código do tipo de mercado Vis, DIs, Fut, Oopc, Opv, etc...
        /// </summary>
        [DataMember]
        public string Mercado { get; set;} 
        
        /// <summary>
        /// Data do vencimento do Instrumento
        /// </summary>
        [DataMember]
        public DateTime Vencimento {get; set;}
        
        /// <summary>
        /// Quantidade de abertura do Instrumento
        /// </summary>
        [DataMember]
        public int QuantAbertura {get; set;}
        
        /// <summary>
        /// Quantidade total - [quantidade abertura + Quantidade Operada no intraday]
        /// </summary>
        [DataMember]
        public int QuantTotal {get; set;}
        
        /// <summary>
        /// Lucro Prejuízo atual  - 
        /// Somatória (Preço de execução Compra * Quantidade executada compra) -
        /// Somatória (Preço de Execução Venda * Quantidade Executada Venda)
        /// </summary>
        [DataMember]
        public decimal PL {get; set;}

        /// <summary>
        /// Cotacao atual do papel
        /// </summary>
        [DataMember]
        public decimal Cotacao { get; set; }

        /// <summary>
        /// Quantidade Executada de compra
        /// </summary>
        [DataMember]
        public int QuantExecutadaCompra{get; set;}
        
        /// <summary>
        /// Quantidade executada de venda
        /// </summary>
        [DataMember]
        public int QuantExecutadaVenda{get; set;}
        
        /// <summary>
        /// Net Quantidade executada
        /// (Quantidade executada de compra - quantidade executada de venda)
        /// </summary>
        [DataMember]
        public int QuantExecutadaNet {get; set;} 
        
        /// <summary>
        /// Quantidade aberta de compra para o instrumento.
        /// Quantidade aberta no book ou quantidade remanescente de ordem parcialmente executada
        /// </summary>
        [DataMember]
        public int QuantAbertaCompra{get; set;}

        /// <summary>
        /// Quantidade aberta de Venda para o instrumento.
        /// Quantidade aberta no book ou quantidade remanescente de ordem parcialmente executada
        /// </summary>
        [DataMember]
        public int QuantAbertaVenda {get; set;}

        /// <summary>
        /// Net da Quantidade aberta 
        /// (Quantidade aberta de compra - quantidade aberta de venda)
        /// </summary>
        [DataMember]
        public int QuantAbertaNet {get; set;}
        
        /// <summary>
        /// Preço médio ponderado de todas as execuções de compra, incluindo as execuções parciais
        /// </summary>
        [DataMember]
        public decimal PrecoMedioCompra {get; set;} 
        
        /// <summary>
        /// Preço médio ponderado de todas as execuções de venda, incluindo as execuções parciais
        /// </summary>
        [DataMember]
        public decimal PrecoMedioVenda{get; set;} 
        
        /// <summary>
        /// Volume acumulado de todas as compras executadas e parcialmente executadas para um determinado ativo
        /// </summary>
        [DataMember]
        public decimal VolumeCompra {get; set;}

        /// <summary>
        /// Volume acumulado de todas as Vendas executadas e parcialmente executadas para um determinado ativo
        /// </summary>
        [DataMember]
        public decimal VolumeVenda {get; set;}

        /// <summary>
        /// Net Volume do determinado ativo [Volume de Venda - Volume de Compra]
        /// </summary>
        [DataMember]
        public decimal VolumeNet { get; set; }

        /// <summary>
        /// Volume Total de umdetermindado ativo [Volume de Venda + Volume de Compra]
        /// </summary>
        [DataMember]
        public decimal VolumeTotal { get; set; }
    }
}
