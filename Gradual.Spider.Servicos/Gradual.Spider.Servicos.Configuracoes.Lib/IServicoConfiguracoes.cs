using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Gradual.Spider.Servicos.Configuracoes.Lib
{
    [ServiceContract(Namespace = "http://gradual")]
    public interface IServicoConfiguracoes
    {
        [OperationContract]
        Gradual.Spider.Servicos.Configuracoes.Lib.Mensageria.BuscarParametrosResponse BuscarParametros(Gradual.Spider.Servicos.Configuracoes.Lib.Mensageria.BuscarParametrosRequest pRequest);
    }
}
