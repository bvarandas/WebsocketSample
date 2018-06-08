using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Gradual.Spider.LimiteRestricao.Lib.Dados
{
    [DataContract]
    public class RiscoPermissaoInfo : BaseInfo
    {
        [DataMember]
        public BolsaInfo Bolsa { get; set; }

        [DataMember]
        public int CodigoCliente { get; set; }

        [DataMember]
        public int CodigoPermissao { get; set; }

        [DataMember]
        public string NomePermissao { get; set; }

        [DataMember]
        public string NameSpace { get; set; }

        [DataMember]
        public string Metodo { get; set; }

        [DataMember]
        public string DescricaoGrupo { get; set; }

        public override string ToString()
        {
            return " ; [Bolsa] "        + this.Bolsa.ToString()             +
                " ; [CodigoCliente] "   + this.CodigoCliente.ToString()     +
                " ; [CodigoPermissao] " + this.CodigoPermissao.ToString()   +
                " ; [NomePermissao] "   + this.NomePermissao.ToString()     +
                " ; [NameSpace] "       + this.NameSpace.ToString()         +
                " ; [Metodo] "          + this.Metodo.ToString()            +
                " ; [DescricaoGrupo] "  + this.DescricaoGrupo.ToString()    +
                "}";
        }
    }
}
