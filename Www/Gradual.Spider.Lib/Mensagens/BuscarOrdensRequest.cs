using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.OMS.Library;

namespace Gradual.Spider.Lib
{
    public class BuscarOrdensRequest : MensagemRequestBase
    {
        public BuscarOrdensRequest(){}

        public int? Canal { get; set; }
        public int? CodigoAssessor { get; set; }
        public int? CodigoBmfDoCliente { get; set; }
        public int? ContaDoCliente { get; set; }
        public DateTime? DataAte { get; set; }
        public DateTime? DataDe { get; set; }
        public int? IdSistemaOrigem { get; set; }
        public string Instrumento { get; set; }
        public string Origem { get; set; }
        public int? PaginaCorrente { get; set; }
        public int? QtdeLimiteRegistros { get; set; }
        public int? Status { get; set; }
        public int? TotalRegistros { get; set; }
    }
}
