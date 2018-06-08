using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.SupervisorRisco.Lib.Dados
{
    public class ClientDataInfo
    {
        public int CodigoBmf        { get; set; }

        public int CodigoBovespa    { get; set; }

        public string NomeCliente   { get; set; }

        public string DsCpfCnpj     { get; set; }

        public int CodigoAssessor   { get; set; }

        public string NomeAssessor  { get; set; }

        public string Bolsa         { get; set; }
    }
}
