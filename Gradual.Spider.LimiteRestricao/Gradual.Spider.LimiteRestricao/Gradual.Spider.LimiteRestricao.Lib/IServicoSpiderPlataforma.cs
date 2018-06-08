using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Gradual.Spider.LimiteRestricao.Lib.Mensagens;

namespace Gradual.Spider.LimiteRestricao.Lib
{
    [ServiceContract(Namespace = "http://gradual")]
    public interface IServicoSpiderPlataforma
    {
        [OperationContract]
        RiscoListarPlataformaResponse ListaPlataformaSpider();

        [OperationContract]
        RiscoListarOperadorResponse ListarOperadorSpider();

        [OperationContract]
        RiscoSelecionarPlataformaClienteResponse SelecionarPlataformaClienteSpider(RiscoSelecionarPlataformaClienteRequest pParametro);

        [OperationContract]
        RiscoSelecionarPlataformaContaMasterResponse SelecionarPlataformaContaMasterSpider(RiscoSelecionarPlataformaContaMasterRequest pParametro);

        [OperationContract]
        RiscoSelecionarPlataformaAssessorResponse SelecionarPlataformaAssessorSpider(RiscoSelecionarPlataformaAssessorRequest pParametro);

        [OperationContract]
        RiscoSelecionarPlataformaOperadorResponse SelecionarPlataformaOperadorSpider(RiscoSelecionarPlataformaOperadorRequest pParametro);

        [OperationContract]
        RiscoSalvarPlataformaClienteResponse SalvarPlataformaClienteSpider(RiscoSalvarPlataformaClienteRequest pParametro);

        [OperationContract]
        RiscoSalvarPlataformaContaMasterResponse SalvarPlataformaContaMasterSpider(RiscoSalvarPlataformaContaMasterRequest pParametro);

        [OperationContract]
        RiscoSalvarPlataformaAssessorResponse SalvarPlataformaAssessorSpider(RiscoSalvarPlataformaAssessorRequest pParametro);

        [OperationContract]
        RiscoSalvarPlataformaOperadorResponse SalvarPlataformaOperadorSpider(RiscoSalvarPlataformaOperadorRequest pParametro);
    }
}
