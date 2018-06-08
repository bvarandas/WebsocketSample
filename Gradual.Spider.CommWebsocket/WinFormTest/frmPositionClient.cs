using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Gradual.Spider.PositionClient.Monitor;
using Gradual.Spider.PositionClient.Monitor.Lib.Message;
using Gradual.Spider.PositionClient.Monitor.Lib.Dados;
using System.ServiceModel.Web;
using System.ServiceModel;
using Gradual.Spider.PositionClient.Monitor.Monitores.OperacoesIntraday;
using Gradual.Spider.PositionClient.Monitor.Monitores.RiscoResumidoIntranet;

namespace WinFormTest
{
    public partial class frmPositionClient : Form
    {
        private PositionClientMonitor _Servico = null; 
        private PositionClientMonitorRiscoResumido _ServicoConsolitedRisk = null;
        private MonitorRiscoResumidoIntranet _ServicoConsolidatedRiskIntranet = null;

        WebServiceHost selfHost = null;

        public frmPositionClient()
        {
            InitializeComponent();
        }

        private void btnInicia_Click(object sender, EventArgs e)
        {
            try
            {
                _Servico = new PositionClientMonitor();

                _Servico.IniciarServico();

                //selfHost = new WebServiceHost(typeof(RestOperacoesIntraday), new Uri("http://localhost:8000/Test"));
                
                //try
                //{
                //    // Step 2 Start the service.
                //    selfHost.Open();
                //    Console.WriteLine("The service is ready.");
                //    Console.WriteLine("Press <ENTER> to terminate service.");
                //    Console.WriteLine();
                //    Console.ReadLine();

                //    // Close the ServiceHostBase to shutdown the service.
                //    //selfHost.Close();
                //}
                //catch (CommunicationException ce)
                //{
                //    Console.WriteLine("An exception occurred: {0}", ce.Message);
                //    selfHost.Abort();
                //}
                

                MessageBox.Show("Serviço de Postion Client Iniciado com sucesso","Position Client", MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Position Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void btnFinaliza_Click(object sender, EventArgs e)
        {
            try
            {
                _Servico.PararServico();

                MessageBox.Show("Serviço de Postion Client Parado com sucesso", "Position Client", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Position Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmPositionClient_Load(object sender, EventArgs e)
        {
            _Servico = new PositionClientMonitor();
            //_ServicoConsolitedRisk = new PositionClientMonitorRiscoResumido();
        }

        private void frmPositionClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
        }

        private void btnEfetuaFiltroRiscoResumido_Click(object sender, EventArgs e)
        {
            var lRequest = new BuscarRiscoResumidoRequest();

            lRequest.OpcaoPrejuizoAtingido |= OpcaoPrejuizoAtingido.Entre20Ke50K;

            _ServicoConsolitedRisk.BuscarRiscoResumido(lRequest);
        }

        private void btnFiltroOperacaoIntraday_Click(object sender, EventArgs e)
        {
            try
            {
                var lRequest = new BuscarOperacoesIntradayRequest();
                  /*  
                lRequest.OpcaoMarket = OpcaoMarket.Opcoes;
                    
                lRequest.OpcaoMarket |= OpcaoMarket.Avista;
                    
                lRequest.OpcaoMarket |= OpcaoMarket.Futuros;
                */
                lRequest.CodigoCliente = 57128;

                lRequest.OpcaoParametrosIntraday |= OpcaoParametrosIntraday.OfertasPedra;

                _Servico.BuscarOperacoesIntraday(lRequest);
            }
            catch (Exception ex)
            {
                
            }
        }

        private void btnIniciaRiscoResumido_Click(object sender, EventArgs e)
        {
            try
            {
                _ServicoConsolitedRisk = new PositionClientMonitorRiscoResumido();

                _ServicoConsolitedRisk.IniciarServico();

                MessageBox.Show("Serviço de Consolited Risk Iniciado com sucesso", "Consolited Risk", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Consolited Risk", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnFinalizaRiscoResumido_Click(object sender, EventArgs e)
        {
            try
            {
                _ServicoConsolitedRisk.PararServico();

                MessageBox.Show("Serviço de Consolited Risk Parado com sucesso", "Consolited Risk", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Consolited Risk", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnIniciaRiscoResumidoIntranet_Click(object sender, EventArgs e)
        {
            try
            {
                _ServicoConsolidatedRiskIntranet = new MonitorRiscoResumidoIntranet();

                _ServicoConsolidatedRiskIntranet.IniciarServico();

                MessageBox.Show("Serviço de Consolited Risk intranet Iniciado com sucesso", "Consolited Risk Intranet", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Consolited Risk Intranet", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        
    }
}
