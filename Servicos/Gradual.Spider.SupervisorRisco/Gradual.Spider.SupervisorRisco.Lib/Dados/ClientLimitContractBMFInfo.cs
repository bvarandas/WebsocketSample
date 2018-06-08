using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Gradual.Spider.SupervisorRisco.Lib.Dados
{
    [Serializable]
    [ProtoContract]
    public class ClientLimitContractBMFInfo
    {
        [ProtoMember(1, IsRequired=true)]
        public int IdClienteParametroBMF { set; get; }
        [ProtoMember(2, IsRequired = true)]
        public int IdClientePermissao { set; get; }
        [ProtoMember(3, IsRequired = true)]
        public int QuantidadeMaximaOferta { set; get; }
        [ProtoMember(4, IsRequired = true)]
        public int Account { set; get; }
        [ProtoMember(5, IsRequired = true)]
        public string Contrato { set; get; }
        [ProtoMember(6, IsRequired = true)]
        public string Sentido { set; get; }
        [ProtoMember(7, IsRequired = true)]
        public int QuantidadeTotal { set; get; }
        [ProtoMember(8, IsRequired = true)]
        public int QuantidadeDisponivel { set; get; }
        [ProtoMember(9, IsRequired = true)]
        public char RenovacaoAutomatica { set; get; }
        [ProtoMember(10)]
        public DateTime DataMovimento { set; get; }
        [ProtoMember(11)]
        public DateTime DataValidade { set; get; }

        public ClientLimitContractBMFInfo()
        {
            
        
        }
    }
}
