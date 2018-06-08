using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.Core.Spider.OrderFixProcessing.Lib.Dados;
using Gradual.Spider.Acompanhamento4Socket.Lib.Dados;
using ProtoBuf;

namespace Gradual.Spider.Acompanhamento4Socket.Lib.Mensagem
{
    [Serializable]
    [ProtoContract]
    public class StreamerOrderInfo
    {
        [ProtoMember(1, IsRequired = true)]
        public int MsgType { get; set; }
        [ProtoMember(2, IsRequired = true)]
        public SpiderOrderInfo Order { get; set; }
        [ProtoMember(3, IsRequired = true)]
        public string Id {get;set;}

        public StreamerOrderInfo()
        {
            this.MsgType = MsgTypeConst.UNDEFINED;
            this.Order = null;
            this.Id = string.Empty;
        }
        ~StreamerOrderInfo()
        {
            if (this.Order != null)
            {
                this.Order.Details.Clear();
                this.Order = null;
            }
        }

    }
}
