using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.Core.Spider.OrderFixProcessing.Lib.Dados;

namespace Gradual.Spider.SupervisorRisco.Lib.Dados
{
    public class TOSpOrder
    {
        public int MsgType { get; set; }
        public SpiderOrderInfo Order { get; set; } 

        public TOSpOrder()
        {
            this.MsgType = 0;
            this.Order = null;
        }
    }
}
