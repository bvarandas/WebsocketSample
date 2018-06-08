using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.OMS.Library;
using System.Runtime.Serialization;

namespace Gradual.Spider.LimiteRestricao.Lib.Dados
{
    [DataContract]
    public class RiscoTravaExposicaoInfo : ICodigoEntidade
    {
        [DataMember]
        public decimal PrejuizoMaximo { get; set; }

        [DataMember]
        public decimal PrecentualOscilacao { get; set; }

        #region ICodigoEntidade Members

        public string ReceberCodigo()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
