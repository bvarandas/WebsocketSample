using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.Lib.Dados
{
    public class AssessorInfo :BaseInfo
    {
        public string NomeAssessor          { get; set; }

        public int IdLoginAssessor          { get; set; }

        public bool StAtivo                 { get; set; }

        public string ListaAssessoresFilhos { get; set; }

        public string CodigoOperador        { get; set; }

        public string CodigoSessao          { get; set; }

        public string CodigoSigla           { get; set; }

        public int CodigoLocalidade         { get; set; }

        public string DsLocalidade          { get; set; }

        public DateTime DtAtualizacao       { get; set; }

        public string Email                 { get; set; }

        public int CodigoAssessor           { get; set; }
    }
}
