using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Gradual.Spider.PositionClient.Monitor.Lib
{
    [ServiceContract(Namespace = "http://gradual")]
    public interface IServicoRiscoResumidoIntranet
    {
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        string BuscarRiscoResumidoIntranetJSON(string pRequestJson);
    }
}
