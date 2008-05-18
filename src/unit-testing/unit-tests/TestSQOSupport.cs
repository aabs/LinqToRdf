using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdfMusic;

namespace UnitTests
{
    /// <summary>
    /// Summary description for TestSQOSupport
    /// </summary>
    [TestClass]
    public class TestSQOSupport
    {
        public TestSQOSupport()
        {
            //
            // TODO: Add constructor logic here
            //
        }

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
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestFirst()
        {
            var ctx = new MusicDataContext(@"http://localhost/linqtordf/SparqlQuery.aspx");
            var albums = (from a in ctx.Albums
                          where a.Name.StartsWith("Thomas")
                          select a);
            Assert.IsTrue(albums.First() != null);
        }

        [TestMethod]
        public void TestFirstOrDefault()
        {
            var ctx = new MusicDataContext(@"http://localhost/linqtordf/SparqlQuery.aspx");
            var albums = (from a in ctx.Albums
                          where a.Name.StartsWith("Thomas")
                          select a);
            Assert.IsTrue(albums.FirstOrDefault() != null);
        }

        [TestMethod]
        public void TestCount()
        {
            var ctx = new MusicDataContext(@"http://localhost/linqtordf/SparqlQuery.aspx");
            var albums = (from a in ctx.Albums
                          where a.Name.StartsWith("Thomas")
                          select a);
            Assert.IsTrue(albums.Count() == 1);

        }

        [TestMethod]
        public void TestSkip()
        {
            var ctx = new MusicDataContext(@"http://localhost/linqtordf/SparqlQuery.aspx");
            var albums = (from a in ctx.Albums
                          where a.Name.StartsWith("Thomas")
                          select a);
            Assert.IsTrue(albums.Count() == 1);

        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void TestSum()
        {
            var ctx = new MusicDataContext(@"http://localhost/linqtordf/SparqlQuery.aspx");
            var albums = (from a in ctx.Albums
                          where a.Name.StartsWith("Thomas")
                          select a);
            Assert.IsTrue(albums.Sum(a=>1) == 1);
        }

    }
}
