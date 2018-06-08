using Gradual.OMS.Library;
using Gradual.Spider.PositionClient.Monitor.Lib.Dados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Gradual.Spider.PositionClient.Monitor.Lib.Message
{
    /// <summary>
    /// Classe de gerenciamento de Request da tela de Risco resumido
    /// </summary>
    [DataContract]
    [Serializable]
    public class BuscarRiscoResumidoRequest : MensagemRequestBase
    {
        /// <summary>
        /// Código do cliente que o usuario inserio no filtro
        /// </summary>
        [DataMember]
        public int CodigoCliente { get; set; }

        /// <summary>
        /// Código TIpo de Opção de filtro de PL 
        /// </summary>
        [DataMember]
        public OpcaoPL OpcaoPL { get; set; }

        /// <summary>
        /// Percentual de SFP atingido de prejuízo como filtro
        /// </summary>
        [DataMember]
        public OpcaoSFPAtingido OpcaoSFPAtingido { get; set; }

        /// <summary>
        /// Prejuízo atingido em moeda como filtro
        /// </summary>
        [DataMember]
        public OpcaoPrejuizoAtingido OpcaoPrejuizoAtingido { get; set; }



    }
}
