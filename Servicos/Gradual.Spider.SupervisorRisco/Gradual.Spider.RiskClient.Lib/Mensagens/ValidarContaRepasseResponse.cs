using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Gradual.Spider.RiskClient.Lib.Mensagens
{
    [Serializable]
    [ProtoContract]
    public class ValidarContaRepasseResponse
    {
        [ProtoMember(1)]
        public string RejectMessage { get; set; }

        [ProtoMember(2)]
        public bool ValidationResult { get; set; }

        public ValidarContaRepasseResponse()
        {
            
        }
    }
}
