using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Data.SqlClient;
using System.Configuration;
using Gradual.Spider.SupervisorRisco.Lib.Dados;
using System.Data;
using System.Collections.Concurrent;

namespace Gradual.Spider.SupervisorRisco.DB.Lib
{
    public class DbRisco
    {
        #region log4net declaration
        public static readonly log4net.ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region Private Variables
        private SqlConnection _sqlConn;
        private SqlCommand _sqlCommand;
        string _strConnectionStringDefault;
        string _strConnectionStringMDS;
        string _strConnectionStringConfig;
        string _strConnectionStringMinicom;
        #endregion

        #region Properties
        public bool ConexaoAberta
        {
            get
            {
                return (_sqlConn != null && _sqlConn.State == System.Data.ConnectionState.Open);
            }
        }
        #endregion 
        // Constructor / Destructor
        public DbRisco()
        {
            _sqlConn = null;
            _sqlCommand = null;
            _strConnectionStringDefault = ConfigurationManager.ConnectionStrings["GradualSpiderRMS"].ConnectionString;
            _strConnectionStringMDS = ConfigurationManager.ConnectionStrings["GradualOMS"].ConnectionString;
            _strConnectionStringConfig = ConfigurationManager.ConnectionStrings["Config"].ConnectionString;
            _strConnectionStringMinicom = ConfigurationManager.ConnectionStrings["MINICOM"].ConnectionString;
        }

        ~DbRisco()
        {
            if (null != _sqlCommand)
            {
                _sqlCommand.Dispose();
                _sqlCommand = null;
            }
            if (null != _sqlConn)
            {
                _fecharConexao();
            }
        }


        #region Connection functions
        private void _abrirConexao()
        {
            this._abrirConexao(_strConnectionStringDefault);
        }

        private void _abrirConexao(string strConnectionString)
        {
            if (!this.ConexaoAberta)
            {
                _sqlConn = new SqlConnection(strConnectionString);
                _sqlConn.Open();
            }
        }
        
        private void _fecharConexao()
        {
            try
            {
                _sqlConn.Close();
                _sqlConn.Dispose();
            }
            catch { }
        }
        private SqlConnection _abrirConexaoSql(string strConnectionString)
        {
            SqlConnection sql=null;
            try
            {
                sql = new SqlConnection(strConnectionString);
                sql.Open();
                return sql;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na abertura da conexao: " + ex.Message, ex);
                throw;
            }
        }
        private void _fecharConexao(SqlConnection sql)
        {
            try
            {
                sql.Close();
                sql.Dispose();
                sql = null;
            }
            catch { }

        }

        #endregion


        #region Carregamentos de Limites
        public ConcurrentDictionary<string, SymbolInfo> CarregarCadastroPapel()
        {
            try
            {
                ConcurrentDictionary<string, SymbolInfo> ret = null;
                SqlDataAdapter lAdapter;
                _abrirConexao(_strConnectionStringMDS);
                DataSet lDataSet = new DataSet();
                _sqlCommand = new SqlCommand("prc_lmt_cadastropapel_sel", _sqlConn);
                int days;
                if (ConfigurationManager.AppSettings.AllKeys.Contains("DaysSecurityList"))
                {
                    days = Convert.ToInt32(ConfigurationManager.AppSettings["DaysSecurityList"].ToString());
                }
                else
                    days = 5;

                _sqlCommand.Parameters.Add(new SqlParameter("@DataRegistro", DateTime.Now.AddDays(days * (-1))));
                _sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                lAdapter = new SqlDataAdapter(_sqlCommand);
                lAdapter.Fill(lDataSet);
                if (lDataSet.Tables.Count > 0)
                {
                    if (lDataSet.Tables[0].Rows.Count != 0)
                        ret = new ConcurrentDictionary<string, SymbolInfo>();
                    foreach (DataRow lRow in lDataSet.Tables[0].Rows)
                    {
                        SymbolInfo item = new SymbolInfo();
                        item.Instrumento = lRow["codigoInstrumento"].DBToString().ToUpper();
                        item.FormaCotacao = lRow["FormaCotacao"].DBToInt32();
                        item.LotePadrao = lRow["LotePadrao"].DBToInt32();
                        item.SegmentoMercadoValor = lRow["SegmentoMercado"].DBToString();
                        switch (lRow["SegmentoMercado"].DBToString())
                        {
                            case "04":
                                item.SegmentoMercado = SegmentoMercadoEnum.OPCAO;
                                break;
                            case "09":
                                item.SegmentoMercado = SegmentoMercadoEnum.OPCAO;
                                break;
                            case "01":
                                item.SegmentoMercado = SegmentoMercadoEnum.AVISTA;
                                break;
                            case "03":
                                item.SegmentoMercado = SegmentoMercadoEnum.FRACIONARIO;
                                break;
                            case "FUT":
                                break;
                        }

                        item.DtNegocio = lRow["dt_negocio"].DBToDateTime();
                        item.DtAtualizacao = lRow["dt_atualizacao"].DBToDateTime();
                        item.VlrUltima = lRow["vl_ultima"].DBToDecimal();
                        item.VlrOscilacao = lRow["vl_oscilacao"].DBToDecimal();
                        item.VlrFechamento = lRow["vl_fechamento"].DBToDecimal();
                        item.VlrAjuste = lRow["vl_ajuste"].DBToDecimal();
                        item.GrupoCotacao = lRow["GrupoCotacao"].DBToString();
                        item.CoeficienteMultiplicacao = lRow["CoeficienteMultiplicacao"].DBToDecimal();
                        item.IndicadorOpcao = lRow["IndicadorOpcao"].DBToString();
                        item.DtVencimento = lRow["DataVencimento"].DBToDateTime();
                        item.DescAtivo = lRow["RazaoSocial"].DBToString();
                        item.CodigoISIN = lRow["CodigoISIN"].DBToString();
                        item.CodigoPapelObjeto = lRow["CodigoPapelObjeto"].DBToString();
                        ret.AddOrUpdate(item.Instrumento, item, (key, oldValue) => item);
                    }
                }
                _fecharConexao();
                lAdapter.Dispose();
                lAdapter = null;
                lDataSet.Dispose();
                lDataSet = null;
                _sqlCommand = null;
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error("Erro no carregamento do cadastro de papel: " + ex.Message, ex);
                return null;
            }
        }

        public Dictionary<int, int> CarregarAccountBvspBmf()
        {
            try
            {
                Dictionary<int, int> ret = null;
                SqlDataAdapter lAdapter;
                _abrirConexao(_strConnectionStringDefault);
                DataSet lDataSet = new DataSet();
                _sqlCommand = new SqlCommand("prc_fix_client_bvsp_bmf_lst", _sqlConn);
                _sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                lAdapter = new SqlDataAdapter(_sqlCommand);
                lAdapter.Fill(lDataSet);
                if (lDataSet.Tables.Count > 0)
                {
                    if (lDataSet.Tables[0].Rows.Count != 0)
                        ret = new Dictionary<int, int>();
                    foreach (DataRow lRow in lDataSet.Tables[0].Rows)
                    {
                        int accbvsp = lRow["AccountBvsp"].DBToInt32();
                        int accbmf = lRow["AccountBmf"].DBToInt32();
                        ret.Add(accbvsp, accbmf);
                    }
                }
                _fecharConexao();
                lAdapter.Dispose();
                lAdapter = null;
                lDataSet.Dispose();
                lDataSet = null;
                _sqlCommand = null;
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error("Erro no carregamento do cadastro de paridades de conta bovespa/bmf: " + ex.Message, ex);
                return null;
            }
        }

        public List<int> CarregarContasSpider() // Antigo ObterAccountHFT
        {
            try
            {
                List<int> ret = null;
                SqlDataAdapter lAdapter;
                _abrirConexao(_strConnectionStringDefault);
                DataSet lDataSet = new DataSet();
                _sqlCommand = new SqlCommand("prc_lmt_obter_relacao_clientes_hft", _sqlConn);
                _sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                lAdapter = new SqlDataAdapter(_sqlCommand);
                lAdapter.Fill(lDataSet);
                if (lDataSet.Tables.Count > 0)
                {
                    if (lDataSet.Tables[0].Rows.Count > 0)
                        ret = new List<int>();

                    foreach (DataRow lRow in lDataSet.Tables[0].Rows)
                    {
                        ret.Add(lRow["id_cliente"].DBToInt32());
                    }
                }
                _fecharConexao();
                lAdapter.Dispose();
                lAdapter = null;
                lDataSet.Dispose();
                lDataSet = null;
                _sqlCommand = null;
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error("Erro ao obter a lista de clientes: " + ex.Message, ex);
                return null;
            }
        }

        public ClientParameterPermissionInfo CarregarPermissoesParametros(int account)
        {
            try
            {
                ClientParameterPermissionInfo ret = null;
                SqlDataAdapter lAdapter;
                _abrirConexao(_strConnectionStringDefault);
                DataSet lDataSet = new DataSet();
                _sqlCommand = new SqlCommand("prc_lmt_sel_regras_cliente_oms", _sqlConn);
                _sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                _sqlCommand.Parameters.Add(new SqlParameter("@id_cliente", account));
                lAdapter = new SqlDataAdapter(_sqlCommand);
                lAdapter.Fill(lDataSet);
                if (lDataSet.Tables.Count > 0)
                {
                    if (lDataSet.Tables[0].Rows.Count > 0)
                        ret = new ClientParameterPermissionInfo();
                    foreach (DataRow lRow in lDataSet.Tables[0].Rows)
                    {
                        ParameterPermissionClientInfo itemPermissao = new ParameterPermissionClientInfo();
                        ret.IdCliente = account;
                        itemPermissao.Especie = lRow["Especie"].DBToString();
                        itemPermissao.IdCliente = lRow["id_cliente"].DBToInt32();
                        itemPermissao.IdBolsa = lRow["id_bolsa"].DBToInt32();
                        itemPermissao.Descricao = lRow["ParametroPermissao"].DBToString();
                        itemPermissao.ValorParametro = lRow["valor"].DBToDecimal();
                        itemPermissao.ValorAlocado = lRow["vl_alocado"].DBToDecimal();
                        itemPermissao.DtValidade = lRow["dt_validade"].DBToDateTime();
                        itemPermissao.DtMovimento = lRow["dt_movimento"].DBToDateTime();
                        // Permissao
                        if (lRow["Especie"].ToString().Equals("Permissao"))
                        {
                            itemPermissao.Permissao = (RiscoPermissoesEnum)(lRow["idParametroPermissao"].DBToInt32());
                            itemPermissao.Parametro = RiscoParametrosEnum.Indefinido;
                            ret.Permissoes.Add(itemPermissao);
                        }
                        else
                        {
                            itemPermissao.Parametro = (RiscoParametrosEnum)(lRow["idParametroPermissao"].DBToInt32());
                            itemPermissao.Permissao = RiscoPermissoesEnum.Indefinido;
                            ret.Parametros.Add(itemPermissao);
                        }
                    }
                }
                _fecharConexao();
                lAdapter.Dispose();
                lAdapter = null;
                lDataSet.Dispose();
                lDataSet = null;
                _sqlCommand = null;
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error("Erro ao obter as permissoes e parametros de clientes: " + ex.Message, ex);
                return null;
            }
        }

        //public FatFingerInfo CarregarParametrosFatFinger(int account)
        //{
        //    try
        //    {
        //        FatFingerInfo ret = null;
        //        SqlDataAdapter lAdapter;
        //        _abrirConexao(_strConnectionStringDefault);
        //        DataSet lDataSet = new DataSet();
        //        _sqlCommand = new SqlCommand("prc_lmt_sel_fatfinger", _sqlConn);
        //        _sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
        //        _sqlCommand.Parameters.Add(new SqlParameter("@id_cliente", account));
        //        lAdapter = new SqlDataAdapter(_sqlCommand);
        //        lAdapter.Fill(lDataSet);
        //        if (lDataSet.Tables.Count > 0)
        //        {
        //            foreach (DataRow lRow in lDataSet.Tables[0].Rows)
        //            {
        //                ret = new FatFingerInfo();
        //                ret.Account = account;
        //                ret.Mercado = "BOVESPA";
        //                ret.ValorRegra = lRow["vl_maximo"].DBToDecimal();
        //                ret.DtValidadeRegra = lRow["dt_vencimento"].DBToDateTime();
        //            }
        //        }
        //        _fecharConexao();
        //        lAdapter.Dispose();
        //        lDataSet.Dispose();
        //        _sqlCommand.Dispose();
        //        return ret;
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error("Erro ao obter os parametros de fat-finger de clientes: " + ex.Message, ex);
        //        return null;
        //    }
        //}

        public RiskExposureClientInfo CarregarExposicaoRiscoCliente(int account, DateTime dtMovimento)
        {
            try
            {
                RiskExposureClientInfo ret = null;
                SqlDataAdapter lAdapter;
                _abrirConexao(_strConnectionStringDefault);
                DataSet lDataSet = new DataSet();
                _sqlCommand = new SqlCommand("prc_lmt_sel_exposicao_risco", _sqlConn);
                _sqlCommand.Parameters.Add(new SqlParameter("@idCliente", account));
                _sqlCommand.Parameters.Add(new SqlParameter("@dtMovimento", account));
                _sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                lAdapter = new SqlDataAdapter(_sqlCommand);
                lAdapter.Fill(lDataSet);
                if (lDataSet.Tables.Count > 0)
                {
                    foreach (DataRow lRow in lDataSet.Tables[0].Rows)
                    {
                        ret = new RiskExposureClientInfo();
                        ret.IdCliente = lRow["id_cliente"].DBToInt32();
                        ret.LucroPrejuizo = lRow["vl_lucro_prejuizo"].DBToDecimal();
                        ret.PatrimonioLiquido = lRow["vl_patrimonio_liquido"].DBToDecimal();
                        ret.DataAtualizacao = lRow["dt_atualizacao"].DBToDateTime();

                        if (ret.LucroPrejuizo >= 0)
                        {
                            ret.LucroPrejuizo = 0;
                        }
                        else
                        {
                            ret.LucroPrejuizo = ret.LucroPrejuizo * -1;
                        }
                    }
                }
                _fecharConexao();
                lAdapter.Dispose();
                lDataSet.Dispose();
                _sqlCommand.Dispose();
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error("Erro ao obter parametros de exposicao do cliente: " + ex.Message, ex);
                return null;
            }
        }

        public List<BlockedInstrumentInfo> CarregarPermissaoAtivo()
        {
            try
            {
                List<BlockedInstrumentInfo> ret = null;
                SqlDataAdapter lAdapter;
                _abrirConexao(_strConnectionStringDefault);
                DataSet lDataSet = new DataSet();
                _sqlCommand = new SqlCommand("prc_lmt_verificar_permissao_ativo", _sqlConn);
                _sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                lAdapter = new SqlDataAdapter(_sqlCommand);
                lAdapter.Fill(lDataSet);
                if (lDataSet.Tables.Count > 0)
                {

                    if (lDataSet.Tables[0].Rows.Count > 0)
                        ret = new List<BlockedInstrumentInfo>();
                    foreach (DataRow lRow in lDataSet.Tables[0].Rows)
                    {
                        BlockedInstrumentInfo item = new BlockedInstrumentInfo();

                        char sentido = lRow["sentido"].DBToString()[0];
                        switch (sentido)
                        {
                            case 'V':
                                item.Sentido = SentidoBloqueioEnum.Venda; break;
                            case 'C':
                                item.Sentido = SentidoBloqueioEnum.Compra; break;
                            default:
                                item.Sentido = SentidoBloqueioEnum.Ambos; break;
                        }
                        item.Instrumento = lRow["ds_item"].DBToString();
                        ret.Add(item);
                    }
                }
                _fecharConexao();
                lAdapter.Dispose();
                lDataSet.Dispose();
                _sqlCommand.Dispose();
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error("Erro ao obter permissao global dos instrumentos: " + ex.Message, ex);
                return null;
            }
        }

        public List<BlockedInstrumentInfo> CarregarPermissaoAtivoGrupoCliente(int account)
        {
            try
            {
                List<BlockedInstrumentInfo> ret = null;
                SqlDataAdapter lAdapter;
                _abrirConexao(_strConnectionStringDefault);
                DataSet lDataSet = new DataSet();
                _sqlCommand = new SqlCommand("prc_lmt_sel_cliente_bloq_grupo", _sqlConn);
                _sqlCommand.Parameters.Add(new SqlParameter("@id_cliente", account));
                _sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                lAdapter = new SqlDataAdapter(_sqlCommand);
                lAdapter.Fill(lDataSet);
                if (lDataSet.Tables.Count > 0)
                {
                    ret = new List<BlockedInstrumentInfo>();
                    foreach (DataRow lRow in lDataSet.Tables[0].Rows)
                    {
                        BlockedInstrumentInfo item = new BlockedInstrumentInfo();

                        char sentido = lRow["direcao"].DBToString()[0];
                        switch (sentido)
                        {
                            case 'V':
                                item.Sentido = SentidoBloqueioEnum.Venda; break;
                            case 'C':
                                item.Sentido = SentidoBloqueioEnum.Compra; break;
                            default:
                                item.Sentido = SentidoBloqueioEnum.Ambos; break;
                        }
                        item.Instrumento = lRow["ds_item"].DBToString();
                        ret.Add(item);
                    }
                }
                _fecharConexao();
                lAdapter.Dispose();
                lDataSet.Dispose();
                _sqlCommand.Dispose();
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error("Erro ao obter permissao de grupo dos instrumentos: " + ex.Message, ex);
                return null;
            }
        }

        public List<BlockedInstrumentInfo> CarregarPermissaoAtivoCliente(int account)
        {
            try
            {
                List<BlockedInstrumentInfo> ret = null;
                SqlDataAdapter lAdapter;
                _abrirConexao(_strConnectionStringDefault);
                DataSet lDataSet = new DataSet();
                _sqlCommand = new SqlCommand("prc_lmt_sel_cliente_bloqueio_instrumento", _sqlConn);
                _sqlCommand.Parameters.Add(new SqlParameter("@id_cliente", account));
                _sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                lAdapter = new SqlDataAdapter(_sqlCommand);
                lAdapter.Fill(lDataSet);
                if (lDataSet.Tables.Count > 0)
                {
                    ret = new List<BlockedInstrumentInfo>();
                    foreach (DataRow lRow in lDataSet.Tables[0].Rows)
                    {
                        BlockedInstrumentInfo item = new BlockedInstrumentInfo();
                        char sentido = lRow["Direcao"].DBToString()[0];
                        switch (sentido)
                        {
                            case 'V':
                                item.Sentido = SentidoBloqueioEnum.Venda; break;
                            case 'C':
                                item.Sentido = SentidoBloqueioEnum.Compra; break;
                            default:
                                item.Sentido = SentidoBloqueioEnum.Ambos; break;
                        }
                        item.Instrumento = lRow["cd_ativo"].DBToString();
                        item.IdCliente = lRow["id_cliente"].DBToInt32();
                        ret.Add(item);
                    }
                }
                _fecharConexao();
                lAdapter.Dispose();
                lDataSet.Dispose();
                _sqlCommand.Dispose();
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error("Erro ao obter parametros de instrumentos do clientes: " + ex.Message, ex);
                return null;
            }
        }

        public List<OperatingLimitInfo> CarregarLimiteOperacional(int account)
        {
            try
            {
                List<OperatingLimitInfo> ret = null;
                SqlDataAdapter lAdapter;
                _abrirConexao(_strConnectionStringDefault);
                DataSet lDataSet = new DataSet();
                _sqlCommand = new SqlCommand("prc_lmt_obter_relacao_limites", _sqlConn);
                _sqlCommand.Parameters.Add(new SqlParameter("@id_cliente", account));
                _sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                lAdapter = new SqlDataAdapter(_sqlCommand);
                lAdapter.Fill(lDataSet);
                if (lDataSet.Tables.Count > 0)
                {
                    if (lDataSet.Tables[0].Rows.Count > 0)
                        ret = new List<OperatingLimitInfo>();
                    foreach (DataRow lRow in lDataSet.Tables[0].Rows)
                    {
                        OperatingLimitInfo item = new OperatingLimitInfo();
                        item.CodigoCliente = account;
                        item.CodigoParametroCliente = lRow["id_cliente_parametro"].DBToInt32();
                        item.DataValidade = lRow["dt_validade"].DBToDateTime();
                        item.TipoLimite = (TipoLimiteEnum)(lRow["id_parametro"].DBToInt32());
                        item.ValotTotal = lRow["vl_parametro"].DBToDecimal();
                        item.ValorAlocado = lRow["vl_alocado"].DBToDecimal();
                        item.ValorDisponivel = (item.ValotTotal - item.ValorAlocado);
                        ret.Add(item);
                    }
                }
                _fecharConexao();
                lAdapter.Dispose();
                lDataSet.Dispose();
                _sqlCommand.Dispose();
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error("Erro ao obter limite operacional do cliente: " + ex.Message, ex);
                return null;
            }
        }

        public ClientLimitBMFInfo CarregarLimitesBMF(int account)
        {
            try
            {
                ClientLimitBMFInfo ret = null;

                SqlDataAdapter lAdapter;
                _abrirConexao(_strConnectionStringDefault);
                DataSet lDataSet = new DataSet();
                _sqlCommand = new SqlCommand("prc_lmt_sel_limites_bmf", _sqlConn);
                _sqlCommand.Parameters.Add(new SqlParameter("@account", account));
                _sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                lAdapter = new SqlDataAdapter(_sqlCommand);
                lAdapter.Fill(lDataSet);
                if (lDataSet.Tables.Count > 0)
                {
                    if (lDataSet.Tables[0].Rows.Count > 0 || lDataSet.Tables[0].Rows.Count > 0)
                        ret = new ClientLimitBMFInfo();

                    foreach (DataRow lRow in lDataSet.Tables[0].Rows)
                    {
                        ClientLimitContractBMFInfo ret1 = new ClientLimitContractBMFInfo();
                        ret1.Account = (lRow["account"]).DBToInt32();
                        ret1.IdClienteParametroBMF = (lRow["idClienteParametroBMF"]).DBToInt32();
                        ret1.Contrato = (lRow["Contrato"]).DBToString();
                        string sentido = lRow["sentido"].DBToString();
                        ret1.Sentido = string.IsNullOrEmpty(sentido) ? string.Empty : sentido;
                        ret1.QuantidadeTotal = (lRow["qtTotal"]).DBToInt32();
                        ret1.QuantidadeDisponivel = (lRow["qtDisponivel"]).DBToInt32();
                        ret1.DataValidade = (lRow["dtValidade"]).DBToDateTime();
                        ret1.DataMovimento = (lRow["dtMovimento"]).DBToDateTime();
                        ret1.QuantidadeMaximaOferta = (lRow["qtMaxOferta"]).DBToInt32();
                        ret.Account = ret1.Account;
                        ret.ContractLimit.Add(ret1);
                    }
                    foreach (DataRow lRow in lDataSet.Tables[1].Rows)
                    {
                        ClientLimitInstrumentBMFInfo ret2 = new ClientLimitInstrumentBMFInfo();
                        ret2.Account = ret.Account;
                        ret2.IdClienteParametroBMF = (lRow["IdClienteParametroBMF"]).DBToInt32();
                        ret2.IdClienteParametroInstrumento = (lRow["IdClienteParametroInstrumento"]).DBToInt32();
                        ret2.Instrumento = (lRow["Instrumento"]).DBToString();
                        ret2.dtMovimento = (lRow["dtMovimento"]).DBToDateTime();
                        ret2.QtTotalContratoPai = (lRow["QtTotalContratoPai"]).DBToInt32();
                        ret2.QtTotalInstrumento = (lRow["QtTotalInstrumento"]).DBToInt32();
                        ret2.QtDisponivel = (lRow["QtDisponivel"]).DBToInt32();
                        ret2.ContratoBase = (lRow["contrato"]).DBToString();
                        ret2.QuantidadeMaximaOferta = (lRow["qtMaxOferta"]).DBToInt32();
                        string sentido = lRow["sentido"].DBToString();
                        ret2.Sentido = string.IsNullOrEmpty(sentido) ? string.Empty : sentido;
                        ret.InstrumentLimit.Add(ret2);

                    }
                }
                _fecharConexao();
                lAdapter.Dispose();
                lDataSet.Dispose();
                _sqlCommand.Dispose();
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error("Erro ao obter limite de bmf do cliente: " + ex.Message, ex);
                return null;
            }
        }

        public Dictionary<int, FixLimitInfo> CarregarLimiteFix()
        {
            try
            {
                Dictionary<int, FixLimitInfo> ret = null;
                SqlDataAdapter lAdapter;
                _abrirConexao(_strConnectionStringDefault);
                DataSet lDataSet = new DataSet();
                _sqlCommand = new SqlCommand("prc_lmt_sel_fix_session_limits", _sqlConn);
                _sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                lAdapter = new SqlDataAdapter(_sqlCommand);
                lAdapter.Fill(lDataSet);
                if (lDataSet.Tables.Count > 0)
                {
                    if (lDataSet.Tables[0].Rows.Count > 0)
                        ret = new Dictionary<int, FixLimitInfo>();
                    foreach (DataRow lRow in lDataSet.Tables[0].Rows)
                    {
                        FixLimitInfo aux = new FixLimitInfo();
                        aux.IdSessaoFix = lRow["IdSessaoFix"].DBToInt32();
                        aux.Descricao = lRow["Descricao"].DBToString();
                        aux.VlDisponivel = lRow["VlDisponivel"].DBToDecimal();
                        aux.VlTotal = lRow["VlTotal"].DBToDecimal();
                        ret.Add(aux.IdSessaoFix, aux);
                    }
                }
                _fecharConexao();
                lAdapter.Dispose();
                lDataSet.Dispose();
                _sqlCommand.Dispose();
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error("Erro ao obter Limites de Sessoes Fix: " + ex.Message, ex);
                return null;
            }
        }

        public ConcurrentDictionary<string, OptionBlockInfo> CarregarVencimentoSerieOpcao()
        {
            try
            {
                ConcurrentDictionary<string, OptionBlockInfo> ret = null;
                SqlDataAdapter lAdapter;
                _abrirConexao(_strConnectionStringDefault);
                DataSet lDataSet = new DataSet();
                _sqlCommand = new SqlCommand("prc_lmt_bloqueio_opcao", _sqlConn);
                _sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                lAdapter = new SqlDataAdapter(_sqlCommand);
                lAdapter.Fill(lDataSet);
                if (lDataSet.Tables.Count > 0)
                {
                    if (lDataSet.Tables[0].Rows.Count > 0)
                        ret = new ConcurrentDictionary<string, OptionBlockInfo>();
                    foreach (DataRow lRow in lDataSet.Tables[0].Rows)
                    {
                        OptionBlockInfo aux = new OptionBlockInfo();
                        aux.IdMovimento = lRow["idMovimento"].DBToInt32();
                        aux.Serie = lRow["ds_serie"].DBToString();
                        aux.DtBloqueio = lRow["dt_bloqueio"].DBToDateTime();
                        ret.AddOrUpdate(aux.Serie, aux, (key, oldValue) => aux);
                    }
                }
                _fecharConexao();
                lAdapter.Dispose();
                lDataSet.Dispose();
                _sqlCommand.Dispose();
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error("Erro ao obter Series de Opcao Bloqueadas: " + ex.Message, ex);
                return null;
            }
        }

        public Dictionary<int, List<ContaBrokerInfo>> CarregarContaBroker()
        {
            try
            {
                Dictionary<int, List<ContaBrokerInfo>> ret = null;
                SqlDataAdapter lAdapter;
                _abrirConexao(_strConnectionStringDefault);
                DataSet lDataSet = new DataSet();
                _sqlCommand = new SqlCommand("prc_lmt_sel_conta_broker", _sqlConn);
                _sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                lAdapter = new SqlDataAdapter(_sqlCommand);
                lAdapter.Fill(lDataSet);
                
                if (lDataSet.Tables[0].Rows.Count > 0)
                    ret = new Dictionary<int, List<ContaBrokerInfo>>();
                foreach (DataRow lRow in lDataSet.Tables[0].Rows)
                {
                    
                    List<ContaBrokerInfo> lst = null;
                    if (ret.TryGetValue(lRow["idCliente"].DBToInt32(), out lst))
                    {
                        ContaBrokerInfo item = new ContaBrokerInfo();
                        item.IdContaBroker = lRow["idContaBroker"].DBToInt32();
                        item.IdCliente = lRow["idCliente"].DBToInt32();
                        item.DescCliente = lRow["dsCliente"].DBToString();
                        item.Ativo = lRow["stAtivo"].DBToString().Equals("S") ? true : false;
                        lst.Add(item);
                    }
                    else
                    {
                        List<ContaBrokerInfo> aux = new List<ContaBrokerInfo>();
                        ContaBrokerInfo item = new ContaBrokerInfo();
                        item.IdContaBroker = lRow["idContaBroker"].DBToInt32();
                        item.IdCliente = lRow["idCliente"].DBToInt32();
                        item.DescCliente = lRow["dsCliente"].DBToString();
                        item.Ativo = lRow["stAtivo"].DBToString().Equals("S") ? true : false;
                        aux.Add(item);
                        ret.Add(item.IdCliente, aux);
                    }
                }
                
                _fecharConexao();
                lAdapter.Dispose();
                lDataSet.Dispose();
                _sqlCommand.Dispose();
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error("Erro ao obter lista de ContaBrokers: " + ex.Message, ex);
                return null;
            }
        }

        public ConcurrentDictionary<int, RestrictionGlobalInfo> CarregarRestrictionGlobal()
        {
            try
            {
                ConcurrentDictionary<int, RestrictionGlobalInfo> ret = new ConcurrentDictionary<int, RestrictionGlobalInfo>();
                SqlDataAdapter lAdapter;
                _abrirConexao(_strConnectionStringDefault);
                DataSet lDataSet = new DataSet();
                _sqlCommand = new SqlCommand("prc_lmt_restriction_global_sel", _sqlConn);
                _sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                lAdapter = new SqlDataAdapter(_sqlCommand);
                lAdapter.Fill(lDataSet);

                if (lDataSet.Tables[0].Rows.Count > 0)
                    ret = new ConcurrentDictionary<int, RestrictionGlobalInfo>();
                foreach (DataRow lRow in lDataSet.Tables[0].Rows)
                {
                    RestrictionGlobalInfo item = new RestrictionGlobalInfo();
                    item.Account = lRow["Account"].DBToInt32();
                    item.LimiteVolumeNet = lRow["LimiteVolumeNet"].DBToDecimal();
                    item.QuantidadeNet = lRow["QuantidadeNet"].DBToDecimal();
                    item.LimiteMaxOfertaVolume = lRow["LimiteMaxOfertaVolume"].DBToDecimal();
                    item.LimiteMaxOfertaQtde = lRow["LimiteMaxOfertaQtde"].DBToDecimal();
                    item.DtAtualizacao = lRow["dtAtualizacao"].DBToDateTime();
                    item.VolumeNetAlocado = lRow["VolumeNetAlocado"].DBToDecimal();
                    item.QuantidadeNetAlocada = lRow["QuantidadeNetAlocada"].DBToDecimal();
                    if (lRow["StAtivo"].DBToString().Equals("S", StringComparison.CurrentCultureIgnoreCase))
                        item.StAtivo = true;
                    else
                        item.StAtivo = false;
                    ret.AddOrUpdate(item.Account, item, (key, oldValue) => item);
                }
                _fecharConexao();
                lAdapter.Dispose();
                lDataSet.Dispose();
                _sqlCommand.Dispose();
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error("Erro ao obter lista de restricoes globais: " + ex.Message, ex);
                return null;
            }
        }

        public ConcurrentDictionary<string, RestrictionSymbolInfo> CarregarRestrictionSymbol()
        {
            try
            {
                ConcurrentDictionary<string, RestrictionSymbolInfo> ret = new ConcurrentDictionary<string, RestrictionSymbolInfo>();
                SqlDataAdapter lAdapter;
                _abrirConexao(_strConnectionStringDefault);
                DataSet lDataSet = new DataSet();
                _sqlCommand = new SqlCommand("prc_lmt_restriction_symbol_sel", _sqlConn);
                _sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                lAdapter = new SqlDataAdapter(_sqlCommand);
                lAdapter.Fill(lDataSet);

                if (lDataSet.Tables[0].Rows.Count > 0)
                    ret = new ConcurrentDictionary<string, RestrictionSymbolInfo>();
                foreach (DataRow lRow in lDataSet.Tables[0].Rows)
                {
                    RestrictionSymbolInfo item = new RestrictionSymbolInfo();
                    item.Account = lRow["Account"].DBToInt32();
                    item.Symbol = lRow["Symbol"].DBToString();
                    item.LimiteVolumeNet = lRow["LimiteVolumeNet"].DBToDecimal();
                    item.QuantidadeNet = lRow["QuantidadeNet"].DBToDecimal();
                    item.LimiteMaxOfertaVolume = lRow["LimiteMaxOfertaVolume"].DBToDecimal();
                    item.LimiteMaxOfertaQtde = lRow["LimiteMaxOfertaQtde"].DBToDecimal();
                    item.DtAtualizacao = lRow["dtAtualizacao"].DBToDateTime();
                    item.VolumeNetAlocado = lRow["VolumeNetAlocado"].DBToDecimal();
                    item.QuantidadeNetAlocada = lRow["QuantidadeNetAlocada"].DBToDecimal();
                    if (lRow["StAtivo"].DBToString().Equals("S", StringComparison.CurrentCultureIgnoreCase))
                        item.StAtivo = true;
                    else
                        item.StAtivo = false;
                    ret.AddOrUpdate(item.Symbol, item, (key, oldValue) => item);
                }
                _fecharConexao();
                lAdapter.Dispose();
                lDataSet.Dispose();
                _sqlCommand.Dispose();
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error("Erro ao obter restricoes por simbolo: " + ex.Message, ex);
                return null;
            }
        }

        public ConcurrentDictionary<string, RestrictionGroupSymbolInfo> CarregarRestrictionGroupSymbol()
        {
            try
            {
                ConcurrentDictionary<string, RestrictionGroupSymbolInfo> ret = new ConcurrentDictionary<string, RestrictionGroupSymbolInfo>();
                SqlDataAdapter lAdapter;
                _abrirConexao(_strConnectionStringDefault);
                DataSet lDataSet = new DataSet();
                _sqlCommand = new SqlCommand("prc_lmt_restriction_group_symbol_sel", _sqlConn);
                _sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                lAdapter = new SqlDataAdapter(_sqlCommand);
                lAdapter.Fill(lDataSet);

                if (lDataSet.Tables[0].Rows.Count > 0)
                    ret = new ConcurrentDictionary<string, RestrictionGroupSymbolInfo>();
                foreach (DataRow lRow in lDataSet.Tables[0].Rows)
                {
                    RestrictionGroupSymbolInfo item = new RestrictionGroupSymbolInfo();
                    item.Account = lRow["Account"].DBToInt32();
                    item.LimiteVolumeNet = lRow["LimiteVolumeNet"].DBToDecimal();
                    item.QuantidadeNet = lRow["QuantidadeNet"].DBToDecimal();
                    item.LimiteMaxOfertaVolume = lRow["LimiteMaxOfertaVolume"].DBToDecimal();
                    item.LimiteMaxOfertaQtde = lRow["LimiteMaxOfertaQtde"].DBToDecimal();
                    item.DtAtualizacao = lRow["dtAtualizacao"].DBToDateTime();
                    item.IdGrupo = lRow["IdGrupo"].DBToString();
                    item.VolumeNetAlocado = lRow["VolumeNetAlocado"].DBToDecimal();
                    item.QuantidadeNetAlocada = lRow["QuantidadeNetAlocada"].DBToDecimal();
                    if (lRow["StAtivo"].DBToString().Equals("S", StringComparison.CurrentCultureIgnoreCase))
                        item.StAtivo = true;
                    else
                        item.StAtivo = false;
                    ret.AddOrUpdate(item.IdGrupo, item, (key, oldValue) => item);
                }
                _fecharConexao();
                lAdapter.Dispose();
                lDataSet.Dispose();
                _sqlCommand.Dispose();
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error("Erro ao obter restricoes por simbolo: " + ex.Message, ex);
                return null;
            }
        }


        public bool InserirMvtoBvsp(OperatingLimitInfo opLimit)
        {
            try
            {
                _abrirConexao(_strConnectionStringDefault);
                _sqlCommand = new SqlCommand("prc_lmt_insere_cliente_parametro_valor", _sqlConn);
                _sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                _sqlCommand.Parameters.Add(new SqlParameter("@id_cliente_parametro", opLimit.CodigoParametroCliente));
                _sqlCommand.Parameters.Add(new SqlParameter("@ValorMovimento", opLimit.ValorMovimento));
                _sqlCommand.Parameters.Add(new SqlParameter("@ValorAlocado", opLimit.ValorAlocado));
                _sqlCommand.Parameters.Add(new SqlParameter("@ValorDisponivel", opLimit.ValorDisponivel));
                _sqlCommand.Parameters.Add(new SqlParameter("@Descricao", "ATUALIZACAO LIMITE OPERACIONAL"));
                _sqlCommand.Parameters.Add(new SqlParameter("@StNatureza", opLimit.StNatureza));
                int rows = _sqlCommand.ExecuteNonQuery();
                _fecharConexao();
                if (rows > 0)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na atualizacao de movimentacao de limite Bovespa: " + ex.Message, ex);
                return false;
            }
        }


        public bool AtualizarMvtoBvsp(OperatingLimitInfo opLimit)
        {
            try
            {
                SqlConnection sql = _abrirConexaoSql(_strConnectionStringDefault);
                SqlCommand sqlCommand = new SqlCommand("prc_lmt_atualiza_posicao_limite_oms", sql);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@id_cliente_parametro", opLimit.CodigoParametroCliente));
                sqlCommand.Parameters.Add(new SqlParameter("@ValorAlocado", opLimit.ValorAlocado));
                int rows = sqlCommand.ExecuteNonQuery();
                _fecharConexao(sql);
                if (rows > 0)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na atualizacao de movimentacao de limite Bovespa: " + ex.Message, ex);
                return false;
            }
        }

        public bool AtualizarMvtoBMFContrato(ClientLimitContractBMFInfo contractLimit)
        {
            try
            {
                SqlConnection sql = _abrirConexaoSql(_strConnectionStringDefault);
                SqlCommand sqlCommand = new SqlCommand("prc_lmt_atualiza_limite_bmf_contrato", sql);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@idClienteParametroBMF", contractLimit.IdClienteParametroBMF));
                sqlCommand.Parameters.Add(new SqlParameter("@qtDisponivel", contractLimit.QuantidadeDisponivel));
                sqlCommand.Parameters.Add(new SqlParameter("@dtMovimento", contractLimit.DataMovimento));
                int rows = sqlCommand.ExecuteNonQuery();
                _fecharConexao(sql);
                if (rows > 0)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na atualizacao de movimentacao de limite de contrato BMF: " + ex.Message, ex);
                return false;
            }
        }

        public bool AtualizarMvtoBMFInstrumento(ClientLimitInstrumentBMFInfo instLimit)
        {
            try
            {
                SqlConnection sql = _abrirConexaoSql(_strConnectionStringDefault);
                SqlCommand sqlCommand = new SqlCommand("prc_lmt_atualiza_limite_bmf_instrumento", sql);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@idClienteParametroInstrumento", instLimit.IdClienteParametroInstrumento));
                sqlCommand.Parameters.Add(new SqlParameter("@idClienteParametroBMF", instLimit.IdClienteParametroBMF));
                sqlCommand.Parameters.Add(new SqlParameter("@qtDisponivel", instLimit.QtDisponivel));
                sqlCommand.Parameters.Add(new SqlParameter("@dtMovimento", instLimit.dtMovimento));
                int rows = _sqlCommand.ExecuteNonQuery();
                _fecharConexao(sql);
                if (rows > 0)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na atualizacao de movimentacao de limite de instrumento BMF: " + ex.Message, ex);
                throw ex;
            }
        }


        public bool AtualizarLimiteFixSession(FixLimitInfo fixLimit)
        {
            try
            {
                _abrirConexao(_strConnectionStringDefault);
                _sqlCommand = new SqlCommand("prc_lmt_atualiza_sessao_fix_limite", _sqlConn);
                _sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;

                _sqlCommand.Parameters.Add(new SqlParameter("@id_sessao_fix", fixLimit.IdSessaoFix));
                _sqlCommand.Parameters.Add(new SqlParameter("@ValorDisp", fixLimit.VlDisponivel));
                int rows = _sqlCommand.ExecuteNonQuery();
                _fecharConexao();
                if (rows > 0)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na atualizacao de movimentacao limite fix session: " + ex.Message, ex);
                return false;
            }
        }


        public bool AtualizarRestrictionGlobal(RestrictionGlobalInfo restGlobal)
        {
            try
            {
                SqlConnection sql = _abrirConexaoSql(_strConnectionStringDefault);
                SqlCommand sqlCommand = new SqlCommand("prc_lmt_restriction_global_updt", sql);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@Account", restGlobal.Account));
                sqlCommand.Parameters.Add(new SqlParameter("@VolumeNetAlocado", restGlobal.VolumeNetAlocado));
                sqlCommand.Parameters.Add(new SqlParameter("@QuantidadeNetAlocada", restGlobal.QuantidadeNetAlocada));
                int rows = sqlCommand.ExecuteNonQuery();
                _fecharConexao(sql);
                if (rows > 0)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na atualizacao de movimentacao de RestricaoGlobal: " + ex.Message, ex);
                return false;
            }
        }

        public bool InserirRestrictionGlobalMvto(RestrictionGlobalInfo restGlobal)
        {
            try
            {
                SqlConnection sql = _abrirConexaoSql(_strConnectionStringDefault);
                SqlCommand sqlCommand = new SqlCommand("prc_lmt_restriction_global_ins", sql);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@Account", restGlobal.Account));
                sqlCommand.Parameters.Add(new SqlParameter("@LimiteVolumeNet", restGlobal.LimiteVolumeNet));
                sqlCommand.Parameters.Add(new SqlParameter("@QuantidadeNet", restGlobal.QuantidadeNet));
                sqlCommand.Parameters.Add(new SqlParameter("@LimiteMaxOfertaVolume", restGlobal.LimiteMaxOfertaVolume));
                sqlCommand.Parameters.Add(new SqlParameter("@LimiteMaxOfertaQtde", restGlobal.LimiteMaxOfertaQtde));
                sqlCommand.Parameters.Add(new SqlParameter("@StAtivo", restGlobal.StAtivo));
                sqlCommand.Parameters.Add(new SqlParameter("@DtAtualizacao", restGlobal.DtAtualizacao));
                sqlCommand.Parameters.Add(new SqlParameter("@VolumeNetAlocado", restGlobal.VolumeNetAlocado));
                sqlCommand.Parameters.Add(new SqlParameter("@QuantidadeNetAlocada", restGlobal.QuantidadeNetAlocada));

                int rows = sqlCommand.ExecuteNonQuery();
                _fecharConexao(sql);
                if (rows > 0)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na insercao de movimentacao de RestricaoGlobal: " + ex.Message, ex);
                return false;
            }
        }

        public bool LimparRestrictionGlobal()
        {
            try
            {
                SqlConnection sql = _abrirConexaoSql(_strConnectionStringDefault);
                SqlCommand sqlCommand = new SqlCommand("prc_cliente_restricao_global_mvto_del", sql);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                int rows = sqlCommand.ExecuteNonQuery();
                _fecharConexao(sql);
                if (rows > 0)
                    return true;
                return false;

            }
            catch (Exception ex)
            {
                logger.Error("Problemas na limpeza do Restricao Global do dia corrente: " + ex.Message, ex);
                return false;

            }
        }


        public bool AtualizarRestrictionSymbol(RestrictionSymbolInfo restSymbol)
        {
            try
            {
                SqlConnection sql = _abrirConexaoSql(_strConnectionStringDefault);
                SqlCommand sqlCommand = new SqlCommand("prc_lmt_restriction_symbol_updt", sql);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@Symbol", restSymbol.Symbol));
                sqlCommand.Parameters.Add(new SqlParameter("@VolumeNetAlocado", restSymbol.VolumeNetAlocado));
                sqlCommand.Parameters.Add(new SqlParameter("@QuantidadeNetAlocada", restSymbol.QuantidadeNetAlocada));
                int rows = sqlCommand.ExecuteNonQuery();
                _fecharConexao(sql);
                if (rows > 0)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na atualizacao de movimentacao de RestricaoAtivo: " + ex.Message, ex);
                return false;
            }
        }

        public bool InserirRestrictionSymbolMvto(RestrictionSymbolInfo restSymbol)
        {
            try
            {
                SqlConnection sql = _abrirConexaoSql(_strConnectionStringDefault);
                SqlCommand sqlCommand = new SqlCommand("prc_lmt_restriction_symbol_ins", sql);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@Account", restSymbol.Account));
                sqlCommand.Parameters.Add(new SqlParameter("@Symbol", restSymbol.Symbol));
                sqlCommand.Parameters.Add(new SqlParameter("@LimiteVolumeNet", restSymbol.LimiteVolumeNet));
                sqlCommand.Parameters.Add(new SqlParameter("@QuantidadeNet", restSymbol.QuantidadeNet));
                sqlCommand.Parameters.Add(new SqlParameter("@LimiteMaxOfertaVolume", restSymbol.LimiteMaxOfertaVolume));
                sqlCommand.Parameters.Add(new SqlParameter("@LimiteMaxOfertaQtde", restSymbol.LimiteMaxOfertaQtde));
                sqlCommand.Parameters.Add(new SqlParameter("@StAtivo", restSymbol.StAtivo));
                sqlCommand.Parameters.Add(new SqlParameter("@DtAtualizacao", restSymbol.DtAtualizacao));
                sqlCommand.Parameters.Add(new SqlParameter("@VolumeNetAlocado", restSymbol.VolumeNetAlocado));
                sqlCommand.Parameters.Add(new SqlParameter("@QuantidadeNetAlocada", restSymbol.QuantidadeNetAlocada));
                int rows = sqlCommand.ExecuteNonQuery();
                _fecharConexao(sql);
                if (rows > 0)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na insercao de movimentacao de RestricaoAtivo: " + ex.Message, ex);
                return false;
            }
        }


        public bool LimparRestrictionSymbol()
        {
            try
            {
                SqlConnection sql = _abrirConexaoSql(_strConnectionStringDefault);
                SqlCommand sqlCommand = new SqlCommand("prc_cliente_restricao_ativo_mvto_del", sql);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                int rows = sqlCommand.ExecuteNonQuery();
                _fecharConexao(sql);
                if (rows > 0)
                    return true;
                return false;

            }
            catch (Exception ex)
            {
                logger.Error("Problemas na limpeza do Restricao Symbol do dia corrente: " + ex.Message, ex);
                return false;

            }
        }

        public bool AtualizarRestrictionGroupSymbol(RestrictionGroupSymbolInfo restGroupSymbol)
        {
            try
            {
                SqlConnection sql = _abrirConexaoSql(_strConnectionStringDefault);
                SqlCommand sqlCommand = new SqlCommand("prc_lmt_restriction_group_symbol_updt", sql);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@IdGrupo", restGroupSymbol.IdGrupo));
                sqlCommand.Parameters.Add(new SqlParameter("@VolumeNetAlocado", restGroupSymbol.VolumeNetAlocado));
                sqlCommand.Parameters.Add(new SqlParameter("@QuantidadeNetAlocada", restGroupSymbol.QuantidadeNetAlocada));
                int rows = sqlCommand.ExecuteNonQuery();
                _fecharConexao(sql);
                if (rows > 0)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na atualizacao de movimentacao de RestricaoGrupoAtivo: " + ex.Message, ex);
                return false;
            }
        }

        public bool InserirRestrictionGroupSymbolMvto(RestrictionGroupSymbolInfo restGroupSymbol)
        {
            try
            {
                SqlConnection sql = _abrirConexaoSql(_strConnectionStringDefault);
                SqlCommand sqlCommand = new SqlCommand("prc_lmt_restriction_group_symbol_ins", sql);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@Account", restGroupSymbol.Account));
                sqlCommand.Parameters.Add(new SqlParameter("@LimiteVolumeNet", restGroupSymbol.LimiteVolumeNet));
                sqlCommand.Parameters.Add(new SqlParameter("@QuantidadeNet", restGroupSymbol.QuantidadeNet));
                sqlCommand.Parameters.Add(new SqlParameter("@LimiteMaxOfertaVolume", restGroupSymbol.LimiteMaxOfertaVolume));
                sqlCommand.Parameters.Add(new SqlParameter("@LimiteMaxOfertaQtde", restGroupSymbol.LimiteMaxOfertaQtde));
                sqlCommand.Parameters.Add(new SqlParameter("@IdGrupo", restGroupSymbol.IdGrupo));
                sqlCommand.Parameters.Add(new SqlParameter("@StAtivo", restGroupSymbol.StAtivo));
                sqlCommand.Parameters.Add(new SqlParameter("@DtAtualizacao", restGroupSymbol.DtAtualizacao));
                sqlCommand.Parameters.Add(new SqlParameter("@VolumeNetAlocado", restGroupSymbol.VolumeNetAlocado));
                sqlCommand.Parameters.Add(new SqlParameter("@QuantidadeNetAlocada", restGroupSymbol.QuantidadeNetAlocada));
                int rows = sqlCommand.ExecuteNonQuery();
                _fecharConexao(sql);
                if (rows > 0)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na atualizacao de movimentacao de RestricaoGrupoAtivo: " + ex.Message, ex);
                return false;
            }
        }

        public bool LimparRestrictionGroupSymbol()
        {
            try
            {
                SqlConnection sql = _abrirConexaoSql(_strConnectionStringDefault);
                SqlCommand sqlCommand = new SqlCommand("prc_cliente_restricao_grupo_mvto_del", sql);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                int rows = sqlCommand.ExecuteNonQuery();
                _fecharConexao(sql);
                if (rows > 0)
                    return true;
                return false;

            }
            catch (Exception ex)
            {
                logger.Error("Problemas na limpeza do Restricao Grupo do dia corrente: " + ex.Message, ex);
                return false;

            }
        }


        public bool AtualizarMaxLossLimit(OperatingLimitInfo maxLoss)
        {
            try
            {
                SqlConnection sql = _abrirConexaoSql(_strConnectionStringDefault);
                SqlCommand sqlCommand = new SqlCommand("prc_lmt_atualiza_max_loss", sql);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@id_cliente_parametro", maxLoss.CodigoParametroCliente));
                sqlCommand.Parameters.Add(new SqlParameter("@ValorAlocado", maxLoss.ValorAlocado));
                sqlCommand.Parameters.Add(new SqlParameter("@id_parametro", (int)maxLoss.TipoLimite));
                sqlCommand.Parameters.Add(new SqlParameter("@vl_parametro", maxLoss.ValotTotal));
                sqlCommand.Parameters.Add(new SqlParameter("@Account", maxLoss.CodigoCliente));
                SqlParameter id_inserted = new SqlParameter("@id_inserted", SqlDbType.Int);
                id_inserted.Direction = ParameterDirection.Output;
                sqlCommand.Parameters.Add(id_inserted);
                int rows = sqlCommand.ExecuteNonQuery();
                maxLoss.CodigoParametroCliente = id_inserted.Value.DBToInt32();
                _fecharConexao(sql);
                if (rows > 0)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na atualizacao de movimentacao de Perda Maxima (MaxLoss): " + ex.Message, ex);
                return false;
            }
        }

        public bool LimparPositionClient()
        {
            try
            {
                SqlConnection sql = _abrirConexaoSql(_strConnectionStringDefault);
                SqlCommand sqlCommand = new SqlCommand("prc_cliente_posicao_del", sql);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                int rows = sqlCommand.ExecuteNonQuery();
                _fecharConexao(sql);
                if (rows > 0)
                    return true;
                return false;
                
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na limpeza do PositionClient do dia corrente: " + ex.Message, ex);
                return false;

            }
        }

        public bool AtualizarPositionClient(PosClientSymbolInfo info)
        {
            try
            {
                SqlConnection sql = _abrirConexaoSql(_strConnectionStringDefault);
                SqlCommand sqlCommand = null;
                sqlCommand = new SqlCommand("prc_cliente_posicao_updt", sql);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@Account", info.Account));
                sqlCommand.Parameters.Add(new SqlParameter("@Ativo", info.Ativo));
                sqlCommand.Parameters.Add(new SqlParameter("@Variacao", info.Variacao));
                sqlCommand.Parameters.Add(new SqlParameter("@UltPreco", info.UltPreco));
                sqlCommand.Parameters.Add(new SqlParameter("@QtdAbertura", info.QtdAbertura));
                sqlCommand.Parameters.Add(new SqlParameter("@QtdExecC", info.QtdExecC));
                sqlCommand.Parameters.Add(new SqlParameter("@QtdExecV", info.QtdExecV));
                sqlCommand.Parameters.Add(new SqlParameter("@NetExec", info.NetExec));
                sqlCommand.Parameters.Add(new SqlParameter("@QtdAbC", info.QtdAbC));
                sqlCommand.Parameters.Add(new SqlParameter("@QtdAbV", info.QtdAbV));
                sqlCommand.Parameters.Add(new SqlParameter("@NetAb", info.NetAb));
                sqlCommand.Parameters.Add(new SqlParameter("@PcMedC", info.PcMedC));
                sqlCommand.Parameters.Add(new SqlParameter("@PcMedV", info.PcMedV));
                sqlCommand.Parameters.Add(new SqlParameter("@FinancNet", info.FinancNet));
                sqlCommand.Parameters.Add(new SqlParameter("@LucroPrej", info.LucroPrej));
                sqlCommand.Parameters.Add(new SqlParameter("@DtPosicao", info.DtPosicao));
                sqlCommand.Parameters.Add(new SqlParameter("@DtMovimento", info.DtMovimento ));
                sqlCommand.Parameters.Add(new SqlParameter("@QtdD1", info.QtdD1));
                sqlCommand.Parameters.Add(new SqlParameter("@QtdD2", info.QtdD2));
                sqlCommand.Parameters.Add(new SqlParameter("@QtdD3", info.QtdD3));
                sqlCommand.Parameters.Add(new SqlParameter("@Bolsa", info.Bolsa));
                sqlCommand.Parameters.Add(new SqlParameter("@TipoMercado", info.TipoMercado));
                if (!info.DtVencimento.Equals(DateTime.MinValue))
                    sqlCommand.Parameters.Add(new SqlParameter("@DtVencimento", info.DtVencimento));

                if (!string.IsNullOrEmpty(info.ExecBroker))
                    sqlCommand.Parameters.Add(new SqlParameter("@ExecBroker", info.ExecBroker));
                
                sqlCommand.Parameters.Add(new SqlParameter("@VlrFechamento", info.PrecoFechamento));

                sqlCommand.Parameters.Add(new SqlParameter("@QtdTotal", info.QtdTotal));
                sqlCommand.Parameters.Add(new SqlParameter("@QtdDisponivel", info.QtdDisponivel));
                
                sqlCommand.Parameters.Add(new SqlParameter("@VolCompra", info.VolCompra));
                sqlCommand.Parameters.Add(new SqlParameter("@VolVenda", info.VolVenda));
                sqlCommand.Parameters.Add(new SqlParameter("@VolTotal", info.VolTotal));

                int rows = sqlCommand.ExecuteNonQuery();
                _fecharConexao(sql);
                if (rows > 0)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na atualizacao do PositionClient: " + ex.Message, ex);
                return false;
            }
        }

        public bool InserirPositionClientMvto(PosClientSymbolInfo info)
        {
            try
            {
                SqlConnection sql = _abrirConexaoSql(_strConnectionStringDefault);
                SqlCommand sqlCommand = new SqlCommand("prc_cliente_posicao_ins", sql);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@Account", info.Account));
                sqlCommand.Parameters.Add(new SqlParameter("@Ativo", info.Ativo));
                sqlCommand.Parameters.Add(new SqlParameter("@SegmentoMercado", info.SegmentoMercado));
                sqlCommand.Parameters.Add(new SqlParameter("@Variacao", info.Variacao));
                sqlCommand.Parameters.Add(new SqlParameter("@UltPreco", info.UltPreco));
                sqlCommand.Parameters.Add(new SqlParameter("@QtdAbertura", info.QtdAbertura));
                sqlCommand.Parameters.Add(new SqlParameter("@QtdExecC", info.QtdExecC));
                sqlCommand.Parameters.Add(new SqlParameter("@QtdExecV", info.QtdExecV));
                sqlCommand.Parameters.Add(new SqlParameter("@NetExec", info.NetExec));
                sqlCommand.Parameters.Add(new SqlParameter("@QtdAbC", info.QtdAbC));
                sqlCommand.Parameters.Add(new SqlParameter("@QtdAbV", info.QtdAbV));
                sqlCommand.Parameters.Add(new SqlParameter("@NetAb", info.NetAb));
                sqlCommand.Parameters.Add(new SqlParameter("@PcMedC", info.PcMedC));
                sqlCommand.Parameters.Add(new SqlParameter("@PcMedV", info.PcMedV));
                sqlCommand.Parameters.Add(new SqlParameter("@FinancNet", info.FinancNet));
                sqlCommand.Parameters.Add(new SqlParameter("@LucroPrej", info.LucroPrej));
                sqlCommand.Parameters.Add(new SqlParameter("@DtPosicao", info.DtPosicao));
                sqlCommand.Parameters.Add(new SqlParameter("@DtMovimento", info.DtMovimento));
                sqlCommand.Parameters.Add(new SqlParameter("@QtdD1", info.QtdD1));
                sqlCommand.Parameters.Add(new SqlParameter("@QtdD2", info.QtdD2));
                sqlCommand.Parameters.Add(new SqlParameter("@QtdD3", info.QtdD3));
                sqlCommand.Parameters.Add(new SqlParameter("@Bolsa", info.Bolsa));
                sqlCommand.Parameters.Add(new SqlParameter("@TipoMercado", info.TipoMercado));

                if (!info.DtVencimento.Equals(DateTime.MinValue))
                    sqlCommand.Parameters.Add(new SqlParameter("@DtVencimento", info.DtVencimento));
                if (!string.IsNullOrEmpty(info.ExecBroker))
                    sqlCommand.Parameters.Add(new SqlParameter("@ExecBroker", info.ExecBroker));

                sqlCommand.Parameters.Add(new SqlParameter("@VlrFechamento", info.PrecoFechamento));
                sqlCommand.Parameters.Add(new SqlParameter("@QtdTotal", info.QtdTotal));
                sqlCommand.Parameters.Add(new SqlParameter("@QtdDisponivel", info.QtdDisponivel));

                sqlCommand.Parameters.Add(new SqlParameter("@VolCompra", info.VolCompra));
                sqlCommand.Parameters.Add(new SqlParameter("@VolVenda", info.VolVenda));
                sqlCommand.Parameters.Add(new SqlParameter("@VolTotal", info.VolTotal));

                int rows = sqlCommand.ExecuteNonQuery();
                _fecharConexao(sql);
                if (rows > 0)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na insercao de PositionClient: " + ex.Message, ex);
                return false;
            }
        }

        public bool AtualizarMdsPositionClient(PosClientSymbolInfo info)
        {
            try
            {
                SqlConnection sql = _abrirConexaoSql(_strConnectionStringDefault);
                SqlCommand sqlCommand = new SqlCommand("prc_cliente_posicao_mds_upd", sql);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@Ativo", info.Ativo));
                sqlCommand.Parameters.Add(new SqlParameter("@Variacao", info.Variacao));
                sqlCommand.Parameters.Add(new SqlParameter("@UltPreco", info.UltPreco));
                sqlCommand.Parameters.Add(new SqlParameter("@PrecoFech", info.PrecoFechamento));
                sqlCommand.Parameters.Add(new SqlParameter("@SegmentoMercado", info.SegmentoMercado));
                int rows = sqlCommand.ExecuteNonQuery();
                _fecharConexao(sql);
                if (rows > 0)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na atualizacao do PositionClient: " + ex.Message, ex);
                return false;
            }
        }

        public bool LimparSaldoCC()
        {
            try
            {
                SqlConnection sql = _abrirConexaoSql(_strConnectionStringDefault);
                SqlCommand sqlCommand = new SqlCommand("prc_cliente_saldo_cc_del", sql);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                int rows = sqlCommand.ExecuteNonQuery();
                _fecharConexao(sql);
                if (rows > 0)
                    return true;
                return false;

            }
            catch (Exception ex)
            {
                logger.Error("Problemas na limpeza do SaldoCC do dia corrente: " + ex.Message, ex);
                return false;

            }
        }

        public bool AtualizarSaldoCC(SaldoCcInfo info)
        {
            try
            {
                SqlConnection sql = _abrirConexaoSql(_strConnectionStringDefault);
                SqlCommand sqlCommand = new SqlCommand("prc_cliente_saldo_cc_updt", sql);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@Account", info.Account));
                sqlCommand.Parameters.Add(new SqlParameter("@SaldoD0", info.SaldoD0));
                sqlCommand.Parameters.Add(new SqlParameter("@SaldoD1", info.SaldoD1));
                sqlCommand.Parameters.Add(new SqlParameter("@SaldoD2", info.SaldoD2));
                sqlCommand.Parameters.Add(new SqlParameter("@SaldoD3", info.SaldoD3));
                sqlCommand.Parameters.Add(new SqlParameter("@SaldoCM", info.SaldoContaMargem));
                sqlCommand.Parameters.Add(new SqlParameter("@SaldoBloqueado", info.SaldoContaBloqueado));
                sqlCommand.Parameters.Add(new SqlParameter("@DtMovimento", info.DtMovimento));
                int rows = sqlCommand.ExecuteNonQuery();
                _fecharConexao(sql);
                if (rows > 0)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na atualizacao do PositionClient: " + ex.Message, ex);
                return false;
            }
        }
        #endregion

        #region Risco
        public List<DateTime> ObterFeriadosDI()
        {
            try
            {
                List<DateTime> ret = new List<DateTime>();
                SqlDataAdapter lAdapter;
                _abrirConexao(_strConnectionStringConfig);
                
                DataSet lDataSet = new DataSet();
                _sqlCommand = new SqlCommand("prc_obter_relacao_feriado_DI", _sqlConn);
                _sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                lAdapter = new SqlDataAdapter(_sqlCommand);
                lAdapter.Fill(lDataSet);
                foreach (DataRow lRow in lDataSet.Tables[0].Rows)
                {
                    DateTime lFeriado = lRow["DT_feriado"].DBToDateTime();
                    ret.Add(lFeriado);
                }
                _fecharConexao();
                lAdapter.Dispose();
                lDataSet.Dispose();
                _sqlCommand.Dispose();
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error("Erro ao obter Feriados DI: " + ex.Message, ex);
                return null;
            }
        }

        #endregion

        #region Risco Consolidado
        public bool LimparRiscoConsolidado()
        {
            try
            {
                SqlConnection sql = _abrirConexaoSql(_strConnectionStringDefault);
                SqlCommand sqlCommand = new SqlCommand("prc_risco_consolidado_del", sql);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                int rows = sqlCommand.ExecuteNonQuery();
                _fecharConexao(sql);
                if (rows > 0)
                    return true;
                return false;

            }
            catch (Exception ex)
            {
                logger.Error("Problemas na limpeza do Risco Consolidado do dia corrente: " + ex.Message, ex);
                return false;

            }
        }

        public bool AtualizarRiscoConsolidado(ConsolidatedRiskInfo info)
        {
            try
            {
                SqlConnection sql = _abrirConexaoSql(_strConnectionStringDefault);
                SqlCommand sqlCommand = null;
                
                sqlCommand = new SqlCommand("prc_risco_consolidado_updt", sql);
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(new SqlParameter("@Account", info.Account));
                sqlCommand.Parameters.Add(new SqlParameter("@TotalCustodiaAbertura", info.TotalCustodiaAbertura));
                sqlCommand.Parameters.Add(new SqlParameter("@TotalContaCorrenteAbertura", info.TotalContaCorrenteOnline));
                sqlCommand.Parameters.Add(new SqlParameter("@TotalGarantias", info.TotalGarantias));
                sqlCommand.Parameters.Add(new SqlParameter("@TotalProdutos", info.TotalProdutos));
                sqlCommand.Parameters.Add(new SqlParameter("@SaldoTotalAbertura", info.SaldoTotalAbertura));
                sqlCommand.Parameters.Add(new SqlParameter("@PLBovespa", info.PLBovespa));
                sqlCommand.Parameters.Add(new SqlParameter("@PLBmf", info.PLBmf  ));
                sqlCommand.Parameters.Add(new SqlParameter("@PLTotal", info.PLTotal));
                sqlCommand.Parameters.Add(new SqlParameter("@SFP", info.SFP));
                sqlCommand.Parameters.Add(new SqlParameter("@TotalPercentualAtingido", info.TotalPercentualAtingido));
                
                if (!info.DtMovimento.Equals(DateTime.MinValue))
                    sqlCommand.Parameters.Add(new SqlParameter("@DtMovimento", info.DtMovimento));

                int rows = sqlCommand.ExecuteNonQuery();
                _fecharConexao(sql);
                if (rows > 0)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na atualizacao de Risco Consolidado: " + ex.Message, ex);
                return false;
            }
        }

        public ConcurrentDictionary<int, ProdutosInfo> CarregarProdutos()
        {
            try
            {
                ConcurrentDictionary<int, ProdutosInfo> ret = new ConcurrentDictionary<int, ProdutosInfo>();
                SqlDataAdapter lAdapter;
                _abrirConexao(_strConnectionStringMinicom);

                DataSet lDataSet = new DataSet();
                string query = "SELECT CD_CLIENTE, SUM(SALDOLIQUIDO) as SALDOLIQUIDO FROM VW_OMS_FixedIncome with (nolock) WHERE Vencimento > GETDATE() "+ 
                               "GROUP BY CD_CLIENTE " + 
                               "Order by CD_CLIENTE ";
                _sqlCommand = new SqlCommand(query, _sqlConn);
                _sqlCommand.CommandType = System.Data.CommandType.Text;
                lAdapter = new SqlDataAdapter(_sqlCommand);
                lAdapter.Fill(lDataSet);
                foreach (DataRow lRow in lDataSet.Tables[0].Rows)
                {
                    ProdutosInfo pInfo = new ProdutosInfo();
                    pInfo.CodCliente = lRow["CD_CLIENTE"].DBToInt32();
                    pInfo.SaldoLiquido = lRow["SALDOLIQUIDO"].DBToDecimal();
                    ret.AddOrUpdate(pInfo.CodCliente, pInfo, (key, oldvalue) => pInfo);
                }
                _fecharConexao();
                lAdapter.Dispose();
                lDataSet.Dispose();
                _sqlCommand.Dispose();
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error("Erro ao obter Saldo Liquido Produto: " + ex.Message, ex);
                return null;
            }
        }

        /// <summary>
        /// Carregar dados de cotação de um ativo específico
        /// </summary>
        /// <param name="Ativo">Ativo para buscar cotação</param>
        /// <returns>Retorno informações de cotação no objeto encapsulado</returns>
        public CotacaoInfo CarregarInstrumentoCotacao(string Ativo)
        {
            var lRetorno = new CotacaoInfo();

            try
            {
                _abrirConexao(_strConnectionStringMDS);
                
                SqlDataAdapter lAdapter;

                DataSet lDataSet = new DataSet();

                string query = "select * from tb_ativo_cotacao where id_ativo='" + Ativo + "'";
                               
                _sqlCommand = new SqlCommand(query, _sqlConn);

                _sqlCommand.CommandType = System.Data.CommandType.Text;
                
                lAdapter = new SqlDataAdapter(_sqlCommand);
                
                lAdapter.Fill(lDataSet);
                
                foreach (DataRow lRow in lDataSet.Tables[0].Rows)
                {
                    lRetorno.Instrumento    = lRow["id_ativo"].DBToString();

                    lRetorno.UltimoPreco    = lRow["vl_ultima"].DBToDecimal();

                    lRetorno.PU             = lRow["vl_precounitario"].DBToDecimal();

                    lRetorno.Ajuste         = lRow["vl_ajuste"].DBToDecimal();

                    lRetorno.Fechamento     = lRow["vl_fechamento"].DBToDecimal();

                    lRetorno.Abertura       = lRow["vl_abertura"].DBToDecimal();
                }

                _fecharConexao();
                
                lAdapter.Dispose();

                lDataSet.Dispose();
                
                _sqlCommand.Dispose();

                return lRetorno;
            }
            catch (Exception ex)
            {
                logger.Error("Erro ao obter cotacao do ativo " + Ativo + " " + ex.Message, ex);

                return null;
            }
            

            return lRetorno;
        }
        #endregion


    }
}
