using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.OMS.Library;
using Gradual.Spider.Lib;

namespace Gradual.Spider.Lib.Mensagens
{
    public class RiscoListarPermissoesRequest : MensagemRequestBase
    {
        #region Propriedades
        public BolsaInfo Bolsa { get; set; }

        public string FiltroNomePermissao { get; set; }

        public override string ToString()
        {
            return " ; {Bolsa " + this.Bolsa.ToString() + " ; FiltroNomePermissao " + this.FiltroNomePermissao.ToString() + "}";
        }
        #endregion

        #region Construtores
        public RiscoListarPermissoesRequest() { }

        ~RiscoListarPermissoesRequest()
        {

		}
        #endregion
    }
}
