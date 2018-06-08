using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Gradual.Spider.PositionClient.Monitor.Lib.Message;
using System.ServiceModel.Web;

namespace Gradual.Spider.PositionClient.Monitor.Lib
{
    /// <summary>
    /// Interface usado para chamar métodos de acesso ao serviço de Operacões Intraday
    /// </summary>
    [ServiceContract(Namespace = "http://gradual")]
    public interface IServicoRestRiscoResumido
    {
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json )]
        string BuscarRiscoResumidoJSON(string pRequestJson);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        string BuscarRiscoResumidoIntranetJSON(string pRequestJson);
    }
}