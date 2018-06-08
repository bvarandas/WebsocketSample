using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Configuration;
using log4net.Core;
using System.IO;

namespace Gradual.Spider.SupervisorRisco.DB.Lib.Persistencia
{
    public class PersistBase
    {

        protected ILog loggerCliente;
        string _pathMvto;
        string _fileName;

        public PersistBase(string appName, bool truncateFile = false)
        {
            string appender = appName;
            _fileName = ConfigurationManager.AppSettings["FinancialMovementPath"].ToString() + "\\" + appender + ".log";
            if (truncateFile)
            {
                if (File.Exists(_fileName))
                    File.Delete(_fileName);
            }
            loggerCliente = LogManager.GetLogger(appender);
            this.AddAppender(appender, loggerCliente.Logger);
        }

        public void AddAppender(string appenderName, ILogger wLogger)
        {
            if (!ConfigurationManager.AppSettings.AllKeys.Contains("FinancialMovementPath"))
                throw new Exception("Parameter 'FinancialMovementPath' is mandatory");

            //string filename = ConfigurationManager.AppSettings["FinancialMovementPath"].ToString() + "\\" + appenderName + ".log";
            
            log4net.Repository.Hierarchy.Logger l = (log4net.Repository.Hierarchy.Logger)wLogger;

            log4net.Appender.IAppender hasAppender = l.GetAppender(appenderName);
            if (hasAppender == null)
            {
                log4net.Appender.RollingFileAppender appender = new log4net.Appender.RollingFileAppender();
                appender.DatePattern = "yyyyMMdd";
                appender.RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Date;
                appender.AppendToFile = true;
                appender.File = _fileName;
                appender.StaticLogFileName = true;
                appender.Name = appenderName;

                log4net.Layout.PatternLayout layout = new log4net.Layout.PatternLayout();
                layout.ConversionPattern = "%date [%thread] %-5level %logger - %message%newline";
                layout.ActivateOptions();

                appender.Layout = layout;
                appender.ActivateOptions();

                l.AddAppender(appender);
            }
        }

        public virtual void TraceInfo(object info) { }

    }
}
