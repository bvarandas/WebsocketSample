using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.RiskClient.Lib
{
    public class CotacaoManager
    {
        private static CotacaoManager _me = null;

        public static CotacaoManager Instance
        {
            get
            {
                if (_me == null)
                {
                    _me = new CotacaoManager();
                }

                return _me;
            }
        }


        public Decimal GetLastPrice(string instrumento)
        {
            return Decimal.Zero;
        }

        internal void AddInstrument(string symbol, SupervisorRisco.Lib.Dados.SymbolInfo instrument)
        {
            throw new NotImplementedException();
        }
    }
}
