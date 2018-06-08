using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.Acompanhamento4Socket.Lib.Dados
{
    public class MsgErrors
    {
        public const int OK = 0;
        public const int ERROR = -1;
        public const int ERR_ACCOUNT_NOT_FOUND = 1001;
        public const int ERR_COUNT_MAX_EXCEEDED = 1002;

        public static string MSG_OK = "Ok";
        public static string MSG_ERR = "Generic error";
        public static string MSG_ERR_ACCOUNT_NOT_FOUND = "Account / Id not found";
        public static string MSG_ERR_COUNT_MAX_EXCEEDED = "Maximum number of registries exceeded";



    }
}
