using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Gradual.Spider.SupervisorRisco.Lib.Dados
{
    [Serializable]
    [ProtoContract]
    public class ClientLimitBMFInfo
    {
        [ProtoMember(1)]
        public List<ClientLimitContractBMFInfo> ContractLimit {get;set;}
        [ProtoMember(2)]
        public List<ClientLimitInstrumentBMFInfo> InstrumentLimit{get;set;}
        [ProtoMember(3, IsRequired= true)]
        public int Account { get; set;}
        public ClientLimitBMFInfo()
        {
            this.ContractLimit = new List<ClientLimitContractBMFInfo>();
            this.InstrumentLimit = new List<ClientLimitInstrumentBMFInfo>();
            this.Account = -1;
        }
    }
}
