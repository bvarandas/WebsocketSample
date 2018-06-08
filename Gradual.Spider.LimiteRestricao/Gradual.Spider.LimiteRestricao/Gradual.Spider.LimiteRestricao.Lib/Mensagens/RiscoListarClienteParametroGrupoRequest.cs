using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.OMS.Library;
using Gradual.Spider.LimiteRestricao.Lib.Dados;
using System.Runtime.Serialization;

namespace Gradual.Spider.LimiteRestricao.Lib.Mensagens
{
    [DataContract]
    [Serializable]
    public class RiscoListarClienteParametroGrupoRequest : MensagemRequestBase
    {
        [DataMember]
        public RiscoClienteParametroGrupoInfo Objeto { get; set; }

        public RiscoListarClienteParametroGrupoRequest()
        {
            this.Objeto = new RiscoClienteParametroGrupoInfo();
        }
    }
}
