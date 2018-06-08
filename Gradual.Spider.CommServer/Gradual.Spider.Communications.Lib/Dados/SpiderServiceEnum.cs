using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Gradual.Spider.Communications.Lib.Dados
{
    [Serializable]
    [ProtoContract]
    public enum SpiderServiceEnum
    {
        UnknownService = 0,
        CommunicationServer = 1,
        Acompanhamento4Spider = 2,
        SpiderRisco = 3
    }
}
