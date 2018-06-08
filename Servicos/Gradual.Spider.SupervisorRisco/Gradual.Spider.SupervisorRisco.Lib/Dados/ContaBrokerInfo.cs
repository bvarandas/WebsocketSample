using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Gradual.Spider.SupervisorRisco.Lib.Dados
{
    [Serializable]
    [ProtoContract]
    public class ContaBrokerInfo
    {
        [ProtoMember(1, IsRequired = true)]
        public int IdContaBroker { get; set; }
        [ProtoMember(2, IsRequired = true)]
        public int IdCliente { get; set; }
        [ProtoMember(3, IsRequired = true)]
        public string DescCliente { get; set; }
        [ProtoMember(4, IsRequired = true)]
        public bool Ativo { get; set; }

        public ContaBrokerInfo()
        {
            this.IdCliente = 0;
            this.IdContaBroker = 0;
            this.DescCliente = string.Empty;
            this.Ativo = false;
        }
    }
}
