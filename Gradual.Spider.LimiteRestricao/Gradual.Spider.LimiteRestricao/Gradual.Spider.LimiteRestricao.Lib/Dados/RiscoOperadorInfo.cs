using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Gradual.Spider.LimiteRestricao.Lib.Dados
{
    [Serializable]
    [DataContract]
    public class RiscoOperadorInfo
    {
        [DataMember]
        public int CodigoOperador { get; set; }

        [DataMember]
        public string Sigla { get; set; }

        [DataMember]
        public string Nome { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public eTipoParametroPlataforma TipoPlataforma { get; set; }
    }
}
