using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.OMS.Library;
using Gradual.Spider.Lib.Dados;

namespace Gradual.Spider.Lib.Mensagens
{
    public class RiscoListarClienteParametroGrupoResponse : MensagemResponseBase
    {
        public List<RiscoClienteParametroGrupoInfo> ListaObjeto { get; set; }

        public RiscoListarClienteParametroGrupoResponse()
        {
            this.ListaObjeto = new List<RiscoClienteParametroGrupoInfo>();
        }
    }
}
