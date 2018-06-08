using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using Gradual.Spider.SupervisorRisco.Lib.Dados;

namespace Gradual.Spider.DataSync.Lib.Mensagens
{
    [Serializable]
    [ProtoContract]
    public class FixLimitSyncMsg
    {
        [ProtoMember(1)]
        public SyncMsgAction SyncAction { get; set; }

        [ProtoMember(2)]
        public Dictionary<int,FixLimitInfo> FixLimits { get; set; }

        public FixLimitSyncMsg()
        {
            FixLimits = new Dictionary<int, FixLimitInfo>();
        }

    }
}
