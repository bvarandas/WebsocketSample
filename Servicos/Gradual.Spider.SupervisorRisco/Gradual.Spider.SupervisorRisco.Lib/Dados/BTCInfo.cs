using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Gradual.Spider.SupervisorRisco.Lib.Dados
{
    /// <summary>
    /// Classes que armazenam operações BTC - Aluguel de Ações
    /// </summary>
    [Serializable]
    public class BTCInfo
    {
        /// <summary>
        /// Código de Cliente
        /// </summary>
        
        public int CodigoCliente { set; get; }

        /// <summary>
        /// Instrumento
        /// </summary>
        
        public string Instrumento { set; get; }

        /// <summary>
        /// Tipo de Contrato
        /// </summary>
        
        public string TipoContrato { set; get; }

        /// <summary>
        /// Data de abertura da operação btc
        /// </summary>
        
        public DateTime DataAbertura { set; get; }

        /// <summary>
        /// Data do vencimento do btc
        /// </summary>
        
        public DateTime DataVencimento { set; get; }

        /// <summary>
        /// Código da Carteira
        /// </summary>
        
        public int Carteira { set; get; }

        /// <summary>
        /// Quantidade 
        /// </summary>
        
        public int Quantidade { set; get; }

        /// <summary>
        /// Preço médio da operação BTC
        /// </summary>
        
        public decimal PrecoMedio { set; get; }

        /// <summary>
        /// Taxa da Operação BTC
        /// </summary>
        
        public decimal Taxa { set; get; }

        /// <summary>
        /// Preço de Mercado da Operação
        /// </summary>
        
        public decimal PrecoMercado { set; get; }

        /// <summary>
        /// Remuneração da operação BTC
        /// </summary>
        
        public decimal Remuneracao { set; get; }
    }
}
