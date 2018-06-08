using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.OMS.Library;
using System.Runtime.Serialization;
namespace Gradual.Spider.PositionClient.Monitor.Lib.Message
{
    [DataContract]
    [Serializable]
    public class BuscarPositionClientRequest : MensagemRequestBase
    {
        [DataMember]
        public int CodigoCliente { get; set; }
    }
}
