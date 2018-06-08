using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.Core.Spider.Monitoring.Lib.Entities;

namespace Gradual.Core.Spider.Monitoring.Lib.Messages
{
    public class MonitorInfoResponse
    {
        public List<MonitorInfo> Infos { get; set; }
        public int ErrCode {get;set;} 
        public string ErrMsg {get;set;}
        
        public MonitorInfoResponse()
        {
            this.Infos = new List<MonitorInfo>();
            this.ErrCode = 0;
            this.ErrMsg = string.Empty;
        }

    }
}
