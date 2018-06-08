using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Gradual.Spider.Communications.Lib.Mensagens
{
    [Serializable]
    [ProtoContract]
    public class LoginCommServerResponse
    {
        [ProtoMember(1)]
        public string SessionID { get; set; }

        [ProtoMember(2)]
        public string ClientInfo { get; set; }
    }
}
