using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Gradual.Spider.SupervisorRisco.Lib.Dados
{

    [Serializable]
    [ProtoContract]
    public class TestSymbolInfo
    {
        [ProtoMember(1)]
        public string Exchange { get; set; }

        [ProtoMember(2)]
        public string Instrument { get; set; }

        public TestSymbolInfo()
        {
            this.Exchange = string.Empty;
            this.Instrument = string.Empty;
        }
    }
}
