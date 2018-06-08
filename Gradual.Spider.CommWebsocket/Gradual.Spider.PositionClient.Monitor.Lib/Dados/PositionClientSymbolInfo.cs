using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.Spider.SupervisorRisco.Lib.Dados;
using ProtoBuf;

namespace Gradual.Spider.PositionClient.Monitor.Dados
{
    public class PositionClientSymbolInfo : PosClientSymbolInfo
    {
        [ProtoMember(27, IsRequired = true)]
        public string PapelBase { get; set; }
    }
}
