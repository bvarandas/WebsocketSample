using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Core.Spider.AcompanhamentoOrdens.Lib.Mensageria
{
    public class OrderFilterInfo
    {
        public List<int> Conta { get; set; }
        public List<string> Ativo { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public List<string> Sessao { get; set; }
        public List<string> Sentido { get; set; }
        public List<string> Bolsa { get; set; }
        public List<int> OrderStatusID { get; set; }
        public List<string> HandlInst { get; set; }

        public OrderFilterInfo()
        {
            this.Conta = new List<int>();
            this.Ativo = new List<string>(); ;
            this.DataInicial = DateTime.MinValue;
            this.DataFinal = DateTime.MinValue;
            this.Sessao = new List<string>();
            this.Bolsa = new List<string>();
            this.Sentido = new List<string>();
            this.OrderStatusID = new List<int>();
            this.HandlInst = new List<string>();
        }
    }
}
