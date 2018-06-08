using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.SupervisorRisco.Lib.Dados
{
    public class CRLucroPrej
    {

        public string Ativo             { get; set; }

        public int Account              { get; set; }

        public decimal LucroPrejuizo    { get; set; }

        public decimal LucroPrejuizoSemAbertura { get; set; }

        public string Bolsa             { get; set; }

        public decimal NetFinan         { get; set; }

        public decimal FinancAbertura   { get; set; }

        public CRLucroPrej()
        {
            this.Ativo          = string.Empty;
            this.Account        = 0;
            this.LucroPrejuizo  = decimal.Zero;
            this.Bolsa          = string.Empty;
            this.NetFinan       = 0;
            this.FinancAbertura = decimal.Zero;
            
        }


    }
    public class ListClientPosition
    {
        public int Account { get; set; }

        public string Ativo { get; set; }

        public decimal NetFinan {get; set;}

        public string Bolsa { get; set; }

        public ListClientPosition()
        {
            this.Ativo = string.Empty;
            this.Account = 0;
            
            this.Bolsa = string.Empty;
            this.NetFinan = 0;
        }

    }

}
