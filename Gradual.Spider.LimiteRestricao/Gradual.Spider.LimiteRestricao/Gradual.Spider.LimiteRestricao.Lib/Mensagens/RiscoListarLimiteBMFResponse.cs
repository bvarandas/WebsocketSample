using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Gradual.OMS.Library;
using Gradual.Spider.LimiteRestricao.Lib.Dados;

namespace Gradual.Spider.LimiteRestricao.Lib.Mensagens
{
    [Serializable]
    [DataContract]
    public class RiscoListarLimiteBMFResponse : MensagemResponseBase
    {
        [DataMember]
        public List<ClienteParametroLimiteBMFInfo> ListaLimites                  = new List<ClienteParametroLimiteBMFInfo>();
        
        [DataMember]
        public List<ClienteParametroBMFInstrumentoInfo> ListaLimitesInstrumentos = new List<ClienteParametroBMFInstrumentoInfo>();
    }
}
