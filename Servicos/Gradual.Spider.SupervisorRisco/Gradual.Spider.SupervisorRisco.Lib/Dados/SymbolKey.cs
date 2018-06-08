using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.Spider.SupervisorRisco.Lib.Dados;
using ProtoBuf;

namespace Gradual.Spider.SupervisorRisco.Lib.Dados
{
    [Serializable]
    [ProtoContract]
    public class SymbolKey
    {
        [ProtoMember(1)]
        public SentidoBloqueioEnum Side { get; set; }

        [ProtoMember(2)]
        public string Instrument { get;set;}

        [ProtoMember(3,IsRequired=true)]
        public int Account { get; set; }

        public SymbolKey()
        {
            this.Instrument = string.Empty;
            this.Account = -1;
        }

        public override int GetHashCode()
        {
            string aux = this.Side.ToString() + this.Instrument + this.Account.ToString();
            return aux.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as SymbolKey);
        }

        public bool Equals(SymbolKey obj)
        {
            if (this.Account == -1)
                return this.Side == obj.Side && this.Instrument == obj.Instrument;
            else
                return this.Side == obj.Side && this.Instrument == obj.Instrument && this.Account == obj.Account;
        }
    }
}
