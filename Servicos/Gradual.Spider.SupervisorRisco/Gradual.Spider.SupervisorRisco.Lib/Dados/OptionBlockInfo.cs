using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Gradual.Spider.SupervisorRisco.Lib.Dados
{
    [Serializable]
    [ProtoContract]
    public class OptionBlockInfo
    {
        [ProtoMember(1, IsRequired=true)]
        public int IdMovimento { get;set;}

        [ProtoMember(2)]
        public string Serie { get; set; }

        [ProtoMember(3)]
        public DateTime DtBloqueio { get; set; }
    }
}
