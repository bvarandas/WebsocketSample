using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.Acompanhamento4Socket.Lib.Dados
{
    public class ExecResp
    {
        public int Code { get; set; }
        public string Msg { get; set; }


        public ExecResp()
        {
            this.Code = MsgErrors.ERROR;
            this.Msg = MsgErrors.MSG_ERR;
        }

        public ExecResp(int i, string m)
        {
            this.Code = i;
            this.Msg = m;
        }
    }
}
