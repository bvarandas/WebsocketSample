using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.Lib.Dados
{
    public class GerenciadorPlataformaInfo
    {
        public int CodigoTraderPlataforma   { get; set; }

        public int CodigoTrader             { get; set; }

        public int CodigoPlataforma         { get; set; }

        public string NomePlataforma        { get; set; }

        public Nullable<int> CodigoSessao   { get; set; }

        public string NomeSessao            { get; set; }

        public DateTime DataUltimoEvento    { get; set; }

        public int CodigoAcesso             { get; set; }

        public string NomeAcesso            { get; set; }
    }
}
