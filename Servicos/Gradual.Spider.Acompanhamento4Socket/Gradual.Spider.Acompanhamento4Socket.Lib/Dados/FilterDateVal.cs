using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Gradual.Spider.Acompanhamento4Socket.Lib.Dados
{
    [ProtoContract]
    [Serializable]
    public class FilterDateVal
    {
        [ProtoMember(1)]
        public DateTime Value { get; set; }
        [ProtoMember(2, IsRequired = true)]
        public int Compare { get; set; }
        [ProtoMember(3)]
        public DateTime Value2 { get; set; }
        [ProtoMember(4)]
        public List<DateTime> ListValue { get; set; }
        public FilterDateVal()
        {
            this.Value = DateTime.MinValue;
            this.Compare = TypeCompare.UNDEFINED;
            this.ListValue = new List<DateTime>();
        }

        public FilterDateVal(DateTime dt, int typeCompare)
        {
            this.Value = dt;
            this.Compare = typeCompare;
            this.ListValue = new List<DateTime>();
        }
        public FilterDateVal(DateTime dt, DateTime dt2, int typeCompare)
        {
            this.Value = dt;
            this.Value2 = dt2;
            this.Compare = typeCompare;
            this.ListValue = new List<DateTime>();
        }
    }
}
