using Gradual.Spider.PositionClient.Monitor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Sockets;
using System.Collections.Generic;

namespace PositionClientTestProject
{
    
    
    /// <summary>
    ///This is a test class for PositionClientPackageSocketTest and is intended
    ///to contain all PositionClientPackageSocketTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PositionClientPackageSocketTest
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
        ///A test for PositionClientPackageSocket Constructor
        ///</summary>
        [TestMethod()]
        public void PositionClientPackageSocketConstructorTest()
        {
            PositionClientPackageSocket target = new PositionClientPackageSocket();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for ClientSocket_OnUnmappedObjectReceived
        ///</summary>
        [TestMethod()]
        public void ClientSocket_OnUnmappedObjectReceivedTest()
        {
            PositionClientPackageSocket target = new PositionClientPackageSocket(); // TODO: Initialize to an appropriate value
            object sender = null; // TODO: Initialize to an appropriate value
            int clientNumber = 0; // TODO: Initialize to an appropriate value
            Socket clientSock = null; // TODO: Initialize to an appropriate value
            Type objectType = null; // TODO: Initialize to an appropriate value
            object objeto = null; // TODO: Initialize to an appropriate value
            target.ClientSocket_OnUnmappedObjectReceived(sender, clientNumber, clientSock, objectType, objeto);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Dispose
        ///</summary>
        [TestMethod()]
        public void DisposeTest()
        {
            PositionClientPackageSocket target = new PositionClientPackageSocket(); // TODO: Initialize to an appropriate value
            target.Dispose();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for MonitorSonda
        ///</summary>
        [TestMethod()]
        public void MonitorSondaTest()
        {
            PositionClientPackageSocket target = new PositionClientPackageSocket(); // TODO: Initialize to an appropriate value
            target.MonitorSonda();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for OpenConnection
        ///</summary>
        [TestMethod()]
        public void OpenConnectionTest()
        {
            PositionClientPackageSocket target = new PositionClientPackageSocket(); // TODO: Initialize to an appropriate value
            target.OpenConnection();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for StartClientConnect
        ///</summary>
        [TestMethod()]
        public void StartClientConnectTest()
        {
            PositionClientPackageSocket target = new PositionClientPackageSocket(); // TODO: Initialize to an appropriate value
            List<int> pSubscriptions = null; // TODO: Initialize to an appropriate value
            target.StartClientConnect(pSubscriptions);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");

            //Assert.
        }

        /// <summary>
        ///A test for StopClientConnect
        ///</summary>
        [TestMethod()]
        public void StopClientConnectTest()
        {
            PositionClientPackageSocket target = new PositionClientPackageSocket(); // TODO: Initialize to an appropriate value
            target.StopClientConnect();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Port
        ///</summary>
        [TestMethod()]
        public void PortTest()
        {
            PositionClientPackageSocket target = new PositionClientPackageSocket(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Port = expected;
            actual = target.Port;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
