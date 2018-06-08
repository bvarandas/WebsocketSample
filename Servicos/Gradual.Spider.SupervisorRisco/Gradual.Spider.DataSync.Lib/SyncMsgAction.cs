using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.DataSync.Lib
{
    public enum SyncMsgAction
    {
        SNAPSHOT = 0,
        INSERT=1,
        UPDATE=2,
        DELETE=3,
        DELETE_ALL=4
    }
}
