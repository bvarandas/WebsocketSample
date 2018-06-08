using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.OMS.Library;
using System.Runtime.Serialization;

namespace Gradual.Spider.LimiteRestricao.Lib.Dados
{
    [DataContract]
    public class RiscoClienteBloqueioRegraInfo : ICodigoEntidade
    {
        #region ICodigoEntidade Members

        [DataMember]
        public int CodigoCliente { get; set; }

        [DataMember]
        public string Ativo { get; set; }

        [DataMember]
        public string Direcao { get; set; }

        public string ReceberCodigo()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
