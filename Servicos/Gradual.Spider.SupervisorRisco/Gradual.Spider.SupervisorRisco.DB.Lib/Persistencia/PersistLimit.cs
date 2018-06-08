using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Configuration;
using log4net.Core;
using Gradual.Spider.SupervisorRisco.Lib.Dados;

namespace Gradual.Spider.SupervisorRisco.DB.Lib.Persistencia
{
    public class PersistLimit
    {

        protected ILog loggerCliente;
        string _pathMvto;

        //int _codCliente;
        public int CodCliente
        {
            get;
            internal set;
        }

        public string Exchange
        {
            get;
            internal set;
        }

        public PersistLimit(string exchange, int codCliente)
        {
            _pathMvto = string.Empty;
            this.CodCliente = codCliente;
            this.Exchange = exchange;

            string appender = exchange + "." + this.CodCliente.ToString();
            loggerCliente = LogManager.GetLogger(appender);
            this.AddAppender(appender, loggerCliente.Logger);
        }

        public void AddAppender(string appenderName, ILogger wLogger)
        {
            if (!ConfigurationManager.AppSettings.AllKeys.Contains("FinancialMovementPath"))
                throw new Exception("Parameter 'FinancialMovementPath' is mandatory");

            string filename = ConfigurationManager.AppSettings["FinancialMovementPath"].ToString() + "\\" + appenderName + ".log";

            log4net.Repository.Hierarchy.Logger l = (log4net.Repository.Hierarchy.Logger)wLogger;

            log4net.Appender.IAppender hasAppender = l.GetAppender(appenderName);
            if (hasAppender == null)
            {
                log4net.Appender.RollingFileAppender appender = new log4net.Appender.RollingFileAppender();
                appender.DatePattern = "yyyyMMdd";
                appender.RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Date;
                appender.AppendToFile = true;
                appender.File = filename;
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

        public bool InserirMvtoBovespa(OperatingLimitInfo item)
        {
            try
            {
                loggerCliente.InfoFormat("CodigoCliente: [{0}]   TipoLimite: [{1}]   PrecoBase: [{2}]   ValorAlocado: [{3}]   ValorDisponivel: [{4}]   ValorTotal: [{5}]",
                    item.CodigoCliente, item.TipoLimite, item.PrecoBase, item.ValorAlocado, item.ValorDisponivel, item.ValotTotal);
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public bool InserirMvtoBmf(ClientLimitBMFInfo item)
        {
            try
            {
                loggerCliente.InfoFormat("CodigoCliente: [{0}]   C.[{1}]  C.MaxOferta: [{2}]  C.Disp: [{3}] C.Total: [{4}] C.Sentido:[{5}] C.DataMovimento:[{6}]",
                                         item.Account, item.ContractLimit[0].Contrato, item.ContractLimit[0].QuantidadeMaximaOferta,
                                         item.ContractLimit[0].QuantidadeDisponivel, item.ContractLimit[0].QuantidadeTotal, item.ContractLimit[0].Sentido,
                                         item.ContractLimit[0].DataMovimento.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                loggerCliente.InfoFormat("CodigoCliente: [{0}]   I.[{1}]  I.MaxOferta: [{2}]  I.Disp: [{3}] I.Total: [{4}] I.ContratoPai:[{5}] I.Sentido:[{6}] I.DataMovimento:[{7}]",
                                         item.Account, item.InstrumentLimit[0].Instrumento, item.InstrumentLimit[0].QuantidadeMaximaOferta,
                                         item.InstrumentLimit[0].QtDisponivel, item.InstrumentLimit[0].QtTotalInstrumento, item.InstrumentLimit[0].QtTotalContratoPai,
                                         item.InstrumentLimit[0].Sentido, item.InstrumentLimit[0].dtMovimento.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                
                return true;
            }
            catch 
            {
                return false;
            }
        }
    }
}
