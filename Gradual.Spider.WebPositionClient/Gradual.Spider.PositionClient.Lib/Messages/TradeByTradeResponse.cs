using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gradual.OMS.Library;

namespace Gradual.Spider.PositionClient.Lib.Messages
{
    /// <summary>
    /// Classe de Response do info de TradeByTRade
    /// </summary>
    public class TradeByTradeResponse : MensagemResponseBase
    {
        #region Propriedades
        /// <summary>
        /// Lista de Objetos de TradeByTrade
        /// </summary>
        public List<TradeByTradeInfo> ListaTradeByTrade { get; set; }
        #endregion

        #region Construtores
        /// <summary>
        /// Construtor da classe para inicialização
        /// </summary>
        public TradeByTradeResponse()
        {
            ListaTradeByTrade = new List<TradeByTradeInfo>();
        }
        #endregion
    }
}
