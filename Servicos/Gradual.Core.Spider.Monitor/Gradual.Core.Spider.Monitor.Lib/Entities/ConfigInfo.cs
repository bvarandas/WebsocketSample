using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Core.Spider.Monitoring.Lib.Entities
{
    public class ConfigInfo
    {
        public int IdConfig { get; set; }
        public string Chave { get; set; }
        public string Valor { get; set; }
        public string Description { get; set; }
        public DateTime DateReg { get; set; }
        
        public ConfigInfo()
        {
            this.DateReg = DateTime.MinValue;
        }
    }
}
