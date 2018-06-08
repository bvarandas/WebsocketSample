using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.Spider.SupervisorRisco.Lib.Dados;
using System.Collections.Concurrent;
using Gradual.Spider.DataSync.Lib;
using SuperWebSocket;
using Gradual.Spider.PositionClient.Monitor.Dados;

namespace Gradual.Spider.PositionClient.Monitor.Lib.Message
{
    /// <summary>
    /// Classe de arguments de position client
    /// Usada para passar a classe como parametro para a mensagem ser enviadas 
    /// aos aplcativos conectados no websocket
    /// </summary>
    public class MessagePositionClientArgs : EventArgs
    {
        /// <summary>
        /// Codigo do Cliente para qual vai ser enviada a mensagem com os dados de position Client
        /// </summary>
        public int CodigoCliente                                             { get; set; }

        /// <summary>
        /// Request do socket de Execution from dos filtros das telas de Daily Activity
        /// </summary>
        public DateTime ExecutionFrom { get; set; }

        /// <summary>
        /// Request do socket de Executioin To dos filtros das telas de Daily Activity
        /// </summary>
        public DateTime ExecutionTo { get; set; }

        /// <summary>
        /// Listagem de posições do cliente da sessão que irá ser 
        /// enviado para a aplicação conectada no websocket
        /// </summary>
        public ConcurrentDictionary<int,  List<PosClientSymbolInfo>> Message { get; set; }

        /// <summary>
        /// Tipo da ação da mensagem que está vindo do servidor
        /// Snapshot, Insert, Delete, Update, Delete_All
        /// </summary>
        public SyncMsgAction SyncAction                                      { get; set; }

        /// <summary>
        /// Propriedade de controle de sessão para envio das mensagens de position client.
        /// Sessão do Aplicativo cliente.
        /// </summary>
        public WebSocketSession Session { get; set; }
    }
}
