using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace Gradual.Spider.Acompanhamento4Socket.Lib.Dados
{
    public class ConnInfo
    {
        public int IdClient { get; set; }
        public bool RestrictDetails { get; set; }
        public Socket ClientSocket { get; set; }
        public ConnInfo()
        {
            this.IdClient = 0;
            this.ClientSocket = null;
            this.RestrictDetails = true;
        }

    }
}
