using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.OMS.Library;
using Gradual.Spider.Lib;

namespace Gradual.Spider.Lib.Dados
{
    
    public class LogSpiderInfo : ICodigoEntidade
    {
        public int IdLogIntranet { get; set; }

        public string DsCpfCnpjClienteAfetado { get; set; }

        public int? IdLoginClienteAfetado { get; set; }

        public int? IdClienteAfetado { get; set; }

        public int? CdBovespaClienteAfetado { get; set; }

        public int IdLogin { get; set; }

        public string DsIp { get; set; }

        public string DsTela { get; set; }

        public string DsObservacao { get; set; }

        public TipoAcaoUsuario IdAcao { get; set; }

        public DateTime DtEvento { get; set; }

        public string ReceberCodigo()
        {
            throw new NotImplementedException();
        }
    }
    
}
