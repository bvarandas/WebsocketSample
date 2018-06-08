using Gradual.Spider.PositionClient.Monitor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Gradual.OMS.Library.Servicos;
using Gradual.Spider.PositionClient.Monitor.Lib.Message;

namespace UnitTestProject1
{
    
    
    /// <summary>
    ///This is a test class for PositionClientMonitorTest and is intended
    ///to contain all PositionClientMonitorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PositionClientMonitorTest
    {


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
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for IniciarServico
        ///</summary>
        [TestMethod()]
        public void IniciarServicoTest()
        {
            PositionClientMonitor target = new PositionClientMonitor(); // TODO: Initialize to an appropriate value
            target.IniciarServico();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for PararServico
        ///</summary>
        [TestMethod()]
        public void PararServicoTest()
        {
            PositionClientMonitor target = new PositionClientMonitor(); // TODO: Initialize to an appropriate value
            target.PararServico();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
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
            Assert.Inconclusive("Verify the correctness of this test method.");
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
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SendMessageMemoryPositionClientToQueue
        ///</summary>
        [TestMethod()]
        public void SendMessageMemoryPositionClientToQueueTest()
        {
            PositionClientMonitor target = new PositionClientMonitor(); // TODO: Initialize to an appropriate value
            object sender = null; // TODO: Initialize to an appropriate value
            MessagePositionClientArgs e = null; // TODO: Initialize to an appropriate value
            target.SendMessageMemoryPositionClientToQueue(sender, e);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
    }
}
