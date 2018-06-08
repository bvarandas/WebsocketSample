using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Gradual.Spider.Ordem.Lib.Dados
{
    [Serializable]
    [ProtoContract]
    public class ExchangePrefixes
    {
        public static string BOVESPA = "BOVESPA";
        public static string BMF = "BMF";

    }
}
