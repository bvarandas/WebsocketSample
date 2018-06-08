using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.Spider.SupervisorRisco.Lib.Dados;

namespace Gradual.Spider.SupervisorRisco.DB.Lib.Persistencia
{
    public class PersistSymbol:PersistBase
    {

        public PersistSymbol(string appName)
            : base(appName)
        {

        }


        public override void TraceInfo(object info)
        {
            RestrictionSymbolInfo rest = info as RestrictionSymbolInfo;

            if (rest != null)
                loggerCliente.InfoFormat("[RestrictionLog] - Symbol [{0}] LimiteVolumeNet [{1}] QuantidadeNet [{2}] LimiteMaxOfertaVolume [{3}] LimiteMaxOfertaQtde [{4}] VolumeNetAlocado [{5}] QuantidadeNetAlocada [{6}]  DtAtualizacao [{7}]",
                    rest.Symbol, rest.LimiteVolumeNet, rest.QuantidadeNet,
                    rest.LimiteMaxOfertaVolume, rest.LimiteMaxOfertaQtde, rest.VolumeNetAlocado,
                    rest.QuantidadeNetAlocada, rest.DtAtualizacao);
            else
                loggerCliente.Error("Erro: Problemas no parser");
                
        }

    }
}
