﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Gradual.Spider.LimiteRestricao.Lib.Mensagens
{
    [Serializable]
    [DataContract]
    public class RiscoListarLimiteBMFRequest
    {
        [DataMember]
        public int Account { set; get; }
    }
}