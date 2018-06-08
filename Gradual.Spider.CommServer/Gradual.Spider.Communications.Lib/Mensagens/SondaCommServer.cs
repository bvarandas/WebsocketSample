using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using Gradual.Spider.Communications.Lib.Dados;
using System.ComponentModel;

namespace Gradual.Spider.Communications.Lib.Mensagens
{
    [ProtoContract]
    [Serializable]
    public class SondaCommServer
    {
        [ProtoMember(1)]
        public DateTime TimeStamp { get; set; }

        [ProtoMember(2), DefaultValue(SpiderServiceEnum.UnknownService)]
        public SpiderServiceEnum Service { get; set; }

        public SondaCommServer()
        {
            this.TimeStamp = DateTime.Now;
        }
    }
}
