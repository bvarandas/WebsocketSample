using Gradual.Spider.DataSync.Lib;
using Gradual.Spider.SupervisorRisco.Lib.Dados;
using SuperWebSocket;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.PositionClient.Monitor.Lib.Message
{
    /// <summary>
    /// Classe de arguments de Risco Resumido
    /// Usada para passar a classe como parametro para a mensagem ser enviadas 
    /// aos aplcativos conectados no websocket
    /// </summary>
    public class MessageRiscoResumidoArgs : EventArgs
    {
        /// <summary>
        /// Codigo do Cliente para qual vai ser enviada a mensagem com os dados de position Client
        /// </summary>
        public int CodigoCliente { get; set; }

        /// <summary>
        /// Listagem de posições do cliente da sessão que irá ser 
        /// enviado para a aplicação conectada no websocket
        /// </summary>
        public ConcurrentDictionary<int, ConsolidatedRiskInfo> Message { get; set; }

        /// <summary>
        /// Tipo da ação da mensagem que está vindo do servidor
        /// Snapshot, Insert, Delete, Update, Delete_All
        /// </summary>
        public SyncMsgAction SyncAction { get; set; }

        /// <summary>
        /// Propriedade de controle de sessão para envio das mensagens de position client.
        /// Sessão do Aplicativo cliente.
        /// </summary>
        public WebSocketSession Session { get; set; }
    }
}
