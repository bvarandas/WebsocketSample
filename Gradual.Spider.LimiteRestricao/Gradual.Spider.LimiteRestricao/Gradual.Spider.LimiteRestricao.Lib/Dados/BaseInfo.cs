using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Gradual.Spider.LimiteRestricao.Lib.Dados
{
    [DataContract]
    public class BaseInfo
    {
        [DataMember]
        public List<string> Criticas = null;

        public BaseInfo()
        {
            this.Criticas = new List<string>();
        }
    }
}
