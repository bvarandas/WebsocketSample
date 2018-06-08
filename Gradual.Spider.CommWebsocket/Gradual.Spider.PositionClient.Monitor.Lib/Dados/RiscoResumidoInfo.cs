using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Gradual.Spider.PositionClient.Monitor.Lib.Dados
{
    /// <summary>
    /// Classe de gerenciamento da info de risco resumido para preenchimento da tela de risco resumido
    /// </summary>
    [DataContract]
    public class RiscoResumidoInfo
    {
        /// <summary>
        /// Código do cliente Bovespa
        /// </summary>
        [DataMember]
        public int CodigoClienteBovespa {get; set;}

        /// <summary>
        /// Código de cliente BMF
        /// </summary>
        [DataMember]
        public int CodigoClienteBmf { get; set; }

        /// <summary>
        /// Total de Custódia de abertura 
        /// </summary>
        [DataMember]
        public decimal CustodiaAbertura {get; set;}

        /// <summary>
        /// Total da conta corrente
        /// </summary>
        [DataMember]
        public decimal CCAbertura {get; set;}

        /// <summary>
        /// Total de Garantias
        /// </summary>
        [DataMember]
        public decimal Garantias {get; set;}

        /// <summary>
        /// Total de Produtos 
        /// </summary>
        [DataMember]
        public decimal Produtos {get; set;}

        /// <summary>
        /// Total de abertura [Custodia de abertura + CC abertura + Garantias + Produtos]
        /// </summary>
        [DataMember]
        public decimal TotalAbertura{get; set;}

        /// <summary>
        /// Custo de reversão de otodos os ativos posicionados no mercado de bovespa
        /// </summary>
        [DataMember]
        public decimal PLBovespa {get; set;}

        /// <summary>
        /// Custo de reversão de todos os ativos posicionados no mercado de bmf
        /// </summary>
        [DataMember]
        public decimal PLBmf {get; set;}

        /// <summary>
        /// Total das somas dos valores de Pl de bovespa e PL de bmf [PLBovespa + PlBmf]
        /// </summary>
        [DataMember]
        public decimal PLTotal{ get; set;}

        /// <summary>
        /// Situação financeira patrimonial do cliente
        /// </summary>
        [DataMember]
        public decimal SFP {get; set;}

        /// <summary>
        /// Percentual do prejuízo atingido
        /// </summary>
        [DataMember]
        public decimal PercAtingido { get; set; }
    }
}
