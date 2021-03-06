﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;
using Gradual.Spider.PositionClient.Monitor.Lib;
using Gradual.OMS.Library.Servicos;
using Gradual.OMS.WebSocket.Lib;
using Newtonsoft.Json;
using Gradual.Spider.SupervisorRisco.Lib.Dados;
using System.Collections.Concurrent;
using Gradual.Spider.PositionClient.Monitor.Transporte;
using System.Web;
using log4net;



namespace Gradual.Spider.PositionClient.Monitor.Monitores.OperacoesIntraday
{
    /// <summary>
    /// Serviço de REST para Buscar operações Intraday na memória e retornar um json
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class RestOperacoesIntraday : IServicoRestOperacoesIntraday
    {
        /// <summary>
        /// Atributo responsável pela log da classe
        /// </summary>
        private static readonly log4net.ILog _Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// para Buscar operações Intraday na memória e retornar um json
        /// </summary>
        /// <returns>Retorna um json da</returns>
        public string BuscarOperacoesIntradayJSON(
            int CodigoCliente,
            string CodigoInstrumento,
            bool OpcaoMarketTodosMercados,
            bool OpcaoMarketAVista,
            bool OpcaoMarketFuturos,
            bool OpcaoMarketOpcao,
            bool OpcaoParametroIntradayOfertasPedra,
            bool OpcaoParametroIntradayNetNegativo,
            bool OpcaoParametroIntradayNetPositivo,
            bool OpcaoParametroIntradayPLNegativo
            )
        {
            string lRetorno = string.Empty;

            try
            {
                var lList = new List<PosClientSymbolInfo>();

                var lListTrans = new List<TransporteOperacoesIntraday>();

                var lDic = new ConcurrentDictionary<int, List<PosClientSymbolInfo>>();

                var lPos = PositionClientPackageSocket.Instance.DicPositionClient;
                
                var lTiposMercado = new List<string> { "vis", "fut", "opf", "dis", "opc", "opv" };

                var lTiposMercadoBmf = new List<string> {"fut", "opf", "dis"};

                var lTiposMercadoBovespa = new List<string> { "vis", "opc", "opv"};

                lock (lPos)
                {
                    foreach (KeyValuePair<int, List<PosClientSymbolInfo>> pos in lPos)
                    {
                        lList.AddRange(pos.Value);
                    }
                }

                var lFiltrado = from a in lList 
                                where 
                                (
                                ((a.QtdExecC != 0 || a.QtdExecV != 0 || a.QtdAbC != 0 || a.QtdAbV != 0 || a.QtdAbertura != 0)) &&
                                ( lTiposMercadoBovespa .Contains( a.TipoMercado.ToLower() ))
                                ) 
                                ||lTiposMercadoBmf.Contains(a.TipoMercado.ToLower())
                                select a;

                ///Filtrando Cliente
                if ( CodigoCliente != 0 )
                {
                    lFiltrado = from a in lFiltrado where a.Account == CodigoCliente select a;
                }

                ///Filtrando Papel
                if (!string.IsNullOrEmpty(CodigoInstrumento))
                {
                    lFiltrado = from a in lFiltrado where a.Ativo == CodigoInstrumento select a;
                }
                
                if (!OpcaoMarketTodosMercados)
                {
                    if (!OpcaoMarketAVista)
                    {
                        lTiposMercado.Remove("vis");
                    }

                    if (!OpcaoMarketFuturos)
                    {
                        lTiposMercado.Remove("fut");
                        lTiposMercado.Remove("opf");
                        lTiposMercado.Remove("dis");
                    }

                    if (!OpcaoMarketOpcao)
                    {
                        lTiposMercado.Remove("opc");
                        lTiposMercado.Remove("opv");
                    }

                    if (OpcaoMarketAVista || OpcaoMarketFuturos || OpcaoMarketOpcao)
                    {
                        lFiltrado = from a in lFiltrado where lTiposMercado.Contains(a.TipoMercado.ToLower()) select a;
                    }
                }
                
                //Opção de Parametros Intraday
                if (OpcaoParametroIntradayNetNegativo)
                {
                    lFiltrado = from a in lFiltrado where a.NetExec < 0 select a;
                }

                if (OpcaoParametroIntradayNetPositivo)
                {
                    lFiltrado = from a in lFiltrado where a.NetExec > 0 select a;
                }

                if (OpcaoParametroIntradayOfertasPedra)
                {
                    lFiltrado = from a in lFiltrado where a.NetAb > 0 select a;
                }

                if (OpcaoParametroIntradayPLNegativo)
                {
                    lFiltrado = from a in lFiltrado where a.LucroPrej < 0 select a;
                }
                
                var lTrans = new TransporteOperacoesIntraday(lFiltrado.ToList());

                
                lRetorno = JsonConvert.SerializeObject(lTrans.ListaTransporte);
            }
            catch (Exception ex)
            {
                _Logger.Error("Erro encontrado  no método BuscarOperacoesIntradayJSON", ex);
            }

            return lRetorno;
        }

        public string BuscarOperacoesIntradayIntranetJSON(int CodigoCliente)
        {
            string lRetorno = string.Empty;

            try
            {
                var lList = new List<PosClientSymbolInfo>();

                var lListTrans = new List<TransporteOperacoesIntraday>();

                var lDic = new ConcurrentDictionary<int, List<PosClientSymbolInfo>>();

                var lPos = PositionClientPackageSocket.Instance.DicPositionClient;
                
                lock (lPos)
                {
                    foreach (KeyValuePair<int, List<PosClientSymbolInfo>> pos in lPos)
                    {
                        lList.AddRange(pos.Value);
                    }
                }

                var lFiltrado = from a in lList select a;

                ///Filtrando Cliente
                if (CodigoCliente != 0)
                {
                    lFiltrado = from a in lFiltrado where a.Account == CodigoCliente select a;
                }

                lFiltrado = from a in lFiltrado where (a.QtdExecC != 0 || a.QtdExecV != 0 ||  a.QtdAbertura != 0) select a;

                var lTrans = new TransporteOperacoesIntraday(lFiltrado.ToList());

                lRetorno = JsonConvert.SerializeObject(lTrans.ListaTransporte);
            }
            catch (Exception ex)
            {
                _Logger.Error("Erro encontrado  no método BuscarOperacoesIntradayIntranetJSON", ex);
            }

            return lRetorno;
        }

        
    }
}
