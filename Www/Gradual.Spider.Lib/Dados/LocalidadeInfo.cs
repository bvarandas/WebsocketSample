using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.Lib.Dados
{
    public class LocalidadeInfo : BaseInfo
    {
        public int CodigoLocalidade { get; set; }

        public string DsLocalidade  { get; set; }

        public bool StAtivo         { get; set; }
    }
}
