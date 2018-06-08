using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.SupervisorRisco.Lib.Dados
{
    public class LimitBaseResponse
    {
        public string StatusResponse { get; set; }
        public string DescricaoErro { get; set; }
        public string StackTrace { get; set; }

        public LimitBaseResponse()
        {
            this.StackTrace = string.Empty;
            this.DescricaoErro = string.Empty;
            this.StatusResponse = string.Empty;
        }
    }
}
