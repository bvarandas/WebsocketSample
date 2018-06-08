using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Gradual.Spider.LimiteRestricao.Lib.Dados
{
    [DataContract]
    public class RiscoClienteParametroGrupoInfo
    {
        [DataMember]
        public int IdCliente { get; set; }

        [DataMember]
        public int IdParametro { get; set; }

        [DataMember]
        public int IdGrupo { get; set; }

        public string ReceberCodigo()
        {
            throw new System.NotImplementedException();
        }
    }
}
