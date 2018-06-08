using Gradual.Spider.PositionClient.Monitor.Lib.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Gradual.Spider.PositionClient.Monitor.Lib
{
    /// <summary>
    /// Interface usado para chamar métodos de acesso ao serviço de position client de Risco Resumido
    /// </summary>
    [ServiceContract(Namespace = "http://gradual")]
    public interface IPositionClientRiscoResumido
    {
        /// <summary>
        /// Método que realiza a busca do objeto de Risco Resumido armazenado na memória
        /// para ser retornado na aplicação conectada
        /// </summary>
        /// <param name="lRequest">Dados de request com o filtro efetuado pelo usuário na aplicação</param>
        /// <returns></returns>
        [OperationContract]
        BuscarRiscoResumidoResponse BuscarRiscoResumido(BuscarRiscoResumidoRequest lRequest);
    }
}
