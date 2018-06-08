using Gradual.Spider.PositionClient.Monitor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Gradual.OMS.Library.Servicos;
using Gradual.Spider.PositionClient.Monitor.Lib.Message;
using System.Diagnostics;
using SuperSocket.SocketBase;

namespace PositionClientTestProject
{
    
    
    /// <summary>
    ///This is a test class for PositionClientMonitorTest and is intended
    ///to contain all PositionClientMonitorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PositionClientMonitorTest
    {
        #region Propriedades
        private PositionClientMonitor gMonitor;
        #endregion

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for PositionClientMonitor Constructor
        ///</summary>
        [TestMethod()]
        public void PositionClientMonitorConstructorTest()
        {
            PositionClientMonitor target = new PositionClientMonitor();
            //Assert.Inconclusive("TODO: Implement code to verify target");

            
        }

        [ClassInitialize]
        ///Deve ser estático
        public static void PreCondincaoClasse(TestContext context)
        {

        }

        //[DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV","|DataDirectory|\\somar.csv")]
        public void SomarTest()
        {

        }


        /// <summary>
        /// Método Inicialializador
        /// </summary>
        [TestInitialize]
        public void IniciarTestes()
        {
            gMonitor = new PositionClientMonitor();
        }
        
        /// <summary>
        /// Método Finalizador
        /// </summary>
        [TestCleanup]
        public void FinalizarTestes()
        {
            Debug.WriteLine("Teste Finalizados");
        }

        /// <summary>
        ///A test for IniciarServico
        ///</summary>
        [TestMethod()]
        public void IniciarServicoTest()
        {
            try
            {
                gMonitor.IniciarServico();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for PararServico
        ///</summary>
        [TestMethod()]
        public void PararServicoTest()
        {
            PositionClientMonitor target = new PositionClientMonitor(); // TODO: Initialize to an appropriate value
            target.PararServico();
            //Assert.Inconclusive("A method that does not return a value cannot be verified.");
            
        }

        /// <summary>
        ///A test for ReceberStatusServico
        ///</summary>
        [TestMethod()]
        public void ReceberStatusServicoTest()
        {
            PositionClientMonitor target = new PositionClientMonitor(); // TODO: Initialize to an appropriate value
            ServicoStatus expected = new ServicoStatus(); // TODO: Initialize to an appropriate value
            ServicoStatus actual;
            actual = target.ReceberStatusServico();
            Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ReturnIniciator
        ///</summary>
        [TestMethod()]
        public void ReturnIniciatorTest()
        {
            PositionClientMonitor target = new PositionClientMonitor(); // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.ReturnIniciator();
            Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SendMessageMemoryPositionClientToQueue
        ///</summary>
        [TestMethod()]
        [Priority(1)]
        [TestCategory("OperationPositionClient")]
        [Owner("Fulano")]
        [WorkItem(143)]
        [Description("Este teste faz alguma coisa....")]
        public void SendMessageMemoryPositionClientToQueueTest()
        {
            PositionClientMonitor target = new PositionClientMonitor(); // TODO: Initialize to an appropriate value
            object sender = this; // TODO: Initialize to an appropriate value
            MessagePositionClientArgs e = new MessagePositionClientArgs(); // TODO: Initialize to an appropriate value

            

            target.SendMessageMemoryPositionClientToQueue(sender, e);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");

            ///Testando método privado
            PrivateObject po = new PrivateObject(target);

            ///Chamando o método privado via reflection
            po.Invoke("SendMessagePositionClientToQueue", sender, e);

            //Testando métodos privados staticos reflection
            PrivateType pt = new PrivateType(typeof(PositionClientMonitor));

            ///Chamando o método privado estático
            pt.InvokeStatic("QueueSendMessagePositionClient");

            

        }

        /// <summary>
        ///A test for PositionClientMonitor Constructor
        ///</summary>
        [TestMethod()]
        public void PositionClientMonitorConstructorTest1()
        {
            PositionClientMonitor target = new PositionClientMonitor();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for PararServico
        ///</summary>
        [TestMethod()]
        public void PararServicoTest1()
        {
            PositionClientMonitor target = new PositionClientMonitor(); // TODO: Initialize to an appropriate value
            target.PararServico();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for ReceberStatusServico
        ///</summary>
        [TestMethod()]
        public void ReceberStatusServicoTest1()
        {
            PositionClientMonitor target = new PositionClientMonitor(); // TODO: Initialize to an appropriate value
            ServicoStatus expected = new ServicoStatus(); // TODO: Initialize to an appropriate value
            ServicoStatus actual;
            actual = target.ReceberStatusServico();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ReturnIniciator
        ///</summary>
        [TestMethod()]
        public void ReturnIniciatorTest1()
        {
            PositionClientMonitor target = new PositionClientMonitor(); // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.ReturnIniciator();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SendMessageMemoryPositionClientToQueue
        ///</summary>
        [TestMethod()]
        public void SendMessageMemoryPositionClientToQueueTest1()
        {
            PositionClientMonitor target = new PositionClientMonitor(); // TODO: Initialize to an appropriate value
            object sender = null; // TODO: Initialize to an appropriate value
            MessagePositionClientArgs e = null; // TODO: Initialize to an appropriate value
            target.SendMessageMemoryPositionClientToQueue(sender, e);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Bootstrap
        ///</summary>
        [TestMethod()]
        public void BootstrapTest()
        {
            PositionClientMonitor target = new PositionClientMonitor(); // TODO: Initialize to an appropriate value
            IBootstrap actual;
            actual = target.Bootstrap;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for PositionClientMonitor Constructor
        ///</summary>
        [TestMethod()]
        public void PositionClientMonitorConstructorTest2()
        {
            PositionClientMonitor target = new PositionClientMonitor();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for IniciarServico
        ///</summary>
        [TestMethod()]
        public void IniciarServicoTest2()
        {
            PositionClientMonitor target = new PositionClientMonitor(); // TODO: Initialize to an appropriate value
            target.IniciarServico();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for PararServico
        ///</summary>
        [TestMethod()]
        public void PararServicoTest2()
        {
            PositionClientMonitor target = new PositionClientMonitor(); // TODO: Initialize to an appropriate value
            target.PararServico();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for ReceberStatusServico
        ///</summary>
        [TestMethod()]
        public void ReceberStatusServicoTest2()
        {
            PositionClientMonitor target = new PositionClientMonitor(); // TODO: Initialize to an appropriate value
            ServicoStatus expected = new ServicoStatus(); // TODO: Initialize to an appropriate value
            ServicoStatus actual;
            actual = target.ReceberStatusServico();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ReturnIniciator
        ///</summary>
        [TestMethod()]
        public void ReturnIniciatorTest2()
        {
            PositionClientMonitor target = new PositionClientMonitor(); // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.ReturnIniciator();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SendMessageMemoryPositionClientToQueue
        ///</summary>
        [TestMethod()]
        public void SendMessageMemoryPositionClientToQueueTest2()
        {
            PositionClientMonitor target = new PositionClientMonitor(); // TODO: Initialize to an appropriate value
            object sender = null; // TODO: Initialize to an appropriate value
            MessagePositionClientArgs e = null; // TODO: Initialize to an appropriate value
            target.SendMessageMemoryPositionClientToQueue(sender, e);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Bootstrap
        ///</summary>
        [TestMethod()]
        //[WorkItem()]
        public void BootstrapTest1()
        {
            PositionClientMonitor target = new PositionClientMonitor(); // TODO: Initialize to an appropriate value
            IBootstrap actual;
            actual = target.Bootstrap;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
        
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", 
            "<Your file path >\\data.csv", 
            "<File Name >#csv", 
            DataAccessMethod.Sequential), 
        DeploymentItem("<File Name>.csv"), 
        TestMethod]
        public void ConnectDataCvsFile()
        {
            ///TestContext é a propriedade da classe de teste onde está sendo rodado o método
            int lValor1 = Convert.ToInt32(TestContext.DataRow["Valor1"]);
            int lValor2 = Convert.ToInt32(TestContext.DataRow["Valor2"]);

            PrivateObject lObject = new PrivateObject(gMonitor);

            double lExpected = (int)TestContext.DataRow["Valor2"];
            double lActual = (int)lObject.Invoke("Somar", lValor1, lValor2);

            Assert.AreEqual(lExpected, lActual);
        }

        [TestMethod]
        [DataSource("System.Data.SqlClient",
            "Data Source=10.11.12.28;User Id=directtrade;Password=directtrade!1985;Initial Catalog=GradualSpider2;",
            //"Data Source=\\153.71.88.80;Database=LoadData_For_R10Core_10_5_Final_Extended;Integrated Security=True;Connect Timeout=30;User Instance=True", 
            "Products", 
            DataAccessMethod.Sequential)]
        public void ConnectDataSQL()
        {
        }

        /// <summary>
        ///A test for SendMessageMemoryPositionClientToQueue
        ///</summary>
        [TestMethod()]
        public void SendMessageMemoryPositionClientToQueueTest3()
        {
            PositionClientMonitor target = new PositionClientMonitor(); // TODO: Initialize to an appropriate value
            object sender = null; // TODO: Initialize to an appropriate value
            MessagePositionClientArgs e = null; // TODO: Initialize to an appropriate value
            target.SendMessageMemoryPositionClientToQueue(sender, e);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
    }
}
