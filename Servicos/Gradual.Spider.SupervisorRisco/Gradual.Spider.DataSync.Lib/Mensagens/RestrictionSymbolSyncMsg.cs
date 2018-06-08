using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using Gradual.Spider.SupervisorRisco.Lib.Dados;
using System.Collections.Concurrent;

namespace Gradual.Spider.DataSync.Lib.Mensagens
{
    [Serializable]
    [ProtoContract]
    public class RestrictionSymbolSyncMsg
    {
        [ProtoMember(1)]
        public SyncMsgAction SyncAction { get; set; }

        [ProtoMember(2)]
        public ConcurrentDictionary<string, RestrictionSymbolInfo> RestrictionSymbol { get; set; }

        public RestrictionSymbolSyncMsg()
        {
            RestrictionSymbol = new ConcurrentDictionary<string, RestrictionSymbolInfo>();
        }
    }
}
