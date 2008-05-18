using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LinqToRdf;

namespace UnitTests
{
    /// <summary>
    /// Summary description for TestExtensions
    /// </summary>
    [TestClass]
    public class TestExtensions
    {
        public TestExtensions()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        [TestMethod]
        public void TestGetPrefix()
        {
            Assert.AreEqual("xsdt", AttributeExtensions.GetOntologyPrefix("Data Types"));
        }
    }
}
