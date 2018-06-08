using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Gradual.Core.Spider.AcompanhamentoOrdens.Lib.Mensageria;

namespace Gradual.Core.Spider.AcompanhamentoOrdens.Lib
{
    [ServiceContract(Namespace = "http://gradual")]
    public interface IAcompanhamentoOrdensAdm
    {
        [OperationContract]
        void DummyFunction();

        [OperationContract]
        OrderManagerMsgResponse GetOrders(OrderManagerMsgRequest req);
    }
}
