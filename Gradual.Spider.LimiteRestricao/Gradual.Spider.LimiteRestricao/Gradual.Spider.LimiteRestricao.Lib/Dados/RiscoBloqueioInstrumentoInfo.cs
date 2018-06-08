using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Gradual.Spider.LimiteRestricao.Lib.Dados
{
    [DataContract]
    public class RiscoBloqueioInstrumentoInfo
    {
        [DataMember]
        public int IdCliente { get; set; }

        [DataMember]
        public string CdAtivo { get; set; }

        [DataMember]
        public string Direcao { get; set; }

        public string ReceberCodigo()
        {
            throw new NotImplementedException();
        }
    }
}
