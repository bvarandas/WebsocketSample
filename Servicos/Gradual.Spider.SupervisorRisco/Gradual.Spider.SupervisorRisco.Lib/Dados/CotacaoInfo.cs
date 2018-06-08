using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.SupervisorRisco.Lib.Dados
{
    public class CotacaoInfo
    {
        public string Instrumento   { get; set; }

        public string TipoMercado   { get; set; }

        public decimal UltimoPreco  { get; set; }

        public decimal Ajuste       { get; set; }

        public decimal PU           { get; set; }

        public decimal Fechamento   { get; set; }

        public decimal Abertura     { get; set; }
    }
}
