using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Core.Spider.Monitoring.Lib.Entities
{
    public class FixSessionInfo
    {
        public MonitorInfo MainInfo;
        public int IdMonitor { get; set; }
        public int IdFix { get; set; }
        public int ActiveUsers { get; set; }
        public int OrderCount { get; set; }
        public int TransactionCount { get; set; }
        public DateTime LastMessage { get; set; }
        public DateTime UpdateTime { get; set; }
        public string BeginString { get; set;}
        public string TargetCompID { get; set; }
        public string TargetSubID { get; set; }
        public string SenderCompID { get; set; }
        public string SenderSubID { get; set; }
        // public int IdType { get; set; }

        
        public FixSessionInfo()
        {
            this.MainInfo = new MonitorInfo();
            this.IdMonitor = 0;
            this.IdFix = 0;
            this.ActiveUsers = 0;
            this.OrderCount = 0;
            this.LastMessage = DateTime.MinValue;
            this.BeginString = string.Empty;
            this.TargetCompID = string.Empty;
            this.TargetSubID = string.Empty;
            this.SenderCompID = string.Empty;
            this.SenderSubID = string.Empty;
        }

        public string ComposeSessionID()
        {
            string ret;

            if (string.IsNullOrEmpty(TargetSubID) || string.IsNullOrEmpty(SenderSubID))
                ret = string.Format("{0}:{1}->{2}", BeginString, SenderCompID, TargetCompID);
            else
                ret = string.Format("{0}:{1}/{2}->{3}/{4}", BeginString, SenderCompID, SenderSubID, TargetCompID, TargetSubID);
            return ret;
        }
    }
}
