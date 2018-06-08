using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.OMS.Library;
using Gradual.Spider.Lib.Dados;

namespace Gradual.Spider.Lib.Mensagens
{
    public class GerenciadorPlataformaSalvarResponse : MensagemResponseBase
    {
        public GerenciadorPlataformaInfo GerenciadorPlataforma { get; set; }
    }
}
