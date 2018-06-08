using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using Gradual.Core.Spider.OrderFixProcessing.Lib.Dados;

namespace Gradual.Spider.Acompanhamento4Socket.Lib.Mensagem
{
    [ProtoContract]
    [Serializable]
    public class FilterDetailInfoRequest
    {
        [ProtoMember(1, IsRequired = true)]
        public int OrderID { get; set; }
        [ProtoMember(2, IsRequired = true)]
        public string Id { get; set; }
        public FilterDetailInfoRequest()
        {
            this.OrderID = 0;
            this.Id = string.Empty;
        }
    }

    [ProtoContract]
    [Serializable]
    public class FilterDetailInfoResponse
    {
        [ProtoMember(1)]
        public List<SpiderOrderDetailInfo> Details { get; set; }
        [ProtoMember(2, IsRequired = true)]
        public int ErrCode { get; set; }
        [ProtoMember(3, IsRequired = true)]
        public string ErrMsg { get; set; }
        [ProtoMember(4, IsRequired = true)]
        public string Id { get; set; }

        public FilterDetailInfoResponse()
        {
            this.Details = new List<SpiderOrderDetailInfo>();
            this.ErrCode = 0;
            this.ErrMsg = string.Empty;
        }
    }
}
