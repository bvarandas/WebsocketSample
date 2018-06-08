using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.Spider.Acompanhamento4Socket.Lib.Dados;
using ProtoBuf;
using Gradual.Core.Spider.OrderFixProcessing.Lib.Dados;

namespace Gradual.Spider.Acompanhamento4Socket.Lib.Mensagem
{
    [ProtoContract]
    [Serializable]
    public class FilterInfoRequest
    {
        [ProtoMember(1)]
        public FilterSpiderOrder Filter { get; set; }
        [ProtoMember(2)]
        public int RecordLimit { get; set; }
        [ProtoMember(3)]
        public string Id { get; set; }
        [ProtoMember(4)]
        public bool ReturnDetails { get; set; }

        public FilterInfoRequest()
        {
            this.Filter = null;
            this.RecordLimit = 500;
            this.ReturnDetails = false;
        }
    }
    [ProtoContract]
    [Serializable]
    public class FilterInfoResponse
    {
        [ProtoMember(1)]
        public List<SpiderOrderInfo> Orders { get; set; }
        [ProtoMember(2)]
        public int ErrCode { get; set; }
        [ProtoMember(3)]
        public string ErrMsg { get; set; }
        [ProtoMember(4)]
        public string Id { get; set; }
        public FilterInfoResponse()
        {
            this.Orders = new List<SpiderOrderInfo>();
            this.ErrCode = 0;
            this.ErrMsg = string.Empty;
        }
    }
}
