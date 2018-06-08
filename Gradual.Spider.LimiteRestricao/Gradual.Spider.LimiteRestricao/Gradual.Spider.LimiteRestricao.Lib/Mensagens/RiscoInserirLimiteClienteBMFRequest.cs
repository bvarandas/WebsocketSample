﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Gradual.Spider.LimiteRestricao.Lib.Dados;
using Gradual.OMS.Library;

namespace Gradual.Spider.LimiteRestricao.Lib.Mensagens
{
    [Serializable]
    [DataContract]
    public class RiscoInserirLimiteClienteBMFRequest : MensagemRequestBase
    {
        [DataMember]
        public ClienteParametroLimiteBMFInfo ClienteParametroLimiteBMFInfo = new ClienteParametroLimiteBMFInfo();
    }
}
