using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.OMS.Library;

namespace Gradual.Spider.Lib.Dados
{
    public class EsqueciSenhaInfo :  BaseInfo 
    {
        public string DsEmail { get; set; }
        public string DsCpfCnpj { get; set; }
        public DateTime DtNascimentoFundacao { get; set; }
        public string CdSenha { get; set; }
        public bool StAlteracaoFuncionario { get; set; }
    }
}
