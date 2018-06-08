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
    public class SymbolListSyncMsg
    {
        [ProtoMember(1)]
        public SyncMsgAction SyncAction { get; set; }

        [ProtoMember(2)]
        public ConcurrentDictionary<string, SymbolInfo> Symbols { get; set; }

        public SymbolListSyncMsg()
        {
            Symbols = new ConcurrentDictionary<string, SymbolInfo>();
        }
    }
}
