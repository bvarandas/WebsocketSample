using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.OMS.Library;
using System.Runtime.Serialization;

namespace Gradual.Spider.LimiteRestricao.Lib.Dados
{
    [DataContract]
    public class LogSpiderInfo : ICodigoEntidade
    {
        [DataMember]
        public int IdLogIntranet { get; set; }

        [DataMember]
        public string DsCpfCnpjClienteAfetado { get; set; }

        [DataMember]
        public int? IdLoginClienteAfetado { get; set; }

        [DataMember]
        public int? IdClienteAfetado { get; set; }

        [DataMember]
        public int? CdBovespaClienteAfetado { get; set; }

        [DataMember]
        public int IdLogin { get; set; }

        [DataMember]
        public string DsIp { get; set; }

        [DataMember]
        public string DsTela { get; set; }

        [DataMember]
        public string DsObservacao { get; set; }

        [DataMember]
        public TipoAcaoUsuario IdAcao { get; set; }

        [DataMember]
        public DateTime DtEvento { get; set; }

        public string ReceberCodigo()
        {
            throw new NotImplementedException();
        }
    }
    
}
