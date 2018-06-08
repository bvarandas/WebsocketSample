using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Gradual.Spider.LimiteRestricao.Lib.Dados
{
    [Serializable]
    [DataContract]
    public class RiscoPlataformaInfo
    {
        [DataMember]
        public int IdPlataforma { get; set; }

        [DataMember]
        public string DsPlataforma { get; set; }
    }
}
