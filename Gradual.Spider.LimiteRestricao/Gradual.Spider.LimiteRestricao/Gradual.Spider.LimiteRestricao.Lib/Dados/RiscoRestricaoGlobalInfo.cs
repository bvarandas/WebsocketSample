using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Gradual.Spider.LimiteRestricao.Lib.Dados
{
    [DataContract]
    public class RiscoRestricaoGlobalInfo
    {
        [DataMember]
        public int CodigoCliente            { get; set; }

        [DataMember]
        public decimal LimiteVolumeNet      { get; set; }

        [DataMember]
        public decimal QuantidadeNet        { get; set; }

        [DataMember]
        public decimal LimitePerdaMax       { get; set; }

        [DataMember]
        public decimal LimiteMaxOfertaVolume { get; set; }

        [DataMember]
        public decimal LimiteMaxOfertaQtde  { get; set; }

        [DataMember]
        public string Ativo                 { get; set; }

        [DataMember]
        public DateTime DataAtualizacao     { get; set; }

    }
}
