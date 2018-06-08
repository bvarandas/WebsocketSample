﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using Gradual.Spider.SupervisorRisco.Lib.Dados;

namespace Gradual.Spider.SupervisorRisco.Lib.Mensagens
{
    [Serializable]
    [ProtoContract]
    public class SincroniaDicInstrBloqCliente
    {
        [ProtoMember(1)]
        public bool IsSnapshot { get; set; }

        [ProtoMember(2)]
        public Dictionary<SymbolKey, BlockedInstrumentInfo> BlockedSymbolClient { get; set; }
    }
}
