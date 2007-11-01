using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LinqToRdf;

namespace RdfSerialisationTest
{
    /// <summary>
    /// Summary description for TestTypeTranslator2
    /// </summary>
    [TestClass]
    public class TestTypeTranslator2
    {
        public TestTypeTranslator2()
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
        public void TestFactoryWorks()
        {
            ITypeTranslator2 tt1 = TypeTranslationProvider.GetTypeTranslator(SupportedTypeDomain.DotNet);
            Assert.IsNotNull(tt1);
            Assert.IsInstanceOfType(tt1, typeof(DotnetTypeTranslator));
            ITypeTranslator2 tt2 = TypeTranslationProvider.GetTypeTranslator(SupportedTypeDomain.XmlSchemaDatatypes);
            Assert.IsNotNull(tt2);
            Assert.IsInstanceOfType(tt2, typeof(XsdtTypeTranslator));
            Assert.AreNotEqual(tt1, tt2);
        }

        [TestMethod]
        public void TestDotNetTypeTranslator()
        {
            ITypeTranslator2 tt1 = TypeTranslationProvider.GetTypeTranslator(SupportedTypeDomain.DotNet);
            Assert.AreEqual<string>("short", tt1[PrimitiveDataType.SHORT]);
            Assert.AreEqual<string>("int", tt1[PrimitiveDataType.INT]);
            Assert.AreEqual<string>("long", tt1[PrimitiveDataType.LONG]);
            Assert.AreEqual<string>("float", tt1[PrimitiveDataType.FLOAT]);
            Assert.AreEqual<string>("double", tt1[PrimitiveDataType.DOUBLE]);
            Assert.AreEqual<string>("decimal", tt1[PrimitiveDataType.DECIMAL]);
            Assert.AreEqual<string>("bool", tt1[PrimitiveDataType.BOOLEAN]);
            Assert.AreEqual<string>("string", tt1[PrimitiveDataType.STRING]);
            Assert.AreEqual<string>("byte[]", tt1[PrimitiveDataType.HEXBINARY]);
            Assert.AreEqual<string>("char[]", tt1[PrimitiveDataType.BASE64BINARY]);
            Assert.AreEqual<string>("TimeSpan", tt1[PrimitiveDataType.DURATION]);
            Assert.AreEqual<string>("DateTime", tt1[PrimitiveDataType.DATETIME]);
            Assert.AreEqual<string>("DateTime", tt1[PrimitiveDataType.TIME]);
            Assert.AreEqual<string>("DateTime", tt1[PrimitiveDataType.DATE]);
            Assert.AreEqual<string>("DateTime", tt1[PrimitiveDataType.GYEARMONTH]);
            Assert.AreEqual<string>("uint", tt1[PrimitiveDataType.GYEAR]);
            Assert.AreEqual<string>("DateTime", tt1[PrimitiveDataType.GMONTHDAY]);
            Assert.AreEqual<string>("uint", tt1[PrimitiveDataType.GDAY]);
            Assert.AreEqual<string>("uint", tt1[PrimitiveDataType.GMONTH]);
            Assert.AreEqual<string>("string", tt1[PrimitiveDataType.ANYURI]);
            Assert.AreEqual<string>("string", tt1[PrimitiveDataType.QNAME]);
        }
        [TestMethod, ExpectedException(typeof(NotImplementedException))]
        public void TestDotNetTypeTranslatorThrowsOk()
        {
            ITypeTranslator2 tt1 = TypeTranslationProvider.GetTypeTranslator(SupportedTypeDomain.DotNet);
            Assert.AreEqual<string>("string", tt1[PrimitiveDataType.NOTATION]);
        }

        [TestMethod]
        public void TestXsdtTypeTranslator()
        {
            ITypeTranslator2 tt1 = TypeTranslationProvider.GetTypeTranslator(SupportedTypeDomain.XmlSchemaDatatypes);
            Assert.AreEqual<string>("short", tt1[PrimitiveDataType.SHORT]);
            Assert.AreEqual<string>("integer", tt1[PrimitiveDataType.INT]);
            Assert.AreEqual<string>("long", tt1[PrimitiveDataType.LONG]);
            Assert.AreEqual<string>("float", tt1[PrimitiveDataType.FLOAT]);
            Assert.AreEqual<string>("double", tt1[PrimitiveDataType.DOUBLE]);
            Assert.AreEqual<string>("decimal", tt1[PrimitiveDataType.DECIMAL]);
            Assert.AreEqual<string>("boolean", tt1[PrimitiveDataType.BOOLEAN]);
            Assert.AreEqual<string>("string", tt1[PrimitiveDataType.STRING]);
            Assert.AreEqual<string>("hexBinary", tt1[PrimitiveDataType.HEXBINARY]);
            Assert.AreEqual<string>("base64Binary", tt1[PrimitiveDataType.BASE64BINARY]);
            Assert.AreEqual<string>("duration", tt1[PrimitiveDataType.DURATION]);
            Assert.AreEqual<string>("dateTime", tt1[PrimitiveDataType.DATETIME]);
            Assert.AreEqual<string>("time", tt1[PrimitiveDataType.TIME]);
            Assert.AreEqual<string>("date", tt1[PrimitiveDataType.DATE]);
            Assert.AreEqual<string>("gYearMonth", tt1[PrimitiveDataType.GYEARMONTH]);
            Assert.AreEqual<string>("gYear", tt1[PrimitiveDataType.GYEAR]);
            Assert.AreEqual<string>("gMonthDay", tt1[PrimitiveDataType.GMONTHDAY]);
            Assert.AreEqual<string>("gDay", tt1[PrimitiveDataType.GDAY]);
            Assert.AreEqual<string>("gMonth", tt1[PrimitiveDataType.GMONTH]);
            Assert.AreEqual<string>("anyUri", tt1[PrimitiveDataType.ANYURI]);
            Assert.AreEqual<string>("QName", tt1[PrimitiveDataType.QNAME]);
            Assert.AreEqual<string>("NOTATION", tt1[PrimitiveDataType.NOTATION]);
            Assert.AreEqual<string>("string", tt1[PrimitiveDataType.UNKNOWN]);
                                                                 
        }
    }                                                            
}                                                                
