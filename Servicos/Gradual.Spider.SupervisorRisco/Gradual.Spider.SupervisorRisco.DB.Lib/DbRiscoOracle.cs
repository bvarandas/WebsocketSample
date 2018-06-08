using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Gradual.Spider.SupervisorRisco.Lib.Dados;
using Gradual.Generico.Dados;
using System.Data.Common;
using System.Data;
using System.Collections.Concurrent;
using System.Collections;


namespace Gradual.Spider.SupervisorRisco.DB.Lib
{
    public class DbRiscoOracle
    {
        #region log4net declaration
        public static readonly log4net.ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        public DbRiscoOracle()
        {

        }

        public ConcurrentDictionary<int, List<PosClientSymbolInfo>> CarregarPosicaoAberturaOrig(ConcurrentDictionary<string, SymbolInfo> papeis)
        {
            ConcurrentDictionary<int, List<PosClientSymbolInfo>> ret = new ConcurrentDictionary<int, List<PosClientSymbolInfo>>();
            try
            {
                
                AcessaDados acesso = new AcessaDados("Retorno");
                acesso.ConnectionStringName = "TRADE";
                int acc = 0;
                
                // Carregar series de vencimento para compor ativos em caso de Bmf
                ConcurrentDictionary<string, TmfSerieInfo> dic = this.CarregarTmfSerie();

                // Buscar Posicao de cliente bovespa
                using (DbCommand cmd = acesso.CreateCommand(CommandType.StoredProcedure, "PRC_OBTER_POS_CLIENTE_ABERTURA"))
                {
                    DataTable table = acesso.ExecuteOracleDataTable(cmd);

                    List<PosClientSymbolInfo> item = new List<PosClientSymbolInfo>();
                    
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        DataRow dataRow = table.Rows[i];

                        // Validar se é tesouro direto
                        if (dataRow["TIPO_GRUP"].DBToString().Equals("TEDI"))
                            continue;

                        // Validar se o papel é TERMO
                        string symbAux = dataRow["COD_NEG"].DBToString();
                        
                        if (!string.IsNullOrEmpty(symbAux) &&  symbAux.Substring(symbAux.Length - 1, 1).Equals("T"))
                            continue;

                        int currentAccount = dataRow["COD_CLI"].DBToInt32();
                        
                        if (acc != currentAccount && acc!=0)
                        {
                            ret.AddOrUpdate(acc, item, (key, oldvalue)=>item);
                            item = new List<PosClientSymbolInfo>();
                        }

                        // Verificar se a posicao do cliente ja existe
                        List<PosClientSymbolInfo> lstAux = null;
                        PosClientSymbolInfo posExist = null;
                        if (ret.TryGetValue(acc, out lstAux))
                        {
                            string ativo = dataRow["COD_NEG"].DBToString();
                            int carteira = dataRow["COD_CART"].DBToInt32();
                            posExist = lstAux.FirstOrDefault(x=>x.Account.Equals(acc) && x.Ativo.Equals(ativo));
                        }

                        PosClientSymbolInfo posSymbol = new PosClientSymbolInfo();
                        if (posExist == null)
                            posExist = new PosClientSymbolInfo();
                        posSymbol.Account = dataRow["COD_CLI"].DBToInt32();
                        posSymbol.ExecBroker = dataRow["COD_CLI"].DBToString();
                        posSymbol.Ativo = dataRow["COD_NEG"].DBToString();
                        posSymbol.QtdAbertura = dataRow["QTDE_DISP"].DBToDecimal() + posExist.QtdAbertura;
                        posSymbol.QtdD1 = dataRow["QTDE_DA1"].DBToDecimal() + posExist.QtdD1;
                        posSymbol.QtdD2 = dataRow["QTDE_DA2"].DBToDecimal() + posExist.QtdD2;
                        posSymbol.QtdD3 = dataRow["QTDE_DA3"].DBToDecimal() + posExist.QtdD3;
                        posSymbol.DtPosicao = DateTime.Now.Date;
                        posSymbol.DtMovimento = DateTime.Now;
                        posSymbol.CodCarteira = dataRow["COD_CART"].DBToInt32();
                        posSymbol.TipoMercado = dataRow["TIPO_MERC"].DBToString();

                        // Validacao do tipo de grupo para definir bolsa
                        // Em teoria: ACOES TEDI, BMF
                        string tpGrupo = dataRow["TIPO_GRUP"].DBToString();
                        if (tpGrupo.Equals("BMF", StringComparison.CurrentCultureIgnoreCase))
                        {
                            posSymbol.Bolsa = Exchange.Bmf;
                            // Para bmf, recompor o symbol
                            posSymbol.Ativo = this._composeSymbol(dataRow, dic);
                        }
                        else
                            posSymbol.Bolsa = Exchange.Bovespa;

                        posSymbol.DtVencimento = dataRow["DATA_VENC"].DBToDateTime();
                        
                        // Parte de cadastro de ativos / cotacao
                        SymbolInfo symb = null;
                        if (papeis.TryGetValue(posSymbol.Ativo, out symb))
                        {
                            posSymbol.CodPapelObjeto = symb.CodigoPapelObjeto;
                            posSymbol.PrecoFechamento = symb.VlrFechamento;
                            posSymbol.Variacao = symb.VlrOscilacao;
                            posSymbol.UltPreco = symb.VlrUltima;
                        }
                        PosClientSymbolInfo xx = item.FirstOrDefault(x => x.Ativo.Equals(posSymbol.Ativo));
                        if (xx == null)
                            item.Add(posSymbol);
                        else
                            xx = posSymbol; // Atualzia  a referencia que achou para nova referencia
                            
                        acc = currentAccount;
                    }
                    ret.AddOrUpdate(acc, item, (key, oldvalule) => item);
                }
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na consulta da posicao de abertura dos clientes: " + ex.Message, ex);
                return null;
            }
        }

        public ConcurrentDictionary<int, ConcurrentDictionary<string, PosClientSymbolInfo>> CarregarPosicaoAbertura(ConcurrentDictionary<string, SymbolInfo> papeis)
        {
            ConcurrentDictionary<int, ConcurrentDictionary<string, PosClientSymbolInfo>> ret = new ConcurrentDictionary<int, ConcurrentDictionary<string, PosClientSymbolInfo>>();
            try
            {

                AcessaDados acesso = new AcessaDados("Retorno");
                acesso.ConnectionStringName = "TRADE";
                int acc = 0;

                // Carregar series de vencimento para compor ativos em caso de Bmf
                ConcurrentDictionary<string, TmfSerieInfo> dic = this.CarregarTmfSerie();

                // Buscar Posicao de cliente bovespa
                using (DbCommand cmd = acesso.CreateCommand(CommandType.StoredProcedure, "PRC_OBTER_POS_CLIENTE_ABERTURA"))
                {
                    DataTable table = acesso.ExecuteOracleDataTable(cmd);

                    ConcurrentDictionary<string, PosClientSymbolInfo> item = new ConcurrentDictionary<string,PosClientSymbolInfo>();
                    
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        DataRow dataRow = table.Rows[i];
                        
                        // Validar se é tesouro direto
                        if (dataRow["TIPO_GRUP"].DBToString().Equals("TEDI"))
                            continue;

                        // Validar se o papel é TERMO
                        string symbAux = dataRow["COD_NEG"].DBToString();
                        
                        if (!string.IsNullOrEmpty(symbAux) && symbAux.Substring(symbAux.Length - 1, 1).Equals("T"))
                            continue;

                        int currentAccount = dataRow["COD_CLI"].DBToInt32();
                        
                        logger.DebugFormat("Acc[{0}] Current[{1}] Symbol[{2}]", acc, currentAccount, symbAux);
                        
                        if (acc != currentAccount)
                        {
                            acc = currentAccount;

                            item = new ConcurrentDictionary<string, PosClientSymbolInfo>();

                            ret.AddOrUpdate(acc, item, (key, oldvalue) => item);
                        }

                        PosClientSymbolInfo posSymbol = new PosClientSymbolInfo();
                        
                        posSymbol.TypePosition = PositionTypeEnum.Abertura;

                        // Validacao do tipo de grupo para definir bolsa
                        // Em teoria: ACOES TEDI, BMF
                        string tpGrupo = dataRow["TIPO_GRUP"].DBToString();

                        if (tpGrupo.Equals("BMF", StringComparison.CurrentCultureIgnoreCase))
                        {
                            posSymbol.Bolsa = Exchange.Bmf;
                            // Para bmf, recompor o symbol
                            posSymbol.Ativo = this._composeSymbol(dataRow, dic);
                        }
                        else
                        {
                            posSymbol.Bolsa = Exchange.Bovespa;
                            posSymbol.Ativo = dataRow["COD_NEG"].DBToString();
                        }
                        // Verificar se a posicao do cliente ja existe
                        ConcurrentDictionary<string, PosClientSymbolInfo> cdAux = null;

                        PosClientSymbolInfo posExist = null;
                        
                        if (ret.TryGetValue(acc, out cdAux))
                        {
                            //string ativo = dataRow["COD_NEG"].DBToString();
                            //int carteira = dataRow["COD_CART"].DBToInt32();
                            //cdAux.TryGetValue(ativo, out posExist);
                            cdAux.TryGetValue(posSymbol.Ativo, out posExist);
                            // posExist = lstAux.FirstOrDefault(x => x.Account.Equals(acc) && x.Ativo.Equals(ativo));
                        }


                        if (posExist == null) 
                        { 
                            posExist = new PosClientSymbolInfo(); 
                        }

                        posSymbol.Account = dataRow["COD_CLI"].DBToInt32();
                        posSymbol.ExecBroker = dataRow["COD_CLI"].DBToString();
                        // posSymbol.Ativo = dataRow["COD_NEG"].DBToString();
                        posSymbol.QtdAbertura = dataRow["QTDE_DISP"].DBToDecimal() + posExist.QtdAbertura;
                        posSymbol.QtdD1 = dataRow["QTDE_DA1"].DBToDecimal() + posExist.QtdD1;
                        posSymbol.QtdD2 = dataRow["QTDE_DA2"].DBToDecimal() + posExist.QtdD2;
                        posSymbol.QtdD3 = dataRow["QTDE_DA3"].DBToDecimal() + posExist.QtdD3;
                        posSymbol.DtPosicao = DateTime.Now.Date;
                        posSymbol.DtMovimento = DateTime.Now;
                        posSymbol.CodCarteira = dataRow["COD_CART"].DBToInt32();
                        posSymbol.TipoMercado = dataRow["TIPO_MERC"].DBToString();
                        posSymbol.QtdDisponivel = posSymbol.QtdAbertura + posSymbol.QtdD1 + posSymbol.QtdD2;
                        posSymbol.QtdTotal = posSymbol.QtdDisponivel + posSymbol.NetExec;
                        
                        
                        //// Validacao do tipo de grupo para definir bolsa
                        //// Em teoria: ACOES TEDI, BMF
                        //string tpGrupo = dataRow["TIPO_GRUP"].DBToString();
                        //if (tpGrupo.Equals("BMF", StringComparison.CurrentCultureIgnoreCase))
                        //{
                        //    posSymbol.Bolsa = Exchange.Bmf;
                        //    // Para bmf, recompor o symbol
                        //    posSymbol.Ativo = this._composeSymbol(dataRow, dic);
                        //}
                        //else
                        //    posSymbol.Bolsa = Exchange.Bovespa;

                        posSymbol.DtVencimento = dataRow["DATA_VENC"].DBToDateTime();

                        // Parte de cadastro de ativos / cotacao
                        SymbolInfo symb = null;
                        if (papeis.TryGetValue(posSymbol.Ativo, out symb))
                        {
                            posSymbol.CodPapelObjeto = symb.CodigoPapelObjeto;
                            posSymbol.PrecoFechamento = symb.VlrFechamento;
                            posSymbol.Variacao = symb.VlrOscilacao;
                            posSymbol.UltPreco = symb.VlrUltima;
                        }
                        // PosClientSymbolInfo xx = item.FirstOrDefault(x => x.Ativo.Equals(posSymbol.Ativo));
                        item.AddOrUpdate(posSymbol.Ativo, posSymbol, (key, oldvalue)=>posSymbol);
                        
                        ret.AddOrUpdate(acc, item, (key, oldvalule) => item);
                    }
                    //ret.AddOrUpdate(acc, item, (key, oldvalule) => item);
                }
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na consulta da posicao de abertura dos clientes: " + ex.Message, ex);
                return null;
            }
        }

        public ConcurrentDictionary<string, TmfSerieInfo> CarregarTmfSerie()
        {
            ConcurrentDictionary<string, TmfSerieInfo> ret = new ConcurrentDictionary<string, TmfSerieInfo>();
            try
            {
                AcessaDados acesso = new AcessaDados("Retorno");
                acesso.ConnectionStringName = "TRADE";
                // Buscar Posicao de cliente bovespa
                using (DbCommand cmd = acesso.CreateCommand(CommandType.StoredProcedure, "PRC_OBTER_TMF_SERIE"))
                {
                    DataTable table = acesso.ExecuteOracleDataTable(cmd);
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        DataRow dataRow = table.Rows[i];
                        string chave = string.Format("{0}-{1}-{2}", dataRow["CD_COMMOD"].DBToString(), dataRow["CD_MERCAD"].DBToString(), dataRow["CD_SERIE"].DBToString());
                        TmfSerieInfo tmf = new TmfSerieInfo();
                        tmf.CdCommod = dataRow["CD_COMMOD"].DBToString();
                        tmf.CdMercad = dataRow["CD_MERCAD"].DBToString();
                        tmf.CdSerie = dataRow["CD_SERIE"].DBToString();
                        tmf.CdCodNeg = dataRow["CD_CODNEG"].DBToString();
                        ret.AddOrUpdate(chave, tmf, (key, oldvalule) => tmf);
                    }
                }
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na consulta de TmfSerie: " + ex.Message, ex);
                return null;
            }
        }

        public List<ClientDataInfo> CarregarDadosCliente()
        {
            var lRetorno = new List<ClientDataInfo>();
            try
            {
                var acesso = new AcessaDados("Retorno");

                acesso.ConnectionStringName = "TRADE";

                using (DbCommand cmd = acesso.CreateCommand(CommandType.StoredProcedure, "PRC_OBTEM_CLIENTE_ATIVO_LST"))
                {
                    DataTable table = acesso.ExecuteOracleDataTable(cmd);

                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        var lCliente = new ClientDataInfo();

                        DataRow lRow = table.Rows[i];

                        string lBolsa = (lRow["Tipo"]).DBToString();

                        lCliente.CodigoAssessor = lRow["CD_ASSESSOR"].DBToInt32();
                        lCliente.NomeAssessor   = lRow["NM_ASSESSOR"].DBToString();
                        lCliente.NomeCliente    = lRow["NM_CLIENTE"].DBToString();
                        lCliente.DsCpfCnpj      = lRow["CD_CPFCGC"].DBToString();
                        lCliente.Bolsa          = lBolsa;

                        if (lBolsa.Equals("BOVESPA"))
                        {
                            lCliente.CodigoBovespa = (lRow["Codigo"]).DBToInt32();
                        }
                        else
                        {
                            lCliente.CodigoBmf = (lRow["Codigo"]).DBToInt32();
                        }

                        lRetorno.Add(lCliente);
                    }
                }

                return lRetorno;
            }
            catch (Exception ex)
            {
                logger.Error("Erro ao obter a lista de clientes com os dados de Nome, CodigoBovespa e CodigoBmf: " + ex.Message, ex);
                return null;
            }
        }

        private string _composeSymbol(DataRow dr, ConcurrentDictionary<string, TmfSerieInfo> dict)
        {
            string ret = string.Empty;
            switch (dr["TIPO_MERC"].DBToString())
            {
                case "FUT":
                    ret = dr["COD_COMM"].DBToString() + dr["COD_SERI"].DBToString();
                    break;
                default:
                    // Buscar ativo no dicionario de series
                    string chave = string.Format("{0}-{1}-{2}", dr["COD_COMM"].DBToString(), dr["TIPO_MERC"].DBToString(), dr["COD_SERI"].DBToString());
                    TmfSerieInfo item = null;
                    if (dict.TryGetValue(chave, out item))
                        ret = item.CdCodNeg;
                    break;
            }
            return ret;
        }

        public double ObteCotacaoPtax()
        {
            double ret = Double.MinValue;
            try
            {
                AcessaDados acesso = new AcessaDados("Retorno");
                acesso.ConnectionStringName = "TRADE";
                // Busca cotacao Ptax
                using (DbCommand cmd = acesso.CreateCommand(CommandType.StoredProcedure, "PRC_OBTER_COTACAO_PTAX"))
                {
                    DataTable table = acesso.ExecuteOracleDataTable(cmd);
                    if (table.Rows.Count>0)
                    {
                        
                        
                        
                        DataRow dataRow = table.Rows[0];
                        double? aux = dataRow["VL_DOLVDA_ATU"].DBToDouble();
                        if (aux == null)
                            ret = Double.MinValue;
                        else
                            ret = Convert.ToDouble(aux);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na consulta de cotacao Ptax: " + ex.Message, ex);
            }
            return ret;
        }

        public Hashtable ObterVencimentosDI()
        {

            Hashtable ret = new Hashtable();
            try
            {
                AcessaDados acesso = new AcessaDados("Retorno");
                acesso.ConnectionStringName = "TRADE";
                // Busca cotacao Ptax
                using (DbCommand cmd = acesso.CreateCommand(CommandType.StoredProcedure, "prc_obter_relacao_DI"))
                {
                    DataTable table = acesso.ExecuteOracleDataTable(cmd);
                    if (table.Rows.Count > 0)
                    {
                        for (int i = 0; i < table.Rows.Count; i++)
                        {
                            DataRow dataRow = table.Rows[i];
                            string Instrumento = dataRow["cd_codneg"].DBToString();
                            DateTime Vencimento = dataRow["DT_VENC"].DBToDateTime();
                            ret.Add(Instrumento, Vencimento);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na consulta de cotacao Ptax: " + ex.Message, ex);
            }
            return ret;
        }

        #region RiscoConsolidado
        public ConcurrentDictionary<int, GarantiaInfo> CarregarGarantiaBovespa()
        {
            ConcurrentDictionary<int, GarantiaInfo> ret = new ConcurrentDictionary<int, GarantiaInfo>();
            try
            {

                AcessaDados acesso = new AcessaDados("Retorno");
                acesso.ConnectionStringName = "TRADE";
             
                // Buscar Garantia de Bovespa
                using (DbCommand cmd = acesso.CreateCommand(CommandType.StoredProcedure, "PRC_OBTER_GARANTIAS_BOVESPA"))
                {
                    DataTable table = acesso.ExecuteOracleDataTable(cmd);
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        DataRow dataRow = table.Rows[i];
                        GarantiaInfo gi = new GarantiaInfo();
                        gi.CodCliente = dataRow["COD_CLI"].DBToInt32();
                        gi.GarantiaDisponivel = dataRow["GarantiaDisponivel"].DBToDecimal();
                        gi.Bolsa = Exchange.Bovespa;
                        ret.AddOrUpdate(gi.CodCliente, gi, (key, oldvalule) => gi);    
                    }
                    
                }
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na consulta da posicao de garantia de bovespa dos clientes: " + ex.Message, ex);
                return null;
            }

        }

        public ConcurrentDictionary<int, GarantiaInfo> CarregarGarantiaBmf()
        {
            ConcurrentDictionary<int, GarantiaInfo> ret = new ConcurrentDictionary<int, GarantiaInfo>();
            try
            {

                AcessaDados acesso = new AcessaDados("Retorno");
                acesso.ConnectionStringName = "TRADE";

                // Buscar Garantia de Bovespa
                using (DbCommand cmd = acesso.CreateCommand(CommandType.StoredProcedure, "PRC_OBTER_GARANTIAS_BMF"))
                {
                    DataTable table = acesso.ExecuteOracleDataTable(cmd);
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        DataRow dataRow = table.Rows[i];
                        GarantiaInfo gi = new GarantiaInfo();
                        gi.CodCliente = dataRow["CD_CLIENTE"].DBToInt32();
                        gi.GarantiaDisponivel = dataRow["GarantiaDisponivel"].DBToDecimal();
                        gi.Bolsa = Exchange.Bmf;
                        ret.AddOrUpdate(gi.CodCliente, gi, (key, oldvalule) => gi);
                    }

                }
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na consulta da posicao de garantia de bmf dos clientes: " + ex.Message, ex);
                return null;
            }
        }

        public List<GarantiaInfo> CarregarGarantiaBmfCarteiras(int CodigoCliente)
        {
            var lRetorno = new List<GarantiaInfo>();

            try
            {
                AcessaDados acesso = new AcessaDados("Retorno");

                acesso.ConnectionStringName = "TRADE";

                // Buscar Garantia de Bmf de cliente
                using (DbCommand cmd = acesso.CreateCommand(CommandType.StoredProcedure, "prc_obter_garantias_bmf_23906"))
                {
                    acesso.AddInParameter(cmd, "cd_cliente", DbType.Int32, CodigoCliente);

                    DataTable table = acesso.ExecuteOracleDataTable(cmd);

                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        DataRow dataRow = table.Rows[i];

                        GarantiaInfo lInfo = new GarantiaInfo();

                        lInfo.CodCliente            = dataRow["COD_CLI"].DBToInt32();
                        //lInfo.GarantiaDisponivel    = dataRow["GarantiaDisponivel"].DBToDecimal();
                        lInfo.Bolsa                 = Exchange.Bmf;
                        lInfo.Quantidade            = dataRow["QTD"].DBToInt32();
                        lInfo.Instrumento           = dataRow["COD_NEG"].DBToString();

                        lRetorno.Add(lInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na consulta da posicao de garantia de bmf do cliente: " + CodigoCliente + " - " + ex.Message, ex);
                return null;   
            }

            return lRetorno;
        }

        public List<GarantiaInfo> CarregarGarantiaBovespaCarteiras(int CodigoCliente)
        {

            var lRetorno = new List<GarantiaInfo>();

            try
            {
                AcessaDados acesso = new AcessaDados("Retorno");

                acesso.ConnectionStringName = "TRADE";

                // Buscar Garantia de Bovespa
                using (DbCommand cmd = acesso.CreateCommand(CommandType.StoredProcedure, "prc_obter_garantias_bov_23019"))
                {
                    acesso.AddInParameter(cmd, "cd_cliente", DbType.Int32, CodigoCliente);

                    DataTable table = acesso.ExecuteOracleDataTable(cmd);

                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        DataRow dataRow = table.Rows[i];

                        GarantiaInfo lInfo = new GarantiaInfo();

                        lInfo.CodCliente = dataRow["COD_CLI"].DBToInt32();
                        //lInfo.GarantiaDisponivel    = dataRow["GarantiaDisponivel"].DBToDecimal();
                        lInfo.Bolsa = Exchange.Bmf;
                        lInfo.Quantidade = dataRow["QTD"].DBToInt32();
                        lInfo.Instrumento = dataRow["COD_NEG"].DBToString();

                        lRetorno.Add(lInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na consulta da posicao de garantia de bovespa do cliente: " + CodigoCliente + " - " + ex.Message, ex);
            }

            return lRetorno;
        }

        /// <summary>
        /// Método que busca no sinacor a listagem de posição BTC do cliente
        /// Procedure: prc_posicaoclient_btc
        /// </summary>
        /// <param name="idCliente">Código bovespa do cliente</param>
        /// <returns>Retorna listagem da posição BTC do cliente</returns>
        public List<BTCInfo> ObterPosicaoBTC()
        {
            AcessaDados lAcessaDados = new AcessaDados();

            BTCInfo _PosicaoBTC;
            List<BTCInfo> lstBTC = new List<BTCInfo>();

            try
            {
                lAcessaDados.ConnectionStringName = "TRADE";

                using (DbCommand lDbCommand = lAcessaDados.CreateCommand(CommandType.StoredProcedure, "PRC_POSI_BTC_TOMADOR"))
                {
                    //lAcessaDados.AddInParameter(lDbCommand, "IdCliente", DbType.Int32, idCliente);

                    DataTable lDataTable = lAcessaDados.ExecuteOracleDataTable(lDbCommand);

                    if (null != lDataTable && lDataTable.Rows.Count > 0)
                    {
                        for (int i = 0; i <= lDataTable.Rows.Count - 1; i++)
                        {
                            _PosicaoBTC = new BTCInfo();

                            _PosicaoBTC.CodigoCliente = (lDataTable.Rows[i]["COD_CLI"]).DBToInt32();
                            _PosicaoBTC.Carteira = (lDataTable.Rows[i]["COD_CART"]).DBToInt32();
                            _PosicaoBTC.Instrumento = (lDataTable.Rows[i]["COD_NEG"]).DBToString();

                            _PosicaoBTC.DataAbertura = (lDataTable.Rows[i]["DATA_ABER"]).DBToDateTime();
                            _PosicaoBTC.DataVencimento = (lDataTable.Rows[i]["DATA_VENC"]).DBToDateTime();

                            _PosicaoBTC.PrecoMedio = (lDataTable.Rows[i]["PREC_MED"]).DBToDecimal();
                            _PosicaoBTC.Quantidade = (lDataTable.Rows[i]["QTDE_ACOE"]).DBToInt32();
                            _PosicaoBTC.Remuneracao = (lDataTable.Rows[i]["VAL_LIQ"]).DBToDecimal();
                            _PosicaoBTC.Taxa = (lDataTable.Rows[i]["TAXA_REMU"]).DBToDecimal();
                            _PosicaoBTC.TipoContrato = (lDataTable.Rows[i]["TIPO_COTR"]).DBToString();

                            lstBTC.Add(_PosicaoBTC);
                        }
                    }
                }

                return lstBTC;
            }
            catch (Exception ex)
            {
                throw (ex);

            }

        }

        
        /// <summary>
        /// Carrega os dados de conta corrente de liquidação de D0 + D1 + D2 + D3
        /// </summary>
        /// <returns>Retorna um dictionary com a conta do cliente e com os dados de conta corrente </returns>
        public ConcurrentDictionary<int, ContaCorrenteInfo> CarregarContaCorrente()
        {
            var lRetorno = new ConcurrentDictionary<int, ContaCorrenteInfo>();

            try
            {

                AcessaDados acesso = new AcessaDados("Retorno");

                acesso.ConnectionStringName = "TRADE";
                
                using (DbCommand cmd = acesso.CreateCommand(CommandType.StoredProcedure, "PRC_LIQUIDACAO_DIA_SEL"))
                {
                    DataTable table = acesso.ExecuteOracleDataTable(cmd);

                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        DataRow dataRow = table.Rows[i];

                        if (dataRow["VL_TOTAL"].DBToDecimal().Equals(0))
                            continue;

                        ContaCorrenteInfo cc = new ContaCorrenteInfo();
                        
                        cc.CodCliente = dataRow["CD_CLIENTE"].DBToInt32();

                        ContaCorrenteInfo lFounded = null;

                        if (lRetorno.TryGetValue(cc.CodCliente, out lFounded))
                        {
                            lFounded.VlrTotal += dataRow["VL_TOTAL"].DBToDecimal();

                            cc.VlrTotal = lFounded.VlrTotal;

                        }
                        else
                        {
                            decimal lAberturaOpcao = AberturaDiaOpcoes(cc.CodCliente);

                            cc.VlrTotal = (dataRow["VL_TOTAL"].DBToDecimal() + lAberturaOpcao);
                        }

                        lRetorno.AddOrUpdate(cc.CodCliente, cc, (key, oldvalule) => cc);
                    }
                }

                return lRetorno;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na consulta da posicao de conta corrente dos clientes: " + ex.Message, ex);
                return null;
            }
        }

        public ContaCorrenteInfo CarregarContaCorrenteCliente(int CodigoCliente)
        {
            var lDicCC = new ConcurrentDictionary<int, ContaCorrenteInfo>();
            
            var lRetorno = new ContaCorrenteInfo();

            try
            {
                AcessaDados acesso = new AcessaDados("Retorno");

                acesso.ConnectionStringName = "TRADE";

                logger.InfoFormat("Recuperando informações de conta corrente do cliente [{0}]", CodigoCliente);

                using (DbCommand cmd = acesso.CreateCommand(CommandType.StoredProcedure, "PRC_LIQ_CC_CLIENTE_SEL"))
                {
                    acesso.AddInParameter(cmd, "CodigoCliente", DbType.Int32, CodigoCliente);

                    DataTable table = acesso.ExecuteOracleDataTable(cmd);

                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        DataRow dataRow = table.Rows[i];

                        ContaCorrenteInfo cc = new ContaCorrenteInfo();

                        cc.CodCliente = dataRow["CD_CLIENTE"].DBToInt32();

                        ContaCorrenteInfo lFounded = null;

                        if (lDicCC.TryGetValue(cc.CodCliente, out lFounded))
                        {
                            lFounded.VlrTotal += dataRow["VL_TOTAL"].DBToDecimal();

                            cc.VlrTotal = lFounded.VlrTotal;

                        }
                        else
                        {
                            decimal lAberturaOpcao = AberturaDiaOpcoes(cc.CodCliente);

                            cc.VlrTotal = (dataRow["VL_TOTAL"].DBToDecimal() + lAberturaOpcao);
                        }

                        lDicCC.AddOrUpdate(cc.CodCliente, cc, (key, oldvalule) => cc);
                    }
                }

                lDicCC.TryGetValue(CodigoCliente, out lRetorno);

                return lRetorno;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na consulta da posicao de conta corrente dos clientes: " + ex.Message, ex);
                return null;
            }
        }

        private decimal AberturaDiaOpcoes(int IdCliente)
        {
            AcessaDados lAcessaDados = new AcessaDados();
            decimal SaldoAberturaD1 = 0;

            try
            {
                lAcessaDados.ConnectionStringName = "TRADE";

                using (DbCommand lDbCommand = lAcessaDados.CreateCommand(CommandType.StoredProcedure, "PRC_SALDOD1_ABERTURA"))
                {

                    lAcessaDados.AddInParameter(lDbCommand, "pCodCliente", DbType.Int32, IdCliente);


                    DataTable lDataTable = lAcessaDados.ExecuteOracleDataTable(lDbCommand);

                    if (lDataTable.Rows.Count > 0)
                    {
                        SaldoAberturaD1 = (lDataTable.Rows[0]["Abertura_D1"]).DBToDecimal();
                    }
                }

                return SaldoAberturaD1;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Ocorreu um erro ao acessar a Conta Corrente (D3)do cliente: {0} Erro: {1}", IdCliente.ToString(), ex.StackTrace));
            }
        }

        public ConcurrentDictionary<string, List<CustodiaInfo>> CarregarCustodiaBvsp()
        {
            try
            {
                ConcurrentDictionary<string, List<CustodiaInfo>> ret = new ConcurrentDictionary<string, List<CustodiaInfo>>();
                AcessaDados acesso = new AcessaDados("Retorno");
                acesso.ConnectionStringName = "TRADE";

                // Buscar Garantia de Bovespa
                using (DbCommand cmd = acesso.CreateCommand(CommandType.StoredProcedure, "PRC_OBTER_CUSTODIA_ABERTURA"))
                {
                    
                    DataTable table = acesso.ExecuteOracleDataTable(cmd);
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        DataRow dataRow = table.Rows[i];
                        string symb = dataRow["COD_NEG"].DBToString();
                        List<CustodiaInfo> lst =null;
                        if (!ret.TryGetValue(symb, out lst))
                        {
                            lst = new List<CustodiaInfo>();
                            ret.AddOrUpdate(symb, lst, (key, old) => lst);
                        }
                        CustodiaInfo cBvsp = new CustodiaInfo();
                        cBvsp.Account = dataRow["COD_CLI"].DBToInt32();
                        cBvsp.Symbol = symb;
                        cBvsp.Qty = dataRow["QTD"].DBToInt32();
                        lst.Add(cBvsp);
                    }
                }
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na consulta da posicao de custodia bovespa: " + ex.Message, ex);
                return null;
            }
        }

        public ConcurrentDictionary<string, List<CustodiaInfo>> CarregarCustodiaBmf()
        {
            try
            {
                ConcurrentDictionary<string, TmfSerieInfo> dicTmf = this.CarregarTmfSerie();

                ConcurrentDictionary<string, List<CustodiaInfo>> ret = new ConcurrentDictionary<string, List<CustodiaInfo>>();

                AcessaDados acesso = new AcessaDados("Retorno");
                
                acesso.ConnectionStringName = "TRADE";

                // Buscar Garantia de Bovespa
                using (DbCommand cmd = acesso.CreateCommand(CommandType.StoredProcedure, "PRC_OBTER_CUSTODIA_AB_BMF"))
                {
                    DataTable table = acesso.ExecuteOracleDataTable(cmd);

                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        DataRow dataRow = table.Rows[i];

                        string tpGrupo = dataRow["TIPO_GRUP"].DBToString();

                        string symb = string.Empty;

                        if (tpGrupo.Equals("BMF", StringComparison.CurrentCultureIgnoreCase))
                        {
                            symb = this._composeSymbol(dataRow, dicTmf);
                        }
                        else
                        {
                            symb = dataRow["COD_NEG"].DBToString();
                        }
                        
                        List<CustodiaInfo> lst = null;

                        if (!ret.TryGetValue(symb, out lst))
                        {
                            lst = new List<CustodiaInfo>();
                            ret.AddOrUpdate(symb, lst, (key, old)=> lst);
                        }

                        CustodiaInfo cBmf = new CustodiaInfo();
                        
                        cBmf.Account = dataRow["COD_CLI"].DBToInt32();
                        
                        cBmf.Symbol = symb;
                        
                        cBmf.Qty = Convert.ToInt32(dataRow["QTD"]);//.DBToInt32();
                        
                        lst.Add(cBmf);
                    }
                }
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na consulta da posicao de custodia bmf: " + ex.Message, ex);
                return null;
            }
        }

        //public ConcurrentDictionary<int, CustodiaInfo> CarregarCustodia(ConcurrentDictionary<string, SymbolInfo> papeis)
        //{
        //    try
        //    {
        //        ConcurrentDictionary<int, CustodiaInfo> ret = new ConcurrentDictionary<int, CustodiaInfo>();
        //        AcessaDados acesso = new AcessaDados("Retorno");
        //        acesso.ConnectionStringName = "TRADE";

        //        // Buscar Garantia de Bovespa
        //        using (DbCommand cmd = acesso.CreateCommand(CommandType.StoredProcedure, "PRC_OBTER_CUSTODIA_ABERTURA"))
        //        {
        //            int acc = 0;
        //            DataTable table = acesso.ExecuteOracleDataTable(cmd);
                    
        //            for (int i = 0; i < table.Rows.Count; i++)
        //            {
        //                DataRow dataRow = table.Rows[i];
        //                int currentAcc = dataRow["COD_CLI"].DBToInt32();

        //                if (acc != currentAcc)
        //                {
        //                    acc = currentAcc;
        //                    CustodiaInfo cInfo = new CustodiaInfo();
        //                    cInfo.CodCliente = acc;
        //                    ret.AddOrUpdate(acc, cInfo, (key, old) =>cInfo);
        //                }
        //                CustodiaInfo cItem = null;
        //                if (ret.TryGetValue(acc, out cItem))
        //                {
        //                    string symbol = dataRow["COD_NEG"].DBToString();
        //                    SymbolInfo ativo;
        //                    if (!papeis.TryGetValue(symbol, out ativo))
        //                    {
        //                        logger.Error("Problemas na consulta do ativo: " + symbol + ". Considerando valores zerados!!");
        //                        ativo = new SymbolInfo();
        //                    }
        //                    cItem.VlrCustodia += (dataRow["QTD"].DBToInt32() * ativo.VlrFechamento);
        //                }
        //            }
        //        }
        //        return ret;
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error("Problemas na consulta da posicao de conta corrente dos clientes: " + ex.Message, ex);
        //        return null;
        //    }
        //}

        public ConcurrentDictionary<int, TesouroDiretoAbInfo> CarregarTesouroDireto()
        {
            try
            {
                ConcurrentDictionary<int, TesouroDiretoAbInfo> ret = new ConcurrentDictionary<int, TesouroDiretoAbInfo>();
                AcessaDados acesso = new AcessaDados("Retorno");
                acesso.ConnectionStringName = "TRADE";

                // Buscar Garantia de Bovespa
                using (DbCommand cmd = acesso.CreateCommand(CommandType.StoredProcedure, "PRC_OBTER_CUSTODIA_AB_TEDI"))
                {
                    int acc = 0;
                    DataTable table = acesso.ExecuteOracleDataTable(cmd);

                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        DataRow dataRow = table.Rows[i];
                        int currentAcc = dataRow["COD_CLI"].DBToInt32();
                        if (acc != currentAcc)
                        {
                            acc = currentAcc;
                            TesouroDiretoAbInfo tediAbInfo = new TesouroDiretoAbInfo();
                            tediAbInfo.CodCliente = acc;
                            tediAbInfo.ValPosi += dataRow["VAL_POSI"].DBToDecimal();
                            ret.AddOrUpdate(acc, tediAbInfo, (key, old) => tediAbInfo);
                        }
                    }
                }
                return ret;
            }
            catch (Exception ex)
            {
                logger.Error("Problemas na consulta da posicao de custodia de abertura de tesouro direto: " + ex.Message, ex);
                return null;
            }
        }

        /// <summary>
        /// Método que busca no sinacor relação de cliente que operaram nos ultimos 2 minutos
        /// Procedure: PRC_REL_CLIENTE_ORDENS_2MIN
        /// </summary>
        /// <returns>Retorna uma listagem da relação de clientes que operaram nos ultimos 2 minutos</returns>
        public List<int> ObterClientesPosicaoDiaLimitada()
        {
            AcessaDados lAcessaDados = new AcessaDados();
            List<int> lstClientes = new List<int>();

            try
            {
                lAcessaDados.ConnectionStringName = "TRADE";

                using (DbCommand lDbCommand = lAcessaDados.CreateCommand(CommandType.StoredProcedure, "PRC_REL_CLIENTE_ORDENS_2MIN"))
                {
                    DataTable lDataTable = lAcessaDados.ExecuteOracleDataTable(lDbCommand);

                    if (null != lDataTable && lDataTable.Rows.Count > 0)
                    {
                        for (int i = 0; i <= lDataTable.Rows.Count - 1; i++)
                        {
                            int IdClient = (lDataTable.Rows[i]["cd_cliente"]).DBToInt32();
                            lock (lstClientes)
                            {
                                lstClientes.Add(IdClient);
                            }
                        }
                    }
                }

                return lstClientes;
            }
            catch (Exception ex)
            {
                logger.Info("Ocorreu um erro ao acessar o método ObterClientesPosicaoDia.", ex);
            }

            return lstClientes;
        }

        /// <summary>
        /// Método que busca no sinacor opção exercida do operador zero.
        /// Esse procedimento é necesspario pois na consulta que busca as operações não consegue pegar as 
        /// opperações de opções execídas do operador zero.
        /// E o setor de risco estava tendo carência de informções sobre este tipo de operação.
        /// Procedure: PRC_OBTER_OPCAOEXER_OP0_DIA
        /// </summary>
        /// <returns>Retorna as operações de opções exercidas com o código do operador zero 0</returns>
        public List<PosClientSymbolInfo> ObtemPosicaoOpcaoExercidaOperadorZero()
        {
            DateTime InitialDate = DateTime.Now;

            List<PosClientSymbolInfo> _ListaCliente = new List<PosClientSymbolInfo>();
            PosClientSymbolInfo _itemCliente;

            try
            {
                logger.InfoFormat("Verificando Opção exercida do Operador 0 (zero) dos clientes ");

                AcessaDados lAcessaDados = new AcessaDados();

                lAcessaDados.ConnectionStringName = "TRADE";

                using (DbCommand lDbCommand = lAcessaDados.CreateCommand(CommandType.StoredProcedure, "PRC_OBTER_OPCAOEXER_OP0_DIA"))
                {
                    DataTable lDataTable = lAcessaDados.ExecuteOracleDataTable(lDbCommand);

                    if (lDataTable.Rows.Count > 0)
                    {
                        for (int i = 0; i <= lDataTable.Rows.Count - 1; i++)
                        {
                            var lRow = lDataTable.Rows[i];

                            int lCodigoCliente  = lRow["cd_cliente"].DBToInt32();
                            string lAtivo       = lRow["cd_negocio"].DBToString();
                            string lSentido     = lRow["CD_NATOPE"].DBToString();

                            var lEncontrou = _ListaCliente.Find(pos => { return pos.Account == lCodigoCliente && pos.Ativo == lAtivo ; }); //verifica se há algum cliente já na lista com o novo papel

                            if (lEncontrou != null)
                            {
                                _ListaCliente.Remove(lEncontrou);

                                if (lSentido.Equals("C"))
                                {
                                    lEncontrou.QtdExecC += lRow["QT_NEGOCIO"].DBToDecimal();
                                    lEncontrou.PcMedC = ((lEncontrou.PcMedC + lRow["VL_NEGOCIO"].DBToDecimal())/ lEncontrou.QtdExecC);
                                    lEncontrou.VolCompra += lRow["VALOR"].DBToDecimal();
                                }
                                else if (lSentido.Equals("V"))
                                {
                                    lEncontrou.QtdExecV += lRow["QT_NEGOCIO"].DBToDecimal();
                                    lEncontrou.PcMedV = ((lEncontrou.PcMedV + lRow["VL_NEGOCIO"].DBToDecimal()) / lEncontrou.QtdExecV);
                                    lEncontrou.VolVenda += lRow["VALOR"].DBToDecimal();
                                }

                                lEncontrou.VolTotal = lEncontrou.VolVenda + lEncontrou.VolCompra;

                                _ListaCliente.Add(lEncontrou);

                                continue;
                            }

                            _itemCliente = new PosClientSymbolInfo();

                            _itemCliente.Ativo          = lAtivo;
                            _itemCliente.Account        = lCodigoCliente;
                            _itemCliente.NomeCliente    = lRow["NM_CLIENTE"].DBToString();
                            _itemCliente.CodPapelObjeto = lRow["CD_TITOBJ"].DBToString();
                            _itemCliente.Bolsa          = Exchange.Bovespa;
                            _itemCliente.TipoMercado    = lRow["TP_MERCADO"].DBToString();

                            if (lSentido.Equals("C"))
                            {
                                _itemCliente.PcMedC     = lRow["VL_NEGOCIO"].DBToDecimal();
                                _itemCliente.QtdExecC   = lRow["QT_NEGOCIO"].DBToInt32();
                                _itemCliente.VolCompra  = lRow["VALOR"].DBToDecimal();
                                //_itemCliente.LucroPrej = lRow["VALOR"].DBToDecimal(); O Lucro e prejuízo irá ser caluclado lá na frente, na classe de PositionClientManager no método de calculo do position Client
                            }
                            else
                            {
                                _itemCliente.PcMedV     = lRow["VL_NEGOCIO"].DBToDecimal();
                                _itemCliente.QtdExecV   = lRow["QT_NEGOCIO"].DBToInt32();
                                _itemCliente.VolVenda   = lRow["VALOR"].DBToDecimal();
                                //_itemCliente.LucroPrej  = -lRow["VALOR"].DBToDecimal(); ///A Venda é negativa no campos
                            }
                            /*
                            string InstrumentoAux = _itemCliente.Ativo;

                            if (_itemCliente.Ativo.Substring(_itemCliente.Ativo.Length - 1, 1) == "F")
                            {
                                InstrumentoAux = _itemCliente.Ativo.Remove(_itemCliente.Ativo.Length - 1);
                            }

                            _itemCliente.Ativo = InstrumentoAux;
                            */
                            _ListaCliente.Add(_itemCliente);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Info("Ocorreu um erro ao acessar o método ObtemPosicaoOpcaoExercidaOperadorZero");
                logger.Info("Descricao do erro: " + ex.Message);
                logger.Info("StackTrace do erro: " + ex.StackTrace);
            }

            return _ListaCliente;
        }
        #endregion
    }
}
