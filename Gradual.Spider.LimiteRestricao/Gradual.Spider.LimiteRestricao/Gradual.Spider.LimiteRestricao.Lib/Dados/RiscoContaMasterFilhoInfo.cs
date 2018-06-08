using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Gradual.Spider.LimiteRestricao.Lib.Dados
{
    [Serializable]
    [DataContract]
    public class RiscoContaMasterFilhoInfo
    {
        [DataMember]
        public int CodigoCliente { get; set; }

        [DataMember]
        public string NomeCliente { get; set; }

        [DataMember]
        public string Ativo { get; set; }
    }
}
