using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Gradual.Spider.LimiteRestricao.Lib.Dados
{
    [DataContract]
    public class RiscoParametroClienteInfo
    {
        #region Prorpiedades
        [DataMember]
        public List<RiscoParametroClienteValorInfo> ListaParametroClienteValores { get; set; }

        [DataMember]
        public int CodigoParametroCliente           { get; set; }

        [DataMember]
        public int CodigoCliente                    { get; set; }

        [DataMember]
        public RiscoParametroInfo Parametro         { get; set; }

        [DataMember]
        public decimal? Valor                       { get; set; }

        [DataMember]
        public DateTime? DataValidade               { get; set; }

        [DataMember]
        public RiscoGrupoInfo Grupo                 { get; set; }

        [DataMember]
        public int IdBolsa                          { get; set; }

        [DataMember]
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
