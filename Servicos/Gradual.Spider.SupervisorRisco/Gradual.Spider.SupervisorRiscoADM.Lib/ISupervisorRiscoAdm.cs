using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Gradual.Spider.SupervisorRiscoADM.Lib
{
    [ServiceContract(Namespace = "http://gradual")]
    public interface ISupervisorRiscoAdm
    {
        [OperationContract]
        bool ReloadResync();
    }
}
