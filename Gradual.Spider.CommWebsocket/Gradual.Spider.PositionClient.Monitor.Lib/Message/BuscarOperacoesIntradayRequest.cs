using Gradual.OMS.Library;
using Gradual.Spider.PositionClient.Monitor.Lib.Dados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Gradual.Spider.PositionClient.Monitor.Lib.Message
{
    [DataContract]
    [Serializable]
    public class BuscarOperacoesIntradayRequest : MensagemRequestBase
    {
        [DataMember]
        public int CodigoCliente { get; set; }

        [DataMember]
        public string Ativo { get; set; }

        [DataMember]
        public OpcaoMarket OpcaoMarket { get; set; }

        [DataMember]
        public OpcaoParametrosIntraday OpcaoParametrosIntraday { get; set; }
    }
}
