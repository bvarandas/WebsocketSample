using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Gradual.Spider.LimiteRestricao.Lib.Dados
{
    [Serializable]
    [DataContract]
    public class ClienteParametroBMFInstrumentoInfo
    {
        [DataMember]
        public int IdClienteParametroInstrumento { set; get; }
        [DataMember]
        public int IdClienteParametroBMF { set; get; }
        [DataMember]
        public string ContratoBase { set; get; }
        [DataMember]
        public string Instrumento { set; get; }
        [DataMember]
        public int QtTotalContratoPai { set; get; }
        [DataMember]
        public int QtTotalInstrumento { set; get; }
        [DataMember]
        public int QtDisponivel { set; get; }
        [DataMember]
        public int QuantidadeMaximaOferta { set; get; }
        [DataMember]
        public char Sentido { set; get; }
        [DataMember]
        public DateTime dtMovimento { set; get; }
    }
}
