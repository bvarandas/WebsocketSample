using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.SupervisorRisco.Lib.Dados
{
    public class TOPosClient
    {
        public PosClientSymbolInfo PositionClient { get; set; }
        public TOPosClient()
        {
            this.PositionClient = null;
        }
    }
}
