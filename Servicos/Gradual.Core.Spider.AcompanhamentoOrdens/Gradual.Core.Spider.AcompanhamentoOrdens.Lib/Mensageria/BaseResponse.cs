using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Core.Spider.AcompanhamentoOrdens.Lib.Mensageria
{
    public class BaseResponse
    {
        public int StatusResponse { get; set; }
        public string DescricaoErro { get; set; }
        public string StackTrace { get; set; }

        public BaseResponse()
        {
            this.StackTrace = string.Empty;
            this.DescricaoErro = string.Empty;
            this.StatusResponse = -1;
        }
    }
}
