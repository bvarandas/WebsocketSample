using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.Spider.SupervisorRisco.Lib.Dados;

namespace Gradual.Spider.SupervisorRisco.DB.Lib.Persistencia
{
    public class PersistMaxLoss:PersistBase
    {

        public PersistMaxLoss(string app)
            : base(app)
        {
        }

        public override void TraceInfo(object info)
        {
            OperatingLimitInfo aux = info as OperatingLimitInfo;
            if (aux != null)
                loggerCliente.InfoFormat("[MaxLoss] Account[{0}] ValorDisponivel [{1}] ValorAlocado [{2}] ValorTotal [{3}] ValorMovimento [{4}]",
                    aux.CodigoCliente, aux.ValorDisponivel, aux.ValorAlocado, aux.ValotTotal, aux.ValorMovimento);
            else
                loggerCliente.Error("Erro: Problemas no parser");
            
        }
    }
}
