using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.Lib.Dados
{
    public class PlataformaInfo
    {
        public int CodigoCliente { get; set; }

        public int CodigoPlataforma { get; set; }

        public int CodigoAcesso { get; set; }

        public string DescricaoAcesso { get; set; }

        public Nullable<int> CodigoSessao { get; set; }

        public DateTime DataAtualizacao { get; set; }
    }
}
