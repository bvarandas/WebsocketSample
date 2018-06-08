using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.SupervisorRisco.Lib.Dados
{
    public class TORestriction
    {
        public int TypeRest { get; set; }
        public object RestrictionObject { get; set; }

        public TORestriction()
        {
            this.TypeRest = TypeRestriction.UNDEFINED;
            this.RestrictionObject = null;
        }

    }
}
