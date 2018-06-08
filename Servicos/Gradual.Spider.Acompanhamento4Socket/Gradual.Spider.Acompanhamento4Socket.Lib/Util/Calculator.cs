using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.Core.Spider.OrderFixProcessing.Lib.Dados;

namespace Gradual.Spider.Acompanhamento4Socket.Lib.Util
{
    public static class Calculator
    {
        public static decimal CalculateWeightedAvgPx(List<SpiderOrderDetailInfo> lst)
        {
            try
            {
                decimal wavg = lst
                    .Where(x => x.OrderStatusID == (int)SpiderOrderStatus.EXECUTADA || x.OrderStatusID == (int)SpiderOrderStatus.PARCIALMENTEEXECUTADA)
                    .WeightedAverage(o => o.Price, o => o.TradeQty);

                List<SpiderOrderDetailInfo> aux = lst.Where(x => x.OrderStatusID == (int)SpiderOrderStatus.EXECUTADA || x.OrderStatusID == (int)SpiderOrderStatus.PARCIALMENTEEXECUTADA).ToList();
                decimal ret = decimal.Zero;
                int len = aux.Count;
                decimal soma = 0.0M;
                int qtd = 0;
                for (int i = 0; i < len; i++)
                {
                    soma = soma + (aux[i].TradeQty * aux[i].Price);
                    qtd = qtd + aux[i].TradeQty;
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

        public static decimal CalculateAvgPx(List<SpiderOrderDetailInfo> lst)
        {
            try
            {
                decimal avg = lst
                    .Where(x => x.OrderStatusID == (int)SpiderOrderStatus.EXECUTADA || x.OrderStatusID == (int)SpiderOrderStatus.PARCIALMENTEEXECUTADA)
                    .Average(o => o.Price);

                List<SpiderOrderDetailInfo> aux = lst.Where(x => x.OrderStatusID == (int)SpiderOrderStatus.EXECUTADA || x.OrderStatusID == (int)SpiderOrderStatus.PARCIALMENTEEXECUTADA).ToList();
                decimal ret = decimal.Zero;
                int len = aux.Count;
                decimal soma = 0.0M;
                int qtd = 0;
                for (int i = 0; i < len; i++)
                {
                    soma = soma + aux[i].Price;
                }
                if (len !=0)
                    ret = soma / len;
                return ret;
            }
            catch
            {
                return -1.0M;
            }
        }

        public static void CalculateAllPxAverages(List<SpiderOrderDetailInfo> lst, out decimal average, out decimal weighted)
        {
            try
            {
                average = weighted = -1.0M;

                decimal somaSimples = 0.0M;
                decimal somaPond = 0.0M;
                int qtd = 0;
                int len = 0;

                foreach (SpiderOrderDetailInfo info in lst)
                {
                    if (info.OrderStatusID == (int)SpiderOrderStatus.EXECUTADA ||
                        info.OrderStatusID == (int)SpiderOrderStatus.PARCIALMENTEEXECUTADA)
                    {
                        somaSimples += info.Price;
                        somaPond += (info.TradeQty * info.Price);
                        qtd += info.TradeQty;
                        len++;
                    }
                }

                if (qtd != 0)
                    weighted = somaPond / qtd;

                if (len != 0)
                    average = somaSimples / len;
            }
            catch
            {
                average = weighted= - 1.0M;
            }
        }

    }
}
