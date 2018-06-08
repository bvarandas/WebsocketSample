using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Gradual.Spider.LimiteRestricao.Lib.Dados
{
    [DataContract]
    public class RiscoParametroClienteValorInfo
    {
        #region Propriedades
        
        [DataMember]
        public int CodigoParametroClienteValor { get; set; }

        [DataMember]
        public RiscoParametroClienteInfo ParametroCliente { get; set; }

        [DataMember]
        public decimal ValorAlocado { get; set; }

        [DataMember]
        public decimal ValorDisponivel { get; set; }

        [DataMember]
        public string Descricao { get; set; }

        [DataMember]
        public DateTime DataMovimento { get; set; }

        public override string ToString()
        {
            string lRetorno = " ; [CodigoParametroClienteValor] " + this.CodigoParametroClienteValor.ToString();
            if (null != ParametroCliente)
            {

                lRetorno += ParametroCliente.ToString();
            }

            lRetorno += " ; {[ValorAlocado] " + this.ValorAlocado.ToString();

            lRetorno += " ; [ValorDisponivel] " + this.ValorDisponivel.ToString();
            lRetorno += " ; [Descricao] " + this.Descricao.ToString();
            lRetorno += " ; [DataMovimento] " + this.DataMovimento.ToString() +
                "}";

            return lRetorno;
        }
        #endregion
    }
}
