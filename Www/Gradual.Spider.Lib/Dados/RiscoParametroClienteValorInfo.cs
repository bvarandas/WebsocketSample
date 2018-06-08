using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.Lib.Dados
{
    public class RiscoParametroClienteValorInfo
    {
        #region Propriedades
        
        public int CodigoParametroClienteValor { get; set; }

        public RiscoParametroClienteInfo ParametroCliente { get; set; }

        public decimal ValorAlocado { get; set; }

        public decimal ValorDisponivel { get; set; }

        public string Descricao { get; set; }

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
