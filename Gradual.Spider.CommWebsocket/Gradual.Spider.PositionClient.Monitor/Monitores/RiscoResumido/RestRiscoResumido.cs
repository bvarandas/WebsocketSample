using System;
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
using Gradual.Spider.PositionClient.Monitor.Lib.Message;
using log4net;

namespace Gradual.Spider.PositionClient.Monitor.Monitores.RiscoResumido
{
    /// <summary>
    /// Serviço de REST para Buscar operações Intraday na memória e retornar um json
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class RestRiscoResumido : IServicoRestRiscoResumido
    {
        /// <summary>
        /// Atributo responsável pela log da classe
        /// </summary>
        private static readonly log4net.ILog _Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// para Buscar Risco Resumido na memória e retornar um json
        /// </summary>
        /// <param name="CodigoCliente">Código do Cliente</param>
        /// <param name="OpcaoPLSomenteComLucro">Opção de somente com lucro</param>
        /// <param name="OpcaoPLSomentePLnegativo">Opção de somente de PL negativo</param>
        /// <param name="OpcaoSFPAtingidoAte25">Opção de SFP Atingido até 25%</param>
        /// <param name="OpcaoSFPAtingidoEntre25e50">Opção SFP Atingido Entre 25% e 50% </param>
        /// <param name="OpcaoSFPAtingidoEntre50e75">Opção SFP Atingido Entre 50% e 75%</param>
        /// <param name="OpcaoSFPAtingidoAcima75">Opção SFP Atingido Acima 75%</param>
        /// <param name="OpcaoPrejuizoAtingidoAte2K">Opção Prejuízo Atingido até 2.000</param>
        /// <param name="OpcaoPrejuizoAtingidoEntre2Ke5K">Opção Prejuízo Atingido Entre 2.000 e 5.000</param>
        /// <param name="OpcaoPrejuizoAtingidoEntre5Ke10K">Opção Prejuízo Atingido Entre 5.000 e 10.000</param>
        /// <param name="OpcaoPrejuizoAtingidoEntre10Ke20K">Opção Prejuízo Atingido Entre 10.000 e 20.000</param>
        /// <param name="OpcaoPrejuizoAtingidoEntre20Ke50K">Opção Prejuízo Atingido Entre 20.000 e 50.000</param>
        /// <param name="OpcaoPrejuizoAtingidoAcima50K">Opção Prejuízo Atingido Acima 50.000</param>
        /// <returns>String em Json com os dados de Risco Resumido</returns>
        public string BuscarRiscoResumidoJSON(string pRequestJson)
        {
            string lRetorno = string.Empty;

            try
            {
                
                var lList = new List<ConsolidatedRiskInfo>();

                var lDic = new ConcurrentDictionary<int, ConsolidatedRiskInfo>();

                lList.AddRange(PositionClientSocketRiscoResumido.Instance.DicConsolidatedRisk.Values);

                var pRequest = JsonConvert.DeserializeObject(pRequestJson, typeof(BuscarRiscoResumidoRESTRequest)) as BuscarRiscoResumidoRESTRequest;

                var lFiltradoLista = from a in lList select a;

                var lFiltroLucroPrejuizo = new List<ConsolidatedRiskInfo>();

                if (pRequest.ListaClientes.Count() > 0)
                {
                    lFiltradoLista = from a in lFiltradoLista where pRequest.ListaClientes.Contains(a.Account) select a;

                    lFiltroLucroPrejuizo.AddRange(lFiltradoLista);
                }

                //Opção de PL negativo ou com lucro
                if (pRequest.OpcaoPLSomenteComLucro)
                {
                    lFiltradoLista = from a in lFiltradoLista where a.PLTotal > 0 select a;

                    lFiltroLucroPrejuizo = lFiltradoLista.ToList();
                }
                else if (pRequest.OpcaoPLSomentePLnegativo)
                {
                    lFiltradoLista = from a in lFiltradoLista where a.PLTotal < 0 select a;

                    lFiltroLucroPrejuizo = lFiltradoLista.ToList();
                }

                //Opção de SFP Atingido
                if (pRequest.OpcaoSFPAtingidoAte25)
                {
                    var lFiltrado25 = from a in lFiltradoLista where (a.TotalPercentualAtingido > (-25) && a.TotalPercentualAtingido < 0) select a;

                    lFiltroLucroPrejuizo.AddRange(lFiltrado25.ToList());
                }

                if (pRequest.OpcaoSFPAtingidoEntre25e50)
                {
                    var lFiltrado25e50 = from a in lFiltradoLista where (a.TotalPercentualAtingido < (-25) && a.TotalPercentualAtingido > (-50)) select a;

                    lFiltroLucroPrejuizo.AddRange(lFiltrado25e50.ToList());
                }

                if (pRequest.OpcaoSFPAtingidoEntre50e75)
                {
                    var lFiltrado50e75 = from a in lFiltradoLista where (a.TotalPercentualAtingido < (-50) && a.TotalPercentualAtingido > (-75)) select a;

                    lFiltroLucroPrejuizo.AddRange(lFiltrado50e75.ToList());
                }

                if (pRequest.OpcaoSFPAtingidoAcima75)
                {
                    var lFiltrado75 = from a in lFiltradoLista where (a.TotalPercentualAtingido < (-75)) select a;

                    lFiltroLucroPrejuizo.AddRange(lFiltrado75.ToList());
                }

                lFiltradoLista = lFiltroLucroPrejuizo;

                //Prejuízo Atingido
                if (pRequest.OpcaoPrejuizoAtingidoAte2K)
                {
                    lFiltradoLista = from a in lFiltradoLista where (a.PLTotal >= (-2000) && a.PLTotal < 0) select a;
                }

                if (pRequest.OpcaoPrejuizoAtingidoEntre2Ke5K)
                {
                    lFiltradoLista = from a in lFiltradoLista where (a.PLTotal <= (-2000) && a.PLTotal >= (-5000)) select a;
                }

                if (pRequest.OpcaoPrejuizoAtingidoEntre5Ke10K)
                {
                    lFiltradoLista = from a in lFiltradoLista where (a.PLTotal <= (-5000) && a.PLTotal >= (-10000)) select a;
                }

                if (pRequest.OpcaoPrejuizoAtingidoEntre10Ke20K)
                {
                    lFiltradoLista = from a in lFiltradoLista where (a.PLTotal <= (-10000) && a.PLTotal >= (-20000)) select a;
                }

                if (pRequest.OpcaoPrejuizoAtingidoEntre20Ke50K)
                {
                    lFiltradoLista = from a in lFiltradoLista where (a.PLTotal <= (-20000) && a.PLTotal >= (-50000)) select a;
                }

                if (pRequest.OpcaoPrejuizoAtingidoAcima50K)
                {
                    lFiltradoLista = from a in lFiltradoLista where (a.PLTotal <= (-50000)) select a;
                }

                var lTrans = new TransporteRiscoResumido(lFiltradoLista.ToList());

                lRetorno = JsonConvert.SerializeObject(lTrans.ListaTransporte);

                //_Logger.InfoFormat("Foram Encontrados {0} itens de Risco Resumido", lRetorno.ListRiscoResumido.Count);
            }
            catch (Exception ex)
            {
                _Logger.Error("Erro encontrado  no método BuscarRiscoResumidoJSON", ex);
            }

            return lRetorno;
        }

        /// <summary>
        /// para Buscar Risco Resumido na memória e retornar um json
        /// </summary>
        /// <param name="CodigoCliente">Código do Cliente</param>
        /// <returns>String em Json com os dados de Risco Resumido</returns>
        public string BuscarRiscoResumidoIntranetJSON(string pRequestJson)
        {
            string lRetorno = string.Empty;

            try
            {

                var lList = new List<ConsolidatedRiskInfo>();

                var lDic = new ConcurrentDictionary<int, ConsolidatedRiskInfo>();

                lList.AddRange(PositionClientSocketRiscoResumido.Instance.DicConsolidatedRisk.Values);

                var pRequest = JsonConvert.DeserializeObject(pRequestJson, typeof(BuscarRiscoResumidoIntranetRESTRequest)) as BuscarRiscoResumidoIntranetRESTRequest;

                var lFiltradoLista = from a in lList select a;

                if (pRequest.CodigoCliente != 0)
                {
                    lFiltradoLista = from a in lFiltradoLista where a.Account == pRequest.CodigoCliente select a;
                }

                if (pRequest.CodigoAssessor != 0)
                {
                    lFiltradoLista = from a in lFiltradoLista where a.CodigoAssessor == pRequest.CodigoAssessor select a;
                }

                if (pRequest.OpcaoExcluirClientesZerados)
                {
                    lFiltradoLista = from a in lFiltradoLista where a.Zerado == false select a;
                }

                if (pRequest.OpcaoExcluirNaoOperaramIntraday)
                {
                    lFiltradoLista = from a in lFiltradoLista where a.OperouIntraday == true select a;
                }

                if (pRequest.OpcaoSemaforoVerde)
                {
                    lFiltradoLista= from a in lFiltradoLista where (a.TotalPercentualAtingido >= (-20) && a.TotalPercentualAtingido < 0) select a;
                }

                if (pRequest.OpcaoSemaforoAmarelo)
                {
                    lFiltradoLista  = from a in lFiltradoLista where (a.TotalPercentualAtingido <= (-20) && a.TotalPercentualAtingido >= (-70)) select a;
                }

                if (pRequest.OpcaoSemaforoVermelho)
                {
                    lFiltradoLista = from a in lFiltradoLista where (a.TotalPercentualAtingido <= (-70)) select a;
                }

                //Prejuízo Atingido
                if (pRequest.OpcaoPrejuizoMenor2K)
                {
                    lFiltradoLista = from a in lFiltradoLista where (a.PLTotal >= (-2000) && a.PLTotal < 0) select a;
                }

                if (pRequest.OpcaoPrejuizoMaior2kMenor5k)
                {
                    lFiltradoLista = from a in lFiltradoLista where (a.PLTotal <= (-2000) && a.PLTotal >= (-5000)) select a;
                }

                if (pRequest.OpcaoPrejuizoMaior5kMenor10k)
                {
                    lFiltradoLista = from a in lFiltradoLista where (a.PLTotal <= (-5000) && a.PLTotal >= (-10000)) select a;
                }

                if (pRequest.OpcaoPrejuizoMaior10kMenor15k)
                {
                    lFiltradoLista = from a in lFiltradoLista where (a.PLTotal <= (-10000) && a.PLTotal >= (-15000)) select a;
                }

                if (pRequest.OpcaoPrejuizoMaior15kMenor20k)
                {
                    lFiltradoLista = from a in lFiltradoLista where (a.PLTotal <= (-15000) && a.PLTotal >= (-20000)) select a;
                }

                if (pRequest.OpcaoPrejuizoMaior20k)
                {
                    lFiltradoLista = from a in lFiltradoLista where (a.PLTotal <= (-20000)) select a;
                }

                var lTrans = new TransporteRiscoResumido(lFiltradoLista.ToList());

                lRetorno = JsonConvert.SerializeObject(lTrans.ListaTransporte);

                //_Logger.InfoFormat("Foram Encontrados {0} itens de Risco Resumido", lRetorno.ListRiscoResumido.Count);
            }
            catch (Exception ex)
            {
                _Logger.Error("Erro encontrado  no método BuscarRiscoResumidoJSON", ex);
            }

            return lRetorno;
        }

    }
}
