using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.SupervisorRisco.Lib.Dados
{
    public class LimitResponse
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorStack {get;set;}
        public object InfoObject { get; set; }
        public LimitResponse()
        {
            this.ErrorCode = -1;
            this.ErrorMessage = string.Empty;
            this.ErrorStack = string.Empty;
            this.InfoObject = null;
        }
    }
}
