using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.Spider.SupervisorRisco.Lib.Dados;

namespace Gradual.Spider.SupervisorRisco.DB.Lib.Persistencia
{
    public class PersistPositionClient:PersistBase
    {

        public PersistPositionClient(string app, bool truncate = false)
            : base(app, truncate)
        {

        }

        public override void TraceInfo(object info)
        {
            PosClientSymbolInfo aux = info as PosClientSymbolInfo;
            if (aux != null)
                loggerCliente.InfoFormat("[PositionClient] Account[{0}] Bolsa[{1}] Symbol[{2}] Var[{3}] QtdAbertura[{4}] QtdExecC[{5}] " +
                                        "QtdExecV[{6}] NetExec[{7}] QtdAbC[{8}] QtdAbV[{9}] NetAb[{10}] " +
                                        "PcMedC[{11}] PcMedV[{12}] FinancNet[{13}] LucroPrej[{14}] CodCarteira[{15}] ExecBroker[{16}] DtMovimento [{17}]",
                                        aux.Account, aux.Bolsa, aux.Ativo, aux.Variacao, aux.QtdAbertura, aux.QtdExecC,
                                        aux.QtdExecV, aux.NetExec, aux.QtdAbC, aux.QtdAbV, aux.NetAb,
                                        aux.PcMedC, aux.PcMedV, aux.FinancNet, aux.LucroPrej, 
                                        aux.CodCarteira, aux.ExecBroker, aux.DtMovimento.ToString("dd-MM-yyyy HH:mm:ss.fff"));


            else
                loggerCliente.Error("Erro: Problemas no parser");

        }
    }
}
