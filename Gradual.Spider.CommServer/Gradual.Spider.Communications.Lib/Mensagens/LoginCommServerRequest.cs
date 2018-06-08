using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Gradual.Spider.Communications.Lib.Mensagens
{
    [ProtoContract]
    [Serializable]
    public class LoginCommServerRequest
    {
        [ProtoMember(1)]
        public string AppDescription { get; set; }
    }
}
