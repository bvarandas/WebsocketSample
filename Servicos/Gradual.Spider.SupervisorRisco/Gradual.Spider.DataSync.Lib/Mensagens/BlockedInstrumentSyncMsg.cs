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
    public enum BlockedInstrumentMsgType
    {
        BlockedSymbolClient = 0,
        BlockedSymbolGroupClient = 1,
        BlockedSymbolGroupGlobal = 2,
    }

    [Serializable]
    [ProtoContract]
    public class BlockedInstrumentSyncMsg
    {
        [ProtoMember(1)]
        public SyncMsgAction SyncAction { get; set; }

        [ProtoMember(2)]
        public BlockedInstrumentMsgType BlockedInstrumentType { get; set; }

        [ProtoMember(3)]
        public ConcurrentDictionary<SymbolKey, BlockedInstrumentInfo> BlockedInstruments { get; set; }

        public BlockedInstrumentSyncMsg()
        {
            BlockedInstruments = new ConcurrentDictionary<SymbolKey, BlockedInstrumentInfo>();
        }

    }
}
