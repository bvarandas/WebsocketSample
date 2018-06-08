using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Gradual.Spider.LimiteRestricao.Lib.Dados
{
    [DataContract]
    public class RiscoPermissaoAssociadaInfo : BaseInfo
    {
        #region Propriedades
        [DataMember]
        public int CodigoPermissaoRiscoAssociada { get; set; }

        [DataMember]
        public int CodigoCliente { get; set; }

        [DataMember]
        public RiscoGrupoInfo Grupo { get; set; }

        [DataMember]
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
