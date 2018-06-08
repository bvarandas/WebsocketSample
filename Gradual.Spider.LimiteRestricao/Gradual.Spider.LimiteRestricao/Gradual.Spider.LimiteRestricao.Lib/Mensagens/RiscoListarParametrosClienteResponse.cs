using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.Spider.LimiteRestricao.Lib.Dados;
using Gradual.OMS.Library;
using System.Runtime.Serialization;


namespace Gradual.Spider.LimiteRestricao.Lib.Mensagens
{
    [DataContract]
    [Serializable]
    public class RiscoListarParametrosClienteResponse : MensagemResponseBase
    {
        #region Propriedades
        [DataMember]
        public List<RiscoParametroClienteInfo> ParametrosRiscoCliente
        {
            get;
            set;
        }

        public override string ToString()
        {
            string lRetorno = "{";
            if (null != this.ParametrosRiscoCliente)
            {
                foreach (RiscoParametroClienteInfo item in ParametrosRiscoCliente)
                {
                    lRetorno += item.ToString();
                }
            }
            lRetorno += "}";
            return lRetorno;
        }

        #endregion

        #region Construtores
        public RiscoListarParametrosClienteResponse()
        {
            ParametrosRiscoCliente = new List<RiscoParametroClienteInfo>();
        }

        ~RiscoListarParametrosClienteResponse()
        {
            ParametrosRiscoCliente = new List<RiscoParametroClienteInfo>();
        }
        #endregion

    }
}
