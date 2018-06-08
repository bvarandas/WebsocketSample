using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.Spider.SupervisorRisco.Lib.Dados;
using System.Collections.Concurrent;

namespace Gradual.Spider.SupervisorRisco.Lib.Handlers
{
    public class PositionClientEventArgs : EventArgs
    {
        public EventAction Action { get; set; }
        public int Account { get; set; }
        public List<PosClientSymbolInfo> PosClient { get; set; }
    }

    public class ConsolidatedRiskEventArgs : EventArgs
    {
        public EventAction Action { get; set; }
        public int Account { get; set; }
        public ConcurrentDictionary<int, ConsolidatedRiskInfo> ConsolidatedRisk { get; set; }
    }
}
