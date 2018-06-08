using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using Gradual.Spider.Ordem.Lib.Dados;

namespace Gradual.Spider.RiskClient.Lib.Mensagens
{
    [Serializable]
    [ProtoContract]
    public class ValidarRiscoRequest
    {
        public string FixMsgType { get; set; }
        public SpiderOrderInfo Ordem { get; set; }
        public SpiderOrderInfo OrdemOriginal { get; set; }
    }
}
