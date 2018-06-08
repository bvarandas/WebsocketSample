using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Gradual.Spider.DataSync.Lib.Mensagens
{
    [Serializable]
    [ProtoContract]
    public class AccountBvspBMFSyncMsg
    {
        [ProtoMember(1)]
        public SyncMsgAction SyncAction { get; set; }

        [ProtoMember(2)]
        public Dictionary<int, int> Accounts { get; set; }

        public AccountBvspBMFSyncMsg()
        {
            Accounts = new Dictionary<int, int>();
        }
    }
}
