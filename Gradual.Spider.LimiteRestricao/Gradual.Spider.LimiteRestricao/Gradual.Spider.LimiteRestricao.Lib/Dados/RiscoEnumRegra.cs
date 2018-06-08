using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Gradual.Spider.LimiteRestricao.Lib.Dados
{
    [Serializable]
    [DataContract]
    public class RiscoEnumRegra
    {
        [Serializable]
        public enum TipoGrupo
        {
            GrupoDeRisco = 1,
            GrupoAlavancagem = 2,
        }
    }
}
