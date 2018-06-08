using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.PositionClient.Monitor.Lib.Message
{
    /// <summary>
    /// Classe de request do REST para efetuar o filtro 
    /// da descida dos objeto no socket da aplicação conectada
    /// </summary>
    public class BuscarRiscoResumidoIntranetRESTRequest
    {
        /// <summary>
        /// Codigo Cliente
        /// </summary>
        public int CodigoCliente { get; set; }

        /// <summary>
        /// Codigo Assessor
        /// </summary>
        public int CodigoAssessor { get; set; }

        /// <summary>
        /// Opção de Prejuizo - Sem Informação
        /// </summary>
        public bool OpcaoPrejuizoSemInformacao { get; set; }

        /// <summary>
        /// OPção de Prejuizo - Menor que 2K
        /// </summary>
        public bool OpcaoPrejuizoMenor2K { get; set; }

        /// <summary>
        /// Opção de Prejuizo - Maior que 2k e menor que 5k
        /// </summary>
        public bool OpcaoPrejuizoMaior2kMenor5k { get; set; }

        /// <summary>
        /// Opção de Prejuizo - Maior que 5k e menor 10k
        /// </summary>
        public bool OpcaoPrejuizoMaior5kMenor10k { get; set; }

        /// <summary>
        /// Opção de Prejuizo - Maior que 10k e menor que 15k
        /// </summary>
        public bool OpcaoPrejuizoMaior10kMenor15k { get; set; }

        /// <summary>
        /// Opção de Prejuizo - Maior 15k e menor que 20k
        /// </summary>
        public bool OpcaoPrejuizoMaior15kMenor20k { get; set; }

        /// <summary>
        /// Opção de Prejuizo Maior até 20k
        /// </summary>
        public bool OpcaoPrejuizoMaior20k { get; set; }

        /// <summary>
        /// Opcao de semaforo - Sem Informação
        /// </summary>
        public bool OpcaoSemaforoSemInformacao { get; set; }

        /// <summary>
        /// Opção de semaforo - Vermelho
        /// </summary>
        public bool OpcaoSemaforoVermelho { get; set; }

        /// <summary>
        /// Opção de semaforo - Amarelo
        /// </summary>
        public bool OpcaoSemaforoAmarelo { get; set; }

        /// <summary>
        /// Opção de semaforo - Verde
        /// </summary>
        public bool OpcaoSemaforoVerde { get; set; }

        /// <summary>
        /// Opção que Excuir Clientes Zerados
        /// </summary>
        public bool OpcaoExcluirClientesZerados { get; set; }

        /// <summary>
        /// Opção que Excuir Clientes que não operaram
        /// </summary>
        public bool OpcaoExcluirNaoOperaramIntraday { get; set; }
    }
}
