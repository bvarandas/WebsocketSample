﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Gradual.Spider.LimiteRestricao.Lib.Dados;
using Gradual.OMS.Library;

namespace Gradual.Spider.LimiteRestricao.Lib.Mensagens
{
    [DataContract]
    [Serializable]
    public class RiscoSelecionarPlataformaAssessorRequest : MensagemRequestBase
    {
        [DataMember]
        public RiscoPlataformaAssessorInfo Objeto { get; set; }

        public RiscoSelecionarPlataformaAssessorRequest()
        {
            this.Objeto = new RiscoPlataformaAssessorInfo();
        }
    }
}
