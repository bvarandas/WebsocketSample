using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Gradual.Spider.PositionClient.Monitor.Lib.Dados
{
    [Flags]
    [DataContract]
    public enum OpcaoMarket
    {
        [EnumMember]
        TodosMercados = 1,
        [EnumMember]
        Avista        = 2,
        [EnumMember]
        Opcoes        = 4,
        [EnumMember]
        Futuros       = 8
    }

    [Flags]
    public enum OpcaoParametrosIntraday
    {
        [EnumMember]
        OfertasPedra        = 1,
        [EnumMember]
        NetIntradayNegativo = 2,
        [EnumMember]
        PLNegativo          = 4
    }

    [Flags]
    public enum OpcaoPL
    {
        [EnumMember]
        SomentePLnegativo   = 1,
        [EnumMember]
        SomenteComLucro     = 2
    }

    [Flags]
    public enum OpcaoSFPAtingido
    {
        [EnumMember]
        Ate25       = 1,
        [EnumMember]
        Entre25e50  = 2,
        [EnumMember]
        Entre50e75  = 4,
        [EnumMember]
        Acima75     = 8
    }

    [Flags]
    public enum OpcaoPrejuizoAtingido
    {
        [EnumMember]
        Ate2K           = 1,
        [EnumMember]
        Entre2Ke5K      = 2,
        [EnumMember]
        Entre5Ke10K     = 4,
        [EnumMember]
        Entre10Ke20K    = 8,
        [EnumMember]
        Entre20Ke50K    = 16,
        [EnumMember]
        Acima50K        = 32
    }
}
