using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Gradual.Spider.Communications.Lib.Mensagens
{
    [ProtoContract]
    [Serializable]
    public class CancelAssinaturaCommServerRequest
    {
        /// <summary>
        /// Session ID conforme recebido apos o login junto ao communicationserver
        /// </summary>
        [ProtoMember(1)]
        public string SessionID { get; set; }

        /// <summary>
        /// Informa uma lista de tipos de Classes que esta aguardando serem recebidos como sinal
        /// ou publicados como resposta desta solicitacao de assinatura
        /// </summary>
        [ProtoMember(2)]
        public List<Type> TiposAssinados { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ProtoMember(3)]
        public string Instrumento { get; set; }

        /// <summary>
        /// Indica se eh o ultimo assinante
        /// </summary>
        [ProtoMember(4)]
        public bool UltimoAssinante { get; set; }

        public CancelAssinaturaCommServerRequest()
        {
            TiposAssinados = new List<Type>();
        }

    }
}
