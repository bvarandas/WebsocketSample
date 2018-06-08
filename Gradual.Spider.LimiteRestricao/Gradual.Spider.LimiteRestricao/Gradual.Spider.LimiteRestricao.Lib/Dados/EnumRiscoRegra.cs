using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Gradual.Spider.LimiteRestricao.Lib.Dados
{
    [Serializable]
    [DataContract]
    public class EnumRiscoRegra
    {
        [Serializable]
        public enum TipoGrupo
        {
            Nenhum = 0,
            GrupoDeRisco     = 1,
            GrupoAlavancagem = 2,
        }
    }
}
