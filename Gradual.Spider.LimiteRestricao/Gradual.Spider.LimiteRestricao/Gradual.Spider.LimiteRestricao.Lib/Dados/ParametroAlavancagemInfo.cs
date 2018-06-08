using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Gradual.Spider.LimiteRestricao.Lib.Dados
{
    [DataContract]
    public class ParametroAlavancagemInfo
    {
        [DataMember]
        public int IdParametroGrupoAlavancagem            { get; set; }

        [DataMember]
        public int IdGrupo                                { get; set; }

        [DataMember]
        public decimal PercentualContaCorrente            { get; set; }

        [DataMember]
        public decimal PercentualCustodia                 { get; set; }

        [DataMember]
        public char StCarteiraOpcao                       { get; set; }

        [DataMember]
        public char StCarteiraGarantiaPrazo               { get; set; }

        [DataMember]
        public decimal PercentualAlavancagemCompraAVista  { get; set; }

        [DataMember]
        public decimal PercentualAlavancagemVendaAVista   { get; set; }

        [DataMember]
        public decimal PercentualAlavancagemCompraOpcao   { get; set; }

        [DataMember]
        public decimal PercentualAlavancagemVendaOpcao    { get; set; }

        public string ReceberCodigo()
        {
            throw new NotImplementedException();
        }
    }
}
