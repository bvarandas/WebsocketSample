using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.Lib.Dados
{
    public class RiscoPermissaoInfo : BaseInfo
    {
        public BolsaInfo Bolsa { get; set; }

        public int CodigoCliente { get; set; }

        public int CodigoPermissao { get; set; }

        public string NomePermissao { get; set; }

        public string NameSpace { get; set; }

        public string Metodo { get; set; }

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
