using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Gradual.Spider.SupervisorRisco.Lib.Dados
{
    [Serializable]
    [ProtoContract]
    public class ClientParameterPermissionInfo
    {

        [ProtoMember(1, IsRequired=true)]
        public int IdCliente { get; set; }

        [ProtoMember(2)]
        public List<ParameterPermissionClientInfo> Parametros {get;set;}

        [ProtoMember(3)]
        public List<ParameterPermissionClientInfo> Permissoes {get; set; }

        public ClientParameterPermissionInfo()
        {
            this.Parametros = new List<ParameterPermissionClientInfo>();
            this.Permissoes = new List<ParameterPermissionClientInfo>();
            this.IdCliente = -1;
        }
    }
}
