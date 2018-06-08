using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gradual.Spider.PostTradingClientEngine.App_Codigo.TransporteJSon
{
    public class TransporteSessaoClienteLogado
    {
        #region Enum

        public enum EnumTipoCliente
        {
            Administrador,
            AnaliseGraficas,
            AnaliseFundamentalista,
            AnaliseEconomica,
            VisitanteAte30Dias,
            VisitanteExpirado,
            Cadastrado,
            CadastradoEExportado,
            Direct,
        }

        public enum EnumTipoPessoa
        {
            Fisica,
            Juridica
        }

        public enum PermissoesPertinentesAoSite
        {
            EditarCMS
          ,
            EditarAnaliseEconomica
                ,
            EditarAnaliseFundamentalista
                ,
            EditarAnaliseGrafica
                ,
            EditarCarteirasRecomendadas
                ,
            EditarNikkei
                , EditarGradiusGestao
        }

        public enum eStatusIPO
        {
            Solicitada = 1,
            Executada = 2,
            Cancelada = 3,
        }
        #endregion

        #region Propriedades

        public Nullable<int> IdCliente { get; set; }

        public int IdLogin { get; set; }

        public string Nome { get; set; }

        public string Email { get; set; }

        public string CpfCnpj { get; set; }

        public string DesejaAplicar { get; set; }

        private string _CodigoPrincipal;

        public string CodigoPrincipal
        {
            get
            {
                return _CodigoPrincipal;
            }

            set
            {
                if (string.IsNullOrEmpty(value) || value == "0")
                {
                    _CodigoPrincipal = "";
                }
                else
                {
                    _CodigoPrincipal = value;
                }
            }
        }

        public int CodigoBMF { get; set; }

        public string CodigoDaSessao { get; set; }

        public string AssessorPrincipal { get; set; }

        public EnumTipoCliente TipoAcesso { get; set; } // calc

        public Nullable<int> NumeroDiasAcesso { get; set; } // calc

        public Nullable<int> Passo { get; set; }

        public Nullable<DateTime> NascimentoFundacao { get; set; }

        public EnumTipoPessoa TipoPessoa { get; set; }

        public string Senha { get; set; }

        public bool ExpiracaoDeSenhaJaValidada { get; set; }

        public bool PrimeiroLoginJaVerificado { get; set; }

        public List<PermissoesPertinentesAoSite> Permissoes { get; set; }

        public string PlanoCalculadoraIR { get; set; }

        private string _PerfilSuitability = "n/d";

        public string PerfilSuitability
        {
            get
            {
                return _PerfilSuitability;
            }

            set
            {
                _PerfilSuitability = value;

                switch (_PerfilSuitability.ToLower())
                {
                    case "arrojado": _PerfilSuitability = "Arrojado"; break;
                    case "acessado": _PerfilSuitability = "Conservador"; break;
                    case "cadastronaofinalizado": _PerfilSuitability = "Conservador"; break;
                    case "medioriscocomrendavariavel": _PerfilSuitability = "Moderado"; break;
                    case "medio risco com renda variavel": _PerfilSuitability = "Moderado"; break;
                    case "medioriscosemrendavariavel": _PerfilSuitability = "Moderado"; break;
                    case "conservador": _PerfilSuitability = "Conservador"; break;
                    case "moderado": _PerfilSuitability = "Moderado"; break;
                    case "naoresponderagora": _PerfilSuitability = "Conservador"; break;
                    case "baixorisco": _PerfilSuitability = "Conservador"; break;
                    case "naoresponder": _PerfilSuitability = "Conservador"; break;
                }
            }
        }

        public string IdPerfilSuitability { get; set; }

        public bool JaPreencheuSuitability
        {
            get
            {
                return (!string.IsNullOrEmpty(this.PerfilSuitability) && this.PerfilSuitability != "n/d");
            }
        }

        public string NomeArquivoFichaCadastral { get; set; }
        public string NomeArquivoFichaCadastralCambio { get; set; }

        public DateTime DataDeUltimoLogin { get; set; }

        public Int32 CodigoTipoOperacaoCliente { get; set; }

        #endregion

        #region Construtores

        public TransporteSessaoClienteLogado()
        {
            this.Permissoes = new List<PermissoesPertinentesAoSite>();
            //this.DadosDoCarrinho = new TransporteDadosCarrinho();
        }

        #endregion

        #region Métodos Públicos

        public bool Pode(PermissoesPertinentesAoSite pPermissao)
        {
            return this.Permissoes.Contains(pPermissao);
        }
        #endregion
    }
}