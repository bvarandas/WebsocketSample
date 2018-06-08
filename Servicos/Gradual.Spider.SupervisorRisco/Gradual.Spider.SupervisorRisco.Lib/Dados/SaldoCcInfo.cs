using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.SupervisorRisco.Lib.Dados
{
    public class SaldoCcInfo
    {
        public int Account { get; set; }
        public decimal SaldoD0 { get; set; }
        public decimal SaldoD1 { get; set; }
        public decimal SaldoD2 { get; set; }
        public decimal SaldoD3 { get; set; }
        public decimal SaldoContaMargem { get; set; }
        public decimal SaldoContaBloqueado { get; set; }
        public DateTime DtMovimento { get; set; }

        public SaldoCcInfo()
        {
            this.Account = -1;
            this.SaldoContaBloqueado = decimal.Zero;
            this.SaldoD0 = decimal.Zero;
            this.SaldoD1 = decimal.Zero;
            this.SaldoD2 = decimal.Zero;
            this.SaldoD3 = decimal.Zero;
            this.SaldoContaMargem = decimal.Zero;
            this.DtMovimento = DateTime.MinValue;
        }
    }
}
