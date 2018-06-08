using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.Lib
{
    public enum eDateNull
    {
        Permite,
        DataMinValue
    }

    public enum eTipoAcesso
    {
        Cliente       = 0,
        Cadastro      = 1,
        Assessor      = 2,
        Atendimento   = 3,
        TeleMarketing = 4
    }
    public enum TipoAcaoUsuario
    {
        Consulta = 1,
        Edicao   = 2,
        Exclusao = 3,
        Inclusao = 4,
    }
    public enum eInformacao
    {
        Nacionalidade              = 1,
        EstadoCivil                = 2,
        TipoDocumento              = 3,
        OrgaoEmissor               = 4,
        ProfissaoPF                = 5,
        Banco                      = 6,
        Estado                     = 7,
        Pais                       = 8,
        Assessor                   = 9,
        SituacaoLegalRepresentante = 10,
        TipoCliente                = 11,
        AtividadePJ                = 12,
        TipoInvestidorPJ           = 13,
        TipoConta                  = 14,
        Escolaridade               = 15,
        AtividadePF                = 16,
        AtividadePFePJ             = 17,
        AssessorPadronizado        = 18,
    }
    public enum BolsaInfo 
    {
        TODAS = 0,
        BOVESPA = 1,
        BMF = 2
    }

    public enum eAcao
    {
        Salvar,
        Listar,
        Receber,
        Excluir
    }

    public enum eTipoLimite
    {
        Nenhum                 = 0,
        OperarCompraAVista     = 1,
        OperarCompraOpcao      = 2,
        OperarDescobertoAvista = 3,
        OperarDescobertoOpcao  = 4,
        ValorMaximoOrdem       = 5,
    }
}
