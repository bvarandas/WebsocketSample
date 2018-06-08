using Gradual.Spider.PositionClient.Monitor.Lib.Message;
using Gradual.Spider.SupervisorRisco.Lib.Dados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Gradual.Spider.PositionClient.Monitor.WebAPI.Controllers
{
    /// <summary>
    /// Classe de WebAPI para gerenciar as chamadas de position clientes
    /// </summary>
    public class PostionClientController : ApiController
    {
        // GET api/values
        public List<PosClientSymbolInfo> Get(BuscarOperacoesIntradayRequest pRequest)
        {
            BuscarOperacoesIntradayResponse lResponse = null;
            try
            {
                var lServico = PositionClientMonitor.Instance;

                lResponse = lServico.BuscarOperacoesIntraday(pRequest);
            }
            catch (Exception ex)
            {

                throw;
            }

            return lResponse.ListOperacoesIntraday;
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
