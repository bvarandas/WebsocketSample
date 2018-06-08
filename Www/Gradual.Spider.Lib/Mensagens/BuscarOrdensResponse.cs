using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.OMS.Library;
using Gradual.Core.OMS.DropCopy.Lib.Dados;
using Gradual.OMS.RoteadorOrdens.Lib.Dados;

namespace Gradual.Spider.Lib
{
    public class BuscarOrdensResponse : MensagemResponseBase
    {
        public BuscarOrdensResponse(){}

        public List<OrdemInfo> Ordens { get; set; }
        public int? PaginaAtiva { get; set; }
        public int? TotalItens { get; set; }
    }
}
