using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.Servicos.Configuracoes.Lib.Mensageria
{
    public class BuscarParametrosResponse: Gradual.OMS.Library.MensagemResponseBase
    {
        public List<Gradual.Spider.Servicos.Configuracoes.Lib.Classes.Parametro> Parametros { get; set; }

        public BuscarParametrosResponse()
        {
            Parametros = new List<Gradual.Spider.Servicos.Configuracoes.Lib.Classes.Parametro>();
        }
    }
}
