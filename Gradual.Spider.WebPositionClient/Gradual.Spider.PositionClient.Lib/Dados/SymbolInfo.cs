using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gradual.Spider.PositionClient.Lib.Dados
{
    /// <summary>
    /// Classe de controle de dados de cadastro de papel
    /// </summary>
    public class SymbolInfo
    {
        /// <summary>
        /// Instrumento 
        /// </summary>
        public string Instrumento { get; set; }

        /// <summary>
        /// Fator de cotação
        /// </summary>
        public int FormaCotacao { get; set; }

        /// <summary>
        /// Lote padrão
        /// </summary>
        public int LotePadrao { get; set; }

        /// <summary>
        /// Data do Negócio
        /// </summary>
        public DateTime DtNegocio { get; set; }

        /// <summary>
        /// Data de atualização
        /// </summary>
        public DateTime DtAtualizacao { get; set; }

        /// <summary>
        /// Valor da ultima cotação
        /// </summary>
        public decimal VlrUltima;

        /// <summary>
        /// Valor da oscilação
        /// </summary>
        public decimal VlrOscilacao;

        /// <summary>
        /// Valor de fechamento
        /// </summary>
        public decimal VlrFechamento;

        /// <summary>
        /// Segmento de mercado
        /// </summary>
        public string SegmentoMercado { get; set; }

        /// <summary>
        /// Grupo de cotação 
        /// </summary>
        public string GrupoCotacao { get; set; }

        /// <summary>
        /// Construtor da classe
        /// </summary>
        public SymbolInfo()
        {
            this.Instrumento     = string.Empty;
            this.FormaCotacao    = -1;
            this.LotePadrao      = -1;
            this.SegmentoMercado = string.Empty;
            this.GrupoCotacao    = string.Empty;
        }
    }
}
