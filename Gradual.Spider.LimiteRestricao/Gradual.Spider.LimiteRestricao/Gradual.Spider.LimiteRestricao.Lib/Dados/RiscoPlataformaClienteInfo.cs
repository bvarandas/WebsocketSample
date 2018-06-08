using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Gradual.Spider.LimiteRestricao.Lib.Dados
{
    [Serializable]
    [DataContract]
    public class RiscoPlataformaClienteInfo
    {
        [DataMember]
        public int CodigoCliente                        { get; set; }

        [DataMember]
        public List<RiscoPlataformaInfo> ListaPlataforma { get; set; }

        [DataMember]
        public eTipoParametroPlataforma TipoPlataforma  { get; set; }

        [DataMember]
        public bool EhContaMaster                       { get; set; }
    }
}
