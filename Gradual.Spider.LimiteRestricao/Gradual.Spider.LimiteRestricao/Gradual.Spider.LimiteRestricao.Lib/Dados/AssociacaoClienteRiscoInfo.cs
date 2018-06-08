using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Gradual.Spider.LimiteRestricao.Lib.Dados
{
    [DataContract]
    public class AssociacaoClienteRiscoInfo
    {
        [Serializable]
        public enum eTipoAssociacao
        {
            Parametro = 1,
            Permissao = 2
        }

        [DataMember]
        public eTipoAssociacao TipoAssociacao { get; set; }

        [DataMember]
        public string CodigoAssociacao
        {
            get
            {
                string lRetorno = "";
                switch (TipoAssociacao)
                {
                    case eTipoAssociacao.Parametro:
                        lRetorno = ((int)TipoAssociacao).ToString() + "." + CodigoClienteParametro.ToString();
                        break;
                    case eTipoAssociacao.Permissao:
                        lRetorno = ((int)TipoAssociacao).ToString() + "." + CodigoClientePermissao.ToString();
                        break;
                    default:
                        lRetorno = "0.-1";
                        break;
                }
                return lRetorno;
            }
        }
        
        [DataMember]
        public int CodigoCliente                { get; set; }
        
        [DataMember]
        public int CodigoGrupo                  { get; set; }
        
        //PARAMETRO
        [DataMember]
        public int CodigoClienteParametro       { get; set; }

        [DataMember]
        public int CodigoParametro              { get; set; }

        [DataMember]
        public decimal ValorParametro           { get; set; }

        [DataMember]
        public DateTime DataValidadeParametro   { get; set; }

        [DataMember]
        public string DescricaoParametro        { get; set; }
        
        //PERMISSAO
        [DataMember]
        public int CodigoClientePermissao       { get; set; }

        [DataMember]
        public int CodigoPermissao              { get; set; }

        [DataMember]
        public string DescricaoPermissao        { get; set; }


        public override string ToString()
        {
            return " ; {[TipoAssociacao] "      + this.TipoAssociacao.ToString() +
                " ; [CodigoAssociacao] "        + this.CodigoAssociacao.ToString() +
                " ; [CodigoCliente] "           + this.CodigoCliente.ToString() +
                " ; [CodigoGrupo] "             + this.CodigoGrupo.ToString() +
                " ; [CodigoClienteParametro] "  + this.CodigoClienteParametro.ToString() +
                " ; [CodigoParametro] "         + this.CodigoParametro.ToString() +
                " ; [ValorParametro] "          + this.ValorParametro.ToString() +
                " ; [DataValidadeParametro] "   + this.DataValidadeParametro.ToString() +
                " ; [DescricaoParametro] "      + this.DescricaoParametro.ToString() +
                " ; [CodigoClientePermissao] "  + this.CodigoClientePermissao.ToString() +
                " ; [CodigoPermissao] "         + this.CodigoPermissao.ToString() +
                " ; [DescricaoPermissao] "      + this.DescricaoPermissao.ToString() +
                "}";
        }
    }
}
