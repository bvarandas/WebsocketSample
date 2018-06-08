using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using Gradual.Spider.Acompanhamento4Socket.Lib.Dados;

namespace Gradual.Spider.Acompanhamento4Socket.Lib.Mensagem
{
    [ProtoContract]
    [Serializable]
    public class FilterStreamerRequest
    {
        [ProtoMember(1)]
        public FilterIntVal Account { get; set; }
        [ProtoMember(2)]
        public FilterStringVal SessionID { get; set; }
        [ProtoMember(3)]
        public FilterStringVal Symbol { get; set; }
        [ProtoMember(4)]
        public string Id { get; set; }

        public FilterStreamerRequest()
        {
            this.Account = new FilterIntVal();
            this.SessionID = new FilterStringVal();
            this.Symbol = new FilterStringVal();
            this.Id = string.Empty;
        }
    }

    [ProtoContract]
    [Serializable]
    public class FilterStreamerResponse
    {
        [ProtoMember(1)]
        public int ErrCode { get; set; }
        [ProtoMember(2)]
        public string ErrMsg { get; set; }
        [ProtoMember(3)]
        public string Id { get; set; }

        public FilterStreamerResponse()
        {
            this.ErrCode = 0;
            this.ErrMsg = string.Empty;
            this.Id = string.Empty;
        }
    }

}
