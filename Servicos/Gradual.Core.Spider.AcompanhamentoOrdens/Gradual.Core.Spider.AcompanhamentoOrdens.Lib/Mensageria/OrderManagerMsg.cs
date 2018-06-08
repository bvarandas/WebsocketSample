using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.Core.Spider.AcompanhamentoOrdens.Lib.Dados;

namespace Gradual.Core.Spider.AcompanhamentoOrdens.Lib.Mensageria
{
    public class OrderManagerMsgRequest: BaseRequest
    {

        public OrderFilterInfo FilterOptions { get; set; }
        public OrderManagerMsgRequest()
            : base()
        {
            this.FilterOptions = new OrderFilterInfo();
        }

    }

    public class OrderManagerMsgResponse : BaseResponse
    {

        public List<SpiderOrderInfo> ListOrders;
        public OrderManagerMsgResponse()
            : base()
        {
            this.ListOrders = new List<SpiderOrderInfo>();
        }


    }

}
