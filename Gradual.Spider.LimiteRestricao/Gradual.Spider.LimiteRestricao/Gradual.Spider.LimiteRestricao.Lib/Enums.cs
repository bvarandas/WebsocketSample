using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.LimiteRestricao.Lib
{
    [Serializable]
    public enum eDateNull
    {
        Permite,
        DataMinValue
    }

    [Serializable]
    public enum eTipoAcesso
    {
        Cliente = 0,
        Cadastro = 1,
        Assessor = 2,
        Atendimento = 3,
        TeleMarketing = 4
    }

    [Serializable]
    public enum TipoAcaoUsuario
    {
        Consulta = 1,
        Edicao = 2,
        Exclusao = 3,
        Inclusao = 4,
    }

    [Serializable]
    public enum eInformacao
    {
        Nacionalidade = 1,
        EstadoCivil = 2,
        TipoDocumento = 3,
        OrgaoEmissor = 4,
        ProfissaoPF = 5,
        Banco = 6,
        Estado = 7,
        Pais = 8,
        Assessor = 9,
        SituacaoLegalRepresentante = 10,
        TipoCliente = 11,
        AtividadePJ = 12,
        TipoInvestidorPJ = 13,
        TipoConta = 14,
        Escolaridade = 15,
        AtividadePF = 16,
        AtividadePFePJ = 17,
        AssessorPadronizado = 18,
    }

    [Serializable]
    public enum BolsaInfo
    {
        TODAS = 0,
        BOVESPA = 1,
        BMF = 2
    }

    [Serializable]
    public enum eAcao
    {
        Salvar,
        Listar,
        Receber,
        Excluir
    }

    [Serializable]
    public enum eTipoLimite
    {
        Nenhum = 0,
        OperarCompraAVista = 1,
        OperarCompraOpcao = 2,
        OperarDescobertoAvista = 3,
        OperarDescobertoOpcao = 4,
        PerdaMaximaVista = 5,
        PerdaMaximaOpcao = 6,
        MaximoPorBoleta = 7
    }

    [Serializable]
    public enum eTipoParametroPlataforma
    {
        SessaoNormal = 1,
        SessaoHFT = 2
    }

    [Serializable]
    public enum eTipoTrader
    {
        Cliente = 1,
        Operador = 2,
        Assessor = 3,
        ContaMaster = 4
    }
}
