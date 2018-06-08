using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.Lib.Dados
{
    public class ClienteInfo :BaseInfo
    {
        public int IdLogin { get; set; }

        public string Nome { get; set; }

        public string Email { get; set; }

        public int CodigoAssessor { get; set; }

        public int CodigoBovespa { get; set; }

        public int CodigoBmf { get; set; }

        public string CodigoSessao { get; set; }

        public int CodigoLocalidade { get; set; }

        public string ContaMae { get; set; }

        #region Propriedades SINACOR
        public string DsCpfCnpj { get; set; }

        public bool EhPessoaVinculada { get; set; }

        public string TipoPessoa { get; set; }
        #endregion
    }
}
