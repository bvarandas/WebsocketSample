using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.SupervisorRisco.Lib.Dados
{
    public class RiskExposureClientInfo
    {
        public int IdCliente { set; get; }
        public decimal LucroPrejuizo { set; get; }
        public decimal PatrimonioLiquido { set; get; }
        public DateTime DataAtualizacao { set; get; }
        public bool LimiteMaximoAtingido { set; get; }
    }
}
