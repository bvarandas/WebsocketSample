using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Gradual.Spider.Acompanhamento4Socket.Lib.Dados
{
    public class TypeCompare
    {
        public const int UNDEFINED = -1;
        public const int EQUAL = 0;
        public const int GREATER = 1;
        public const int LOWER = 2;
        public const int GREATER_OR_EQUAL = 3;
        public const int LOWER_OR_EQUAL = 4;
        public const int BETWEEN = 5;
        public const int BETWEEN_EQUAL = 6;
        public const int GREATER_EQUAL_AND_LOWER = 7;
        public const int GREATER_AND_LOWER_EQUAL = 8;
        public const int LIST_VALUE = 9;
        public const int IN_STR = 20;
    }
}
