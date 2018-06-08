using Gradual.OMS.Library;
using Gradual.Spider.PositionClient.Monitor.Lib.Dados;
using Gradual.Spider.SupervisorRisco.Lib.Dados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Gradual.Spider.PositionClient.Monitor.Lib.Message
{
    [DataContract]
    [Serializable]
    public class BuscarOperacoesIntradayResponse : MensagemResponseBase
    {
        [DataMember]
        public List<PosClientSymbolInfo> ListOperacoesIntraday { get; set; }
    }
}
