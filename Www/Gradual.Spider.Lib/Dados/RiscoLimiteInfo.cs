using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.Spider.Lib.Dados;

namespace Gradual.IntranetCorp.Lib.Dados
{
    public class RiscoLimiteInfo :BaseInfo
    {
        public int ConsultaIdCliente            { get; set; }

        public int IdParametro                  { get; set; }

        public string DsParametro               { get; set; }

        public decimal VlParametro              { get; set; }

        public decimal VlAlocado                { get; set; }

        public decimal VlDisponivel             { get; set; }

        public bool NovoOMS                     { get; set; }

        public bool Spider                      { get; set; }

        public List<RiscoLimiteInfo> Resultado  { get; set; }
    }
}
