using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Gradual.Spider.LimiteRestricao.Lib.Dados
{
    public class ClienteParametroLimiteBMFInfo
    {
        [DataMember]
        public int idClienteParametroBMF { set; get; }

        [DataMember]
        public int idClientePermissao { set; get; }

        [DataMember]
        public int QuantidadeMaximaOferta { set; get; }

        [DataMember]
        public int Account { set; get; }

        [DataMember]
        public string Contrato { set; get; }

        [DataMember]
        public string Sentido { set; get; }

        [DataMember]
        public int QuantidadeTotal { set; get; }

        [DataMember]
        public int QuantidadeDisponivel { set; get; }

        [DataMember]
        public char RenovacaoAutomatica { set; get; }

        [DataMember]
        public DateTime DataMovimento { set; get; }

        [DataMember]
        public DateTime DataValidade { set; get; }
    }
}
