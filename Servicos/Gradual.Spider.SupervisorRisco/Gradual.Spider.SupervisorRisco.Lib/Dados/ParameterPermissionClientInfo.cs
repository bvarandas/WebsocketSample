using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gradual.Spider.SupervisorRisco.Lib.Dados;
using ProtoBuf;

namespace Gradual.Spider.SupervisorRisco.Lib.Dados
{
    [Serializable]
    [ProtoContract]
    public class ParameterPermissionClientInfo
    {
        [ProtoMember(1)]
        public string Especie { set; get; }

        [ProtoMember(2)]
        public RiscoParametrosEnum Parametro { set; get; }

        [ProtoMember(3)]
        public RiscoPermissoesEnum Permissao { set; get; }

        [ProtoMember(4, IsRequired=true)]
        public int IdBolsa { set; get; }

        [ProtoMember(5, IsRequired=true)]
        public int IdCliente { set; get; }

        [ProtoMember(6)]
        public string Descricao { set; get; }

        [ProtoMember(7)]
        public decimal ValorParametro { set; get; }

        [ProtoMember(8)]
        public decimal ValorAlocado { set; get; }

        [ProtoMember(9)]
        public Nullable<DateTime> DtValidade { set; get; }

        [ProtoMember(10)]
        public Nullable<DateTime> DtMovimento { set; get; }


        public ParameterPermissionClientInfo()
        {
            this.Especie = string.Empty;
            this.Parametro = RiscoParametrosEnum.Indefinido;
            this.Permissao = RiscoPermissoesEnum.Indefinido;
            this.IdBolsa = -1;
            this.IdCliente = -1;
            this.Descricao = string.Empty;
            this.ValorParametro = decimal.MinValue;
            this.ValorAlocado = decimal.MinValue;
            this.DtMovimento = null;
            this.DtValidade = null;
        }

    }

}
