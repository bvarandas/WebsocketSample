using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.SupervisorRisco.Lib.Handlers
{
    public enum EventAction
    {
        SNAPSHOT = 0,
        INSERT = 1,
        UPDATE = 2,
        DELETE = 3,
        DELETE_ALL = 4
    }
}
