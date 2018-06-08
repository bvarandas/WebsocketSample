using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Core.Spider.Monitoring.Lib.Entities
{
    public class MonitorInfo
    {
        public int IdMonitor { get; set; }
        public string KeyValue { get; set; }
        public string Description { get; set; }
        public DateTime UpdateTime { get; set; }
        public bool Active { get; set; }
        public Dictionary<string, string> Content { get; set; }
        public MonitorInfo()
        {
            this.IdMonitor = 0;
            this.KeyValue = string.Empty;
            this.Description = string.Empty;
            this.UpdateTime = DateTime.MinValue;
            this.Active = false;
            this.Content = new Dictionary<string,string>();
        }
    }
}
