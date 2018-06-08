using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.SupervisorRisco.Lib.Dados
{
    public class MdsTradingInfo
    {
        //public MdsNegociosEventArgs Quotes;
        public string Quotes;
        public TradingInfo Trading;

        public MdsTradingInfo()
        {
            this.Quotes = null;
            this.Trading = null;
        }
    }
}
