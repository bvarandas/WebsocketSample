using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.OMS.Library;
using System.ServiceModel;
using Gradual.Spider.LimiteRestricao.Lib.Dados;
using Gradual.Spider.LimiteRestricao.Lib.Mensagens;

namespace Gradual.Spider.LimiteRestricao.Lib
{
    [ServiceContract(Namespace = "http://gradual")]
    public interface IServicoSpiderLimiteRestricao
    {
        [OperationContract]
        RiscoListarParametrosClienteResponse ListarLimitePorClienteSpider(RiscoListarParametrosClienteRequest pParametro);

        [OperationContract]
        RiscoListarPermissoesResponse ListarPermissoesRiscoSpider(RiscoListarPermissoesRequest pParametro);

        [OperationContract]
        RiscoListarPermissoesClienteResponse ListarPermissoesRiscoClienteSpider(RiscoListarPermissoesClienteRequest pParametro);

        [OperationContract]
        RiscoListarBloqueiroInstrumentoResponse ListarBloqueioPorClienteSpider(RiscoListarBloqueiroInstrumentoRequest pParametro);

        [OperationContract]
        RiscoListarClienteParametroGrupoResponse ListarClienteParametroGrupoSpider(RiscoListarClienteParametroGrupoRequest pParametro);

        [OperationContract]
        RiscoReceberParametroClienteResponse RiscoReceberParametroCliente(RiscoReceberParametroClienteRequest pRequest, Boolean pEfetuarLog = false);

        [OperationContract]
        RiscoSalvarParametroClienteResponse SalvarParametroRiscoClienteSpider(RiscoSalvarParametroClienteRequest pRequest);

        [OperationContract]
        RiscoSalvarParametroClienteResponse SalvarPermissoesRiscoAssociadasSpider(RiscoSalvarPermissoesAssociadasRequest pRequest);

        [OperationContract]
        RiscoSalvarParametroClienteResponse SalvarExpirarLimiteSpider(RiscoSalvarParametroClienteRequest pRequest);

        [OperationContract]
        RiscoListarGrupoItemResponse ListarGrupoItensSpider(RiscoListarGrupoItemRequest pParametro);

        [OperationContract]
        RiscoRemoverGrupoResponse RemoverGrupoRiscoSpider(RiscoRemoverGrupoRequest pRequest);

        [OperationContract]
        RiscoSalvarGrupoItemResponse SalvarGrupoItemSpider(RiscoSalvarGrupoItemRequest lRequest);

        [OperationContract]
        RiscoListarGruposResponse ListarGruposSpider(RiscoListarGruposRequest lRequest);

        [OperationContract]
        RiscoRemoverRegraGrupoItemResponse RemoverRegraGrupoItemSpider(RiscoRemoverRegraGrupoItemRequest pParametro);

        [OperationContract]
        RiscoRemoverBloqueioInstumentoResponse RemoverBloqueioClienteInstrumentoDirecaoSpider(RiscoRemoverClienteBloqueioRequest pParametro);

        [OperationContract]
        RiscoRemoverRegraGrupoItemResponse RemoverRegraGrupoItemGlobalSpider(RiscoRemoverRegraGrupoItemRequest pParametro);

        [OperationContract]
        RiscoListarRegraGrupoItemResponse ListarRegraGrupoItemGlobalSpider(RiscoListarRegraGrupoItemRequest pParametro);

        [OperationContract]
        RiscoListarBloqueiroInstrumentoResponse ListarBloqueioClienteInstrumentoDirecaoSpider(RiscoListarBloqueiroInstrumentoRequest pParametro);

        [OperationContract]
        RiscoListarRegraGrupoItemResponse ListarRegraGrupoItemSpider(RiscoListarRegraGrupoItemRequest pParametro);

        [OperationContract]
        RiscoSalvarBloqueioInstrumentoResponse SalvarClienteBloqueioInstrumentoDirecaoSpider(RiscoSalvarBloqueioInstrumentoRequest pParametro);

        [OperationContract]
        RiscoSalvarRegraGrupoItemResponse SalvarRegraGrupoItemGlobalSpider(RiscoSalvarRegraGrupoItemRequest pParametro);

        [OperationContract]
        RiscoSalvarRegraGrupoItemResponse SalvarRegraGrupoItemSpider(RiscoSalvarRegraGrupoItemRequest pParametro);

        [OperationContract]
        RiscoSalvarTravaExposicaoResponse SalvarTravaExposicaoSpider(RiscoSalvarTravaExposicaoRequest pParametro);

        [OperationContract]
        RiscoListarLimiteAlocadoResponse ConsultarRiscoLimitePorClienteSpider(RiscoListarLimiteAlocadoRequest pParametro);

        [OperationContract]
        RiscoListarLimiteBMFResponse ObterSpiderLimiteBMFCliente(RiscoListarLimiteBMFRequest pParametro);

        [OperationContract]
        RiscoInserirLimiteClienteBMFResponse AtualizarSpiderLimiteBMF(RiscoInserirLimiteClienteBMFRequest pParametro);

        [OperationContract]
        RiscoInserirLimiteBMFInstrumentoResponse AtualizarSpiderLimiteInstrumentoBMF(RiscoInserirLimiteBMFInstrumentoRequest pParametro);

        [OperationContract]
        RiscoRemoveLimiteBMFInstrumentoResponse RemoverSpiderLimiteInstrumentoBMF(RiscoRemoveLimiteBMFInstrumentoRequest pParametro);

        [OperationContract]
        RiscoListarRestricaoGlobalResponse ListarRestricaoGlobalCliente(RiscoListarRestricaoGlobalRequest pParametro);

        [OperationContract]
        RiscoListarRestricaoGrupoResponse ListarRestricaoGrupoCliente(RiscoListarRestricaoGrupoRequest pParametro);

        [OperationContract]
        RiscoListarRestricaoAtivoResponse ListarRestricaoAtivoCliente(RiscoListarRestricaoAtivoRequest pParametro);

        [OperationContract]
        RiscoSalvarRestricaoGlobalResponse SalvarRestricaoGlobalCliente(RiscoSalvarRestricaoGlobalRequest pParametro);

        [OperationContract]
        RiscoSalvarRestricaoGrupoResponse SalvarRestricaoGrupoCliente(RiscoSalvarRestricaoGrupoRequest pParametro);

        [OperationContract]
        RiscoSalvarRestricaoAtivoResponse SalvarRestricaoAtivoCliente(RiscoSalvarRestricaoAtivoRequest pParametro);

        [OperationContract]
        RiscoRemoveLimiteBMFResponse RemoverSpiderLimiteBMF(RiscoRemoveLimiteBMFRequest pParametro);
    }
}
