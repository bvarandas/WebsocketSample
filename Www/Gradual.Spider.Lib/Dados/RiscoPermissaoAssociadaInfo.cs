using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.Lib.Dados
{
    public class RiscoPermissaoAssociadaInfo : BaseInfo
    {
        #region Propriedades
        public int CodigoPermissaoRiscoAssociada { get; set; }

        public int CodigoCliente { get; set; }

        public RiscoGrupoInfo Grupo { get; set; }

        public RiscoPermissaoInfo PermissaoRisco { get; set; }

        public override string ToString()
        {
            string lRetorno = "";

            lRetorno += " ; {[CodigoPermissaoRiscoAssociada] " + this.CodigoPermissaoRiscoAssociada.ToString();
            lRetorno += " ; [CodigoCliente] " + this.CodigoCliente.ToString();

            if (null != Grupo)
            {
                lRetorno += Grupo.ToString();
            }
            if (null != PermissaoRisco)
            {
                lRetorno += PermissaoRisco.ToString();
            }
            lRetorno += "}";


            return lRetorno;
        }
        #endregion

        #region Construtores
        public RiscoPermissaoAssociadaInfo() { }

        ~RiscoPermissaoAssociadaInfo() { }
        #endregion
    }
}
