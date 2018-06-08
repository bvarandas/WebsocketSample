using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.Lib.Dados
{
    public class RiscoParametroInfo
    {
        #region Propriedades
        public string NomeParametro
        {
            get;
            set;
        }

        public int CodigoParametro
        {
            get;
            set;
        }

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
