using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using System.ComponentModel;

namespace Gradual.Spider.SupervisorRisco.Lib.Dados
{
    [ProtoContract]
    [Serializable]
    [Flags]
    public enum RiscoParametrosEnum
    {
        Indefinido = 0,
        LimiteVendaAVista = 5,
        LimiteCompraAVista = 12,
        LimteCompraOpcoes = 13,
        LimiteVendaOpcoes = 7,
        LimiteMaximoBoleta = 8,
        PerdaMaximaAVista = 14,
        PerdaMaximaOpcoes=15
    }

    [ProtoContract]
    [Serializable]
    [Flags]
    public enum RiscoPermissoesEnum
    {
        Indefinido = 0,
        OperarMercadoAVista = 22,
        OperarMercadoOpcoes = 29,
        OperarMercadoFuturo = 30,
        BloquearEnvioOrdemOMS = 61,
        PerfilInstitucional = 62,
        ValidarExposicaoPatrimonial = 64,
        RoteamentoOrdensSpider = 65,
        ContaRepasse = 66,
        AberturaPosicao = 67
    }
    
    [ProtoContract]
    [Serializable]
    [Flags]
    public enum SegmentoMercadoEnum
    {
        AVISTA,
        TERMO,
        OPCAO,
        FUTURO,
        FRACIONARIO,
        INTEGRALFRACIONARIO,
        INDEFINIDO
    }

    [ProtoContract]
    [Serializable]
    public enum SentidoBloqueioEnum
    {
        Compra = 'C',
        Venda = 'V',
        Ambos = 'A'
    }
    
    [ProtoContract]
    [Serializable]
    [Flags]
    public enum TipoLimiteEnum
    {
        INDEFINIDO = 0,
        COMPRAAVISTA = 12,
        VENDAAVISTA = 5,
        COMPRAOPCOES = 13,
        VENDAOPCOES = 7,
        MAXIMOPORBOLETA=8,
        PERDAMAXIMAAVISTA=14,
        PERDAMAXIMAOPCOES=15
    }

    [ProtoContract]
    [Serializable]
    [Flags]
    public enum PositionTypeEnum
    {
        Abertura = 0,
        Intraday = 1
    }
}
