using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Gradual.Spider.LimiteRestricao.Lib.Dados
{
    [Serializable]
    [DataContract]
    public class RiscoPlataformaContaMasterInfo
    {
        [DataMember]
        public int CodigoContaMaster { get; set; }

        [DataMember]
        public List<RiscoContaMasterFilhoInfo> ListaCliente { get; set; }

        [DataMember]
        public List<RiscoPlataformaInfo> ListaPlataforma { get; set; }

        [DataMember]
        public eTipoParametroPlataforma TipoPlataforma { get; set; }
    }
}
