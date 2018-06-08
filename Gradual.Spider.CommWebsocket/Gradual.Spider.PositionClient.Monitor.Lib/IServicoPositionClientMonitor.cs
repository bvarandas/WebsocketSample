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
    /// Interface usado para chamar métodos de acesso ao serviço de position client
    /// </summary>
    [ServiceContract(Namespace = "http://gradual")]
    public interface IServicoPositionClientMonitor
    {
        /// <summary>
        /// Método que realiza a busca do objeto position client armazenado na memória para 
        /// ser retornado a aplicação cliente conectada
        /// </summary>
        /// <param name="lRequest">Dados de request com o código do cliente</param>
        /// <returns>Retorna o objeto de Position Client filtrado pelo código do cliente</returns>
        [OperationContract]
        BuscarPositionClientResponse BuscarPositionClient(BuscarPositionClientRequest lRequest);

        /// <summary>
        /// Método que realiza a busca do objeto Operacoes Intraday armazenado na memória 
        /// para ser retornado na aplicação conectada
        /// </summary>
        /// <param name="lReqeust">Dados de request com o filtro efetuado pelo usuário</param>
        /// <returns></returns>
        [OperationContract]
        BuscarOperacoesIntradayResponse BuscarOperacoesIntraday(BuscarOperacoesIntradayRequest lRequest);

        [OperationContract]
        [WebInvoke(Method = "GET" , ResponseFormat = WebMessageFormat.Json)]
        string BuscarOperacoesIntradayJSON();
        
    }
}
