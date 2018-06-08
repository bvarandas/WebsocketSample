﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.OMS.Library;
using Gradual.Spider.LimiteRestricao.Lib.Dados;
using System.Runtime.Serialization;

namespace Gradual.Spider.LimiteRestricao.Lib.Mensagens
{
    [DataContract]
    [Serializable]
    public class RiscoListarPermissoesResponse : MensagemResponseBase
    {
        public RiscoListarPermissoesResponse()
        {
            this.Permissoes = new List<RiscoPermissaoInfo>();
		}

        [DataMember]
        public List<RiscoPermissaoInfo> Permissoes { get; set; }

        ~RiscoListarPermissoesResponse()
        {
            Permissoes = new List<RiscoPermissaoInfo>();
		}


        public override string ToString()
        {
            string lRetorno = "{";

            if (null != Permissoes)
            {
                foreach (RiscoPermissaoInfo item in Permissoes)
                {
                    lRetorno += item.ToString();
                }
            }

            lRetorno += "}";
            return lRetorno;
        }
    }
}