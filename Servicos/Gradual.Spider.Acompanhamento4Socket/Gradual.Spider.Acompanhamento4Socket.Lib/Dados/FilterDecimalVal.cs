using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Gradual.Spider.Acompanhamento4Socket.Lib.Dados
{
    [ProtoContract]
    [Serializable]
    public class FilterDecimalVal
    {
        [ProtoMember(1)]
        public Decimal Value { get; set; }
        [ProtoMember(2, IsRequired = true)]
        public int Compare { get; set; }
        [ProtoMember(3)]
        public Decimal Value2 { get; set; }
        [ProtoMember(4)]
        public List<Decimal> ListValue { get; set; }

        public FilterDecimalVal()
        {
            this.Value = Decimal.MinValue;
            this.Compare = TypeCompare.UNDEFINED;
            this.ListValue = new List<Decimal>();
        }

        public FilterDecimalVal(Decimal val, int typeCompare)
        {
            this.Value = val;
            this.Compare = typeCompare;
            this.ListValue = new List<Decimal>();
        }

        public FilterDecimalVal(Decimal val, Decimal val2, int typeCompare)
        {
            this.Value = val;
            this.Value2 = val2;
            this.Compare = typeCompare;
            this.ListValue = new List<Decimal>();
        }

    }
}
