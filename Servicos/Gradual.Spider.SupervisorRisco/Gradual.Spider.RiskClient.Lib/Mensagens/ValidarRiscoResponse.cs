using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using Gradual.Spider.SupervisorRisco.Lib.Dados;

namespace Gradual.Spider.RiskClient.Lib.Mensagens
{
    [ProtoContract]
    [Serializable]
    public class ValidarRiscoResponse
    {
        [ProtoMember(1)]
        public string RejectMessage { get; set; }

        [ProtoMember(2)]
        public bool ValidationResult { get; set; }

        [ProtoMember(3)]
        public TipoLimiteEnum TipoLimite { get; set; }

        public ValidarRiscoResponse()
        {
            this.TipoLimite = TipoLimiteEnum.INDEFINIDO;
        }

    }
}
