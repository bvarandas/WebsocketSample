using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace Gradual.Spider.Acompanhamento4Socket.Lib.Dados
{
    public class AcConnectionInfo
    {
        public AcConnectionInfo() { }
        public int ClientNumber { get; set; }
        public Socket ClientSocket { get; set; }
        public long LastSonda { get; set; }
        public bool Logged { get; set; }
        public bool RestrictDetails { get; set; }
    }
}
