using Gradual.OMS.Library;
using Gradual.Spider.PositionClient.Monitor.Lib.Dados;
using Gradual.Spider.SupervisorRisco.Lib.Dados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Gradual.Spider.PositionClient.Monitor.Lib.Message
{
    /// <summary>
    /// Classe de gerenciamento do Resultado da busca da tela de risco resumido
    /// </summary>
    [DataContract]
    [Serializable]
    public class BuscarRiscoResumidoResponse : MensagemResponseBase
    {

        [DataMember]
        public List<ConsolidatedRiskInfo> ListRiscoResumido { get; set; }
    }
}
