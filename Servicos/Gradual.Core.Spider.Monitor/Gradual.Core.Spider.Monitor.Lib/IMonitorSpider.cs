using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Gradual.Core.Spider.Monitoring.Lib
{
    [ServiceContract(Namespace = "http://gradual")]
    public interface IMonitorSpider
    {
        [OperationContract]
        void DummyFunction();

        [OperationContract]
        string GetMonitorListJson();
    }
}
