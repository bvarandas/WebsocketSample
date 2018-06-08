using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gradual.OMS.Library;
using Gradual.Spider.PositionClient.Lib.Dados;

namespace Gradual.Spider.PositionClient.Lib.Messages
{
    public class SymbolResponse : MensagemResponseBase
    {
        #region Properties
        public ConcurrentDictionary<string, SymbolInfo> ListaPapel { get; set; }
        #endregion


        #region Construtores
        public SymbolResponse()
        {
            ListaPapel = new ConcurrentDictionary<string, SymbolInfo>();
        }
        #endregion


    }
}
