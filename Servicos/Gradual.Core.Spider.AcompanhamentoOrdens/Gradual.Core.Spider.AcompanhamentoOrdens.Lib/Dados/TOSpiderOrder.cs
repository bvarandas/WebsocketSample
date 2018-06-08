using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Core.Spider.AcompanhamentoOrdens.Lib.Dados
{
    public class TOSpiderOrder
    {
        public SpiderOrderInfo Order {get; set;}
        public SpiderOrderDetailInfo OrderDetail { get; set; }

        public TOSpiderOrder()
        {
            this.Order = null;
            this.OrderDetail = null;
        }

    }
}
