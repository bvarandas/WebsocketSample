using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.Lib.Dados
{
    public class PlataformaSessaoInfo
    {
        public int CodigoPlataformaSessao { get; set; }

        public int CodigoPlataforma { get; set; }

        public string NomePlataforma { get; set; }

        public int CodigoSessao { get; set; }

        public string NomeSessao { get; set; }

        public decimal ValorPlataforma { get; set; }

        public bool StAtivo { get; set; }

        public DateTime DataUltimoEvento { get; set; }

        public int CodigoAcesso { get; set; }

        public string NomeAcesso { get; set; }

        public string Finalidade { get; set; }
    }
}
