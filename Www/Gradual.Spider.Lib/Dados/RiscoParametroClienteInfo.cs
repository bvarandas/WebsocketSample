using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.Lib.Dados
{
    public class RiscoParametroClienteInfo
    {
        #region Prorpiedades
        public List<RiscoParametroClienteValorInfo> ListaParametroClienteValores { get; set; }

        public int CodigoParametroCliente           { get; set; }

        public int CodigoCliente                    { get; set; }

        public RiscoParametroInfo Parametro         { get; set; }

        public decimal? Valor                       { get; set; }

        public DateTime? DataValidade               { get; set; }

        public RiscoGrupoInfo Grupo                 { get; set; }

        public int IdBolsa                          { get; set; }

        public char StAtivo                         { get; set; }
        #endregion

        #region Construtores
        public RiscoParametroClienteInfo()
        {
            ListaParametroClienteValores = new List<RiscoParametroClienteValorInfo>();
        }
        #endregion
    }
}
