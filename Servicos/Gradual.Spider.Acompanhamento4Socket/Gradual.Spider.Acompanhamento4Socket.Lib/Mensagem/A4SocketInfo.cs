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
    public class A4SocketRequest
    {
        [ProtoMember(1)]
        public string AppDescription { get; set; }

        public A4SocketRequest()
        {
            this.AppDescription = string.Empty;
        }
    }

    [ProtoContract]
    [Serializable]
    public class A4SocketResponse
    {
        [ProtoMember(1)]
        public int ErrorCode { get; set; }
        [ProtoMember(2)]
        public string ErrorMsg { get; set; }


        public A4SocketResponse()
        {
            this.ErrorCode = A4Messages.CODE_OK;
            this.ErrorMsg = A4Messages.MSG_OK;
        }
    }

}
