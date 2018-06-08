using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Gradual.Spider.RiskClient.Lib.Mensagens
{
    [Serializable]
    [ProtoContract]
    public class ValidarContaRepasseRequest
    {
        public int Account { get; set; }
    }
}
