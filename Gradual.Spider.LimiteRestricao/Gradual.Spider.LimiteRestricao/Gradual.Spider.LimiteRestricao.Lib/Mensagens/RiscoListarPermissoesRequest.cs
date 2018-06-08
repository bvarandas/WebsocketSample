using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.OMS.Library;
using Gradual.Spider.LimiteRestricao.Lib;
using System.Runtime.Serialization;

namespace Gradual.Spider.LimiteRestricao.Lib.Mensagens
{
    [DataContract]
    [Serializable]
    public class RiscoListarPermissoesRequest : MensagemRequestBase
    {
        #region Propriedades
        [DataMember]
        public BolsaInfo Bolsa { get; set; }

        [DataMember]
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
