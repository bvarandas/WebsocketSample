using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.Spider.SupervisorRisco.Lib.Dados;
using ProtoBuf;
using System.Collections.Concurrent;

namespace Gradual.Spider.DataSync.Lib.Mensagens
{
    [Serializable]
    [ProtoContract]
    public class TestSymbolSyncMsg
    {
        [ProtoMember(1)]
        public SyncMsgAction SyncAction { get; set; }

        [ProtoMember(2)]
        public ConcurrentDictionary<string,TestSymbolInfo> TestSymbols { get; set; }

        public TestSymbolSyncMsg()
        {
            TestSymbols = new ConcurrentDictionary<string, TestSymbolInfo>();
        }
    }
}
