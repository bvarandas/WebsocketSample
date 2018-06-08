using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.Core.OMS.DropCopy.Lib;

namespace Gradual.Core.Spider.AcompanhamentoOrdens.Lib.Dados
{
    public class InstanciaInfo
    {
        public ISpiderSignDropCopyCallback InterfaceDC;
        public string Addr { get; set; }


        public InstanciaInfo()
        {
            this.InterfaceDC = null;
            this.Addr = string.Empty;
        }
    }
}
