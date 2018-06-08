using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.Spider.SupervisorRisco.Lib.Dados;
using ProtoBuf;

namespace Gradual.Spider.SupervisorRisco.Lib.Dados
{
    [Serializable]
    [ProtoContract]
    public class BlockedInstrumentInfo
    {
        /// <summary>
        /// Código do cliente
        /// </summary>
        [ProtoMember(1,IsRequired=true)]
        public int IdCliente { set; get; }

        /// <summary>
        /// Código do Instrumento
        /// </summary>
        [ProtoMember(2)]
        public string Instrumento { set; get; }

        /// <summary>
        /// Sentido da Ordem
        /// </summary>
        [ProtoMember(3)]
        public SentidoBloqueioEnum Sentido { set; get; }
    }
}
