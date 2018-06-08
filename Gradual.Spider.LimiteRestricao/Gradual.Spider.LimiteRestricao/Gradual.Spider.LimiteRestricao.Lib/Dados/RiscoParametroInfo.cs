using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Gradual.Spider.LimiteRestricao.Lib.Dados
{
    [DataContract]
    public class RiscoParametroInfo
    {
        #region Propriedades
        [DataMember]
        public string NomeParametro
        {
            get;
            set;
        }

        [DataMember]
        public int CodigoParametro
        {
            get;
            set;
        }

        [DataMember]
        public BolsaInfo Bolsa
        {
            get;
            set;
        }
        #endregion


        #region Construtores
        public RiscoParametroInfo(){}

        ~RiscoParametroInfo(){}
        #endregion


    }
}
