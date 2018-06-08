using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Gradual.Spider.Acompanhamento4Socket.Lib.Dados
{
    [ProtoContract]
    [Serializable]
    public class FilterStringVal
    {
        [ProtoMember(1)]
        public string Value { get; set; }
        [ProtoMember(2, IsRequired = true)]
        public int Compare { get; set; }
        [ProtoMember(3, IsRequired = true)]
        public List<string> ListValue { get; set; }

        public FilterStringVal()
        {
            this.Value = string.Empty;
            this.Compare = TypeCompare.UNDEFINED;
            this.ListValue = new List<string>();
        }

        public FilterStringVal(string val, int typeCompare)
        {
            this.Value = val;
            this.Compare = typeCompare;
            this.ListValue = new List<string>();
        }
    }
}
