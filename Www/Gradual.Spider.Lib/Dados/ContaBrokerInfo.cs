using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.Lib.Dados
{
    public class ContaBrokerInfo : BaseInfo
    {
        public int IdContaBroker { get; set; }

        public int IdContaCliente { get; set; }

        public string NomeCliente { get; set; }

        public bool StAtivo { get; set; }

    }
}
