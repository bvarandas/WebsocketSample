using Cors;
using Gradual.OMS.Library.Servicos;
using Gradual.Spider.PositionClient.Monitor.Lib;
using Gradual.Spider.PositionClient.Monitor.Lib.Message;
using Gradual.Spider.PositionClient.Monitor.Monitores.RiscoResumido;
using log4net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;

namespace Gradual.Spider.PositionClient.Monitor.Monitores.RiscoResumidoIntranet
{
    /// <summary>
    /// Classe responsável pelo Broadcast, monitoração e consumo de socket de postion client com os resumos de 
    /// monitoramento de risco dos clientes
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class MonitorRiscoResumidoIntranet : IServicoControlavel, IServicoRiscoResumidoIntranet
    {
        #region Atributos
        /// <summary>
        /// Atributo que sinaliza se o serviço de monitaramento está ativo ou não.
        /// </summary>
        private bool _KeepRunning = false;

        /// <summary>
        /// Atributo responsável pela log da classe
        /// </summary>
        private static readonly log4net.ILog _Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Atributo da classe de socket responsável pela conexão e consumo do serviço de 
        /// socket do Postion Client
        /// </summary>
        private static PositionClientSocketRiscoResumido _Socket = null;

        /// <summary>
        /// Atributo que identifica se o serviço está para rodando ou não.
        /// </summary>
        private ServicoStatus _ServicoStatus;

        /// <summary>
        /// Thread de controle de enfileiramento de mensagem de posições de clientes.
        /// </summary>
        private Thread _ThreadQueueSendRiscoResumido = null;

        /// <summary>
        /// Atributo publico que armazena uma lista de  Session message position Client.
        /// </summary>
        public ConcurrentQueue<SessionMessagePostionClient> _QueueSessionMessagesRiscoResumido = new ConcurrentQueue<SessionMessagePostionClient>();

        /// <summary>
        /// Singleton  da classe para ser como instancia em outras classes externas
        /// </summary>
        private static PositionClientMonitorRiscoResumido _instance = null;

        /// <summary>
        /// WebService Sel HOsting para HOstiar as chamadas REST 
        /// </summary>
        private WebServiceHost _SelfHost = null;
        #endregion

        #region IServicoControlável
        public void IniciarServico()
        {
            try
            {
                _Logger.InfoFormat("Iniciando o Serviço de Risco Resumido para intranet");

                _Socket = PositionClientSocketRiscoResumido.Instance;// new PositionClientPackageSocket();

                _Socket.IpAddr  = ConfigurationManager.AppSettings["ASConnPositionClientIp"].ToString();
                _Socket.Port    = Convert.ToInt32(ConfigurationManager.AppSettings["ASConnPositionClientPort"].ToString());

                //_Socket.OpenConnection();

                var lSubscriptions = new List<int>();

                lSubscriptions.Add(14);

                _Socket.StartClientConnect(lSubscriptions);

                _ThreadQueueSendRiscoResumido = new Thread(new ThreadStart(QueueSendMessageRiscoResumido));

                _ThreadQueueSendRiscoResumido.Name = "ThreadQueueSendPositionClient";

                _ThreadQueueSendRiscoResumido.Start();

                _KeepRunning = true;

                _ServicoStatus = ServicoStatus.EmExecucao;

                //_Socket.SendMessageClientConnected += SendMessageRiscoResumidoToQueue;

                //_CommandAssemblySUBSCRIBE.SendMessageClientConnected += SendMessageMemoryPositionClientToQueue;

                string lSelfHost = ConfigurationManager.AppSettings["RestRiscoResumidoIntranet"].ToString();

                _SelfHost = new WebServiceHost(typeof(RestRiscoResumidoIntranet), new Uri(lSelfHost));

                var lEndPoint = _SelfHost.AddServiceEndpoint(typeof(IServicoRiscoResumidoIntranet), new WebHttpBinding(WebHttpSecurityMode.None), "");

                lEndPoint.Behaviors.Add(new CorsBehaviorAttribute());

                foreach (var operation in lEndPoint.Contract.Operations)
                {
                    //add support for cors (and for operation to be able to not  
                    //invoke the operation if we have a preflight cors request)  
                    operation.Behaviors.Add(new CorsBehaviorAttribute());
                }

                try
                {
                    _Logger.Info("Tetando iniciar Self Hosting  do WebServiceHost");

                    // Step 2 Start the service.
                    _SelfHost.Open();

                    _Logger.Info("Self Hosting iniciado com sucesso WebServiceHost");

                }
                catch (CommunicationException ex)
                {
                    Console.WriteLine("An exception occurred: {0}", ex.Message);

                    _SelfHost.Abort();
                }

                _Logger.Info("Servico de Risco Resumido para Intranet via WebSocket iniciado com sucesso");

            }
            catch (Exception ex)
            {
                _Logger.Error("Erro encontrado ao iniciar serviço para Intranet de Risco Resumido com WebServiceHost", ex);

                throw (ex);
            }
        }

        public void PararServico()
        {
            _Logger.Info("Parando o servico de Monitoramento e consumo do Risco Resumido Intranet");

            _ServicoStatus = ServicoStatus.Parado;

            _Logger.Info("Servico parado com sucesso.");

            //Parando serviço de Hosting do REST
            _SelfHost.Close();

            while (_ThreadQueueSendRiscoResumido.IsAlive)
            {
                _Logger.Info("Aguardando finalizar thThreadCarregarMonitorMemoria");
                Thread.Sleep(250);
                _ThreadQueueSendRiscoResumido.Abort();
            }
        }

        public ServicoStatus ReceberStatusServico()
        {
            return _ServicoStatus;
        }

        /// <summary>
        /// Método que desenfileira a mensagem de postion client da fila e 
        /// envia a mensagem para as sessões conectadas
        /// </summary>
        private void QueueSendMessageRiscoResumido()
        {
            _Logger.Info("Entrou na função de enviar mensagens de Risco Resumido para as sessões conectadas");

            while (_KeepRunning )
            {
                try
                {
                    SessionMessagePostionClient lSessionMessage;

                    if (_QueueSessionMessagesRiscoResumido.TryDequeue(out lSessionMessage))
                    {
                        lSessionMessage.Session.Send(lSessionMessage.MessageString);

                        _Logger.InfoFormat("Quantidade na fila: [{0}]", _QueueSessionMessagesRiscoResumido.Count);

                        continue;
                    }

                    Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    _Logger.Error("Erro encontrado no método QueueSendMessageRiscoResumido() -> ", ex);
                }
            }
        }

        public string BuscarRiscoResumidoIntranetJSON(string pRequestJson)
        {
            throw new NotImplementedException();
        }
        #endregion

        
    }
}
