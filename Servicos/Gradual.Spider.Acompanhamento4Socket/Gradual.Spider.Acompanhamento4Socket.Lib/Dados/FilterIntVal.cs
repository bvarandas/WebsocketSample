using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Gradual.Spider.Acompanhamento4Socket.Lib.Dados
{
    [ProtoContract]
    [Serializable]
    public class FilterIntVal
    {
        [ProtoMember(1, IsRequired = true)]
        public int Value { get; set; }
        [ProtoMember(2, IsRequired = true)]
        public int Compare { get; set; }
        [ProtoMember(3, IsRequired = true)]
        public int Value2 { get; set; }
        [ProtoMember(4, IsRequired = true)]
        public List<int> ListValue { get; set; }

        public FilterIntVal()
        {
            this.Value = Int32.MinValue;
            this.Compare = TypeCompare.UNDEFINED;
            this.ListValue = new List<int>();
        }

        public FilterIntVal(int val, int typeCompare)
        {
            this.Value = val;
            this.Compare = typeCompare;
            this.ListValue = new List<int>();
        }

        public FilterIntVal(int val, int val2, int typeCompare)
        {
            this.Value = val;
            this.Value2 = val2;
            this.Compare = typeCompare;
            this.ListValue = new List<int>();
        }
    }
}
