using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.Spider.SupervisorRisco.Lib.Dados;

namespace Gradual.Spider.SupervisorRisco.Lib.Util
{
    public class Tools
    {
        public static decimal CalculateWeightedAvgPx(List<ExecSymbolInfo> lst)
        {
            try
            {
                decimal ret = decimal.Zero;
                int len = lst.Count;
                decimal soma = 0.0M;
                int qtd = 0;
                for (int i = 0; i < len; i++)
                {
                    soma = soma + (lst[i].Qty * lst[i].Price);
                    qtd = qtd + lst[i].Qty;
                }
                if (qtd != 0)
                    ret = soma / qtd;
                return ret;
            }
            catch
            {
                return -1.0M;
            }
        }


    }
}
