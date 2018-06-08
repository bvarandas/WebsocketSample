using Gradual.Spider.PositionClient.Monitor.Lib;
using Gradual.Spider.PositionClient.Monitor.Lib.Message;
using Gradual.Spider.PositionClient.Monitor.Monitores.RiscoResumido;
using Gradual.Spider.PositionClient.Monitor.Transporte;
using Gradual.Spider.SupervisorRisco.Lib.Dados;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;

namespace Gradual.Spider.PositionClient.Monitor.Monitores.RiscoResumidoIntranet
{
    /// <summary>
    /// Serviço de REST para Buscar operações Intraday na memória e retornar um json
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class RestRiscoResumidoIntranet : IServicoRiscoResumidoIntranet
    {
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

                var lFiltroLucroPrejuizo = new List<ConsolidatedRiskInfo>();

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
                //_Logger.Error("Erro encontrado no método de ", ex);
            }

            return lRetorno;
        }
    }
}
