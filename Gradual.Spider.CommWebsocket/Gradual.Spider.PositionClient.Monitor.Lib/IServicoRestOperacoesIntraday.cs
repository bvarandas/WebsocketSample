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
    public interface IServicoRestOperacoesIntraday
    {
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        string BuscarOperacoesIntradayJSON(
            int CodigoCliente, 
            string CodigoInstrumento, 
            bool OpcaoMarketTodosMercados,
            bool OpcaoMarketAVista,
            bool OpcaoMarketFuturos,
            bool OpcaoMarketOpcao,
            bool OpcaoParametroIntradayOfertasPedra,
            bool OpcaoParametroIntradayNetNegativo,
            bool OpcaoParametroIntradayNetPositivo,
            bool OpcaoParametroIntradayPLNegativo);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json)]
        string BuscarOperacoesIntradayIntranetJSON(int CodigoCliente);
    }
}
