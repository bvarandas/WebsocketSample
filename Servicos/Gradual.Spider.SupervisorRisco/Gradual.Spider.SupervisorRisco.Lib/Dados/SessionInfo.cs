using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace Gradual.Spider.SupervisorRisco.Lib.Dados
{
    public class SessionInfo
    {
        public bool IsSnapshotLoaded { get; set; }
        public int ClientNumber { get; set; }
        public ConcurrentQueue<object> Fila { get; set; }
        public Socket ConnSocket { get; set; }
        public SessionInfo()
        {
            this.IsSnapshotLoaded = false;
            this.ClientNumber = -1;
            this.ConnSocket = null;

        }

    }
}
