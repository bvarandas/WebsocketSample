using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Runtime.Serialization;
using Gradual.OMS.Library;

namespace Gradual.Spider.PositionClient.Monitor.Lib.Message
{
    [DataContract]
    [Serializable]
    public class BuscarPositionClientResponse : MensagemResponseBase
    {
    }
}
