using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Gradual.Spider.SupervisorRisco.Lib.Dados
{
    [Serializable]
    [ProtoContract]
    public class FatFingerInfo
    {

        [ProtoMember(1,IsRequired=true)]
        public int Account { set; get; }

        [ProtoMember(2, IsRequired = true)]
        public int IdRegra { set; get; }

        [ProtoMember(3)]
        public string DsRegra { set; get; }

        [ProtoMember(4)]
        public string Mercado { set; get; }

        [ProtoMember(5)]
        public decimal ValorRegra { set; get; }

        [ProtoMember(6)]
        public Nullable<DateTime> DtValidadeRegra { set; get; }
    }
}
