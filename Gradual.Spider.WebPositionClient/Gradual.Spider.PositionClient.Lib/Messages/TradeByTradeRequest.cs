using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gradual.OMS.Library;

namespace Gradual.Spider.PositionClient.Lib.Messages
{
    /// <summary>
    /// Classe de request para a info de TradeByTrade
    /// </summary>
    public class TradeByTradeRequest : MensagemRequestBase
    {

        #region Propriedades
        /// <summary>
        /// Código do Cliente
        /// </summary>
        public int Account { get; set; }

        /// <summary>
        /// Data de filtro "DE"
        /// </summary>
        public DateTime De { get; set; }

        /// <summary>
        /// Data de filtro "ATÉ"
        /// </summary>
        public DateTime Ate { get; set; }
        #endregion

    }
}
