using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.OMS.Library;
using Gradual.Spider.Lib.Dados;

namespace Gradual.IntranetCorp.Lib.Mensagens
{
    public class PlataformaSalvarResponse : MensagemResponseBase
    {
        public PlataformaSessaoInfo PlataformaSessao { get; set; }

    }
}
