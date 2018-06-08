using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Gradual.Spider.SupervisorRisco.Lib.Dados
{
    [Serializable]
    [ProtoContract]
    public class ClientLimitInstrumentBMFInfo
    {
        [ProtoMember(1, IsRequired=true)]
        public int Account;
        [ProtoMember(2, IsRequired = true)]
        public int IdClienteParametroInstrumento { set; get; }
        [ProtoMember(3, IsRequired = true)]
        public int IdClienteParametroBMF { set; get; }
        [ProtoMember(4, IsRequired = true)]
        public string ContratoBase { set; get; }
        [ProtoMember(5, IsRequired = true)]
        public string Instrumento { set; get; }
        [ProtoMember(6, IsRequired = true)]
        public int QtTotalContratoPai { set; get; }
        [ProtoMember(7, IsRequired = true)]
        public int QtTotalInstrumento { set; get; }
        [ProtoMember(8, IsRequired = true)]
        public int QtDisponivel { set; get; }
        [ProtoMember(9, IsRequired = true)]
        public int QuantidadeMaximaOferta { set; get; }
        [ProtoMember(10, IsRequired = true)]
        public string Sentido { set; get; }
        [ProtoMember(11, IsRequired = true)]
        public DateTime dtMovimento { set; get; }
    }
}
