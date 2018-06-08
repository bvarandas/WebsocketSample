using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using System.Collections.Concurrent;
using Gradual.Spider.SupervisorRisco.Lib.Dados;

namespace Gradual.Spider.DataSync.Lib.Mensagens
{
    [Serializable]
    [ProtoContract]
    public class MaxLossSyncMsg
    {

        [ProtoMember(1)]
        public SyncMsgAction SyncAction { get; set; }

        [ProtoMember(2)]
        public ConcurrentDictionary<int, List<OperatingLimitInfo>> MaxLoss { get; set; }

        public MaxLossSyncMsg()
        {
            MaxLoss = new ConcurrentDictionary<int, List<OperatingLimitInfo>>();
        }
    }
}
