﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using LinqToRdf;

namespace UnitTests
{
    /// <summary>
    /// Summary description for TestTypeTranslator2
    /// </summary>
    [TestFixture]
    public class TestTypeTranslator2
    {
        public TestTypeTranslator2()
        {
            //
            // TODO: Add constructor logic here
            //
        }

		[Test]
        public void TestFactoryWorks()
        {
            ITypeTranslator2 tt1 = TypeTranslationProvider.GetTypeTranslator(SupportedTypeDomain.DotNet);
            Assert.IsNotNull(tt1);
            Assert.IsTrue(tt1 is DotnetTypeTranslator);
            ITypeTranslator2 tt2 = TypeTranslationProvider.GetTypeTranslator(SupportedTypeDomain.XmlSchemaDatatypes);
            Assert.IsNotNull(tt2);
            Assert.IsTrue(tt2 is XsdtTypeTranslator);
            Assert.AreNotEqual(tt1, tt2);
        }

		[Test]
        public void TestDotNetTypeTranslator()
        {
            ITypeTranslator2 tt1 = TypeTranslationProvider.GetTypeTranslator(SupportedTypeDomain.DotNet);
            Assert.AreEqual("short", tt1[PrimitiveDataType.SHORT]);
            Assert.AreEqual("int", tt1[PrimitiveDataType.INT]);
            Assert.AreEqual("long", tt1[PrimitiveDataType.LONG]);
            Assert.AreEqual("float", tt1[PrimitiveDataType.FLOAT]);
            Assert.AreEqual("double", tt1[PrimitiveDataType.DOUBLE]);
            Assert.AreEqual("decimal", tt1[PrimitiveDataType.DECIMAL]);
            Assert.AreEqual("bool", tt1[PrimitiveDataType.BOOLEAN]);
            Assert.AreEqual("string", tt1[PrimitiveDataType.STRING]);
            Assert.AreEqual("byte[]", tt1[PrimitiveDataType.HEXBINARY]);
            Assert.AreEqual("char[]", tt1[PrimitiveDataType.BASE64BINARY]);
            Assert.AreEqual("TimeSpan", tt1[PrimitiveDataType.DURATION]);
            Assert.AreEqual("DateTime", tt1[PrimitiveDataType.DATETIME]);
            Assert.AreEqual("DateTime", tt1[PrimitiveDataType.TIME]);
            Assert.AreEqual("DateTime", tt1[PrimitiveDataType.DATE]);
            Assert.AreEqual("DateTime", tt1[PrimitiveDataType.GYEARMONTH]);
            Assert.AreEqual("uint", tt1[PrimitiveDataType.GYEAR]);
            Assert.AreEqual("DateTime", tt1[PrimitiveDataType.GMONTHDAY]);
            Assert.AreEqual("uint", tt1[PrimitiveDataType.GDAY]);
            Assert.AreEqual("uint", tt1[PrimitiveDataType.GMONTH]);
            Assert.AreEqual("string", tt1[PrimitiveDataType.ANYURI]);
            Assert.AreEqual("string", tt1[PrimitiveDataType.QNAME]);
        }
        [Test, ExpectedException(typeof(NotImplementedException))]
        public void TestDotNetTypeTranslatorThrowsOk()
        {
            ITypeTranslator2 tt1 = TypeTranslationProvider.GetTypeTranslator(SupportedTypeDomain.DotNet);
            Assert.AreEqual("string", tt1[PrimitiveDataType.NOTATION]);
        }

		[Test]
        public void TestXsdtTypeTranslator()
        {
            ITypeTranslator2 tt1 = TypeTranslationProvider.GetTypeTranslator(SupportedTypeDomain.XmlSchemaDatatypes);
            Assert.AreEqual("short", tt1[PrimitiveDataType.SHORT]);
            Assert.AreEqual("integer", tt1[PrimitiveDataType.INT]);
            Assert.AreEqual("long", tt1[PrimitiveDataType.LONG]);
            Assert.AreEqual("float", tt1[PrimitiveDataType.FLOAT]);
            Assert.AreEqual("double", tt1[PrimitiveDataType.DOUBLE]);
            Assert.AreEqual("decimal", tt1[PrimitiveDataType.DECIMAL]);
            Assert.AreEqual("boolean", tt1[PrimitiveDataType.BOOLEAN]);
            Assert.AreEqual("string", tt1[PrimitiveDataType.STRING]);
            Assert.AreEqual("hexBinary", tt1[PrimitiveDataType.HEXBINARY]);
            Assert.AreEqual("base64Binary", tt1[PrimitiveDataType.BASE64BINARY]);
            Assert.AreEqual("duration", tt1[PrimitiveDataType.DURATION]);
            Assert.AreEqual("dateTime", tt1[PrimitiveDataType.DATETIME]);
            Assert.AreEqual("time", tt1[PrimitiveDataType.TIME]);
            Assert.AreEqual("date", tt1[PrimitiveDataType.DATE]);
            Assert.AreEqual("gYearMonth", tt1[PrimitiveDataType.GYEARMONTH]);
            Assert.AreEqual("gYear", tt1[PrimitiveDataType.GYEAR]);
            Assert.AreEqual("gMonthDay", tt1[PrimitiveDataType.GMONTHDAY]);
            Assert.AreEqual("gDay", tt1[PrimitiveDataType.GDAY]);
            Assert.AreEqual("gMonth", tt1[PrimitiveDataType.GMONTH]);
            Assert.AreEqual("anyUri", tt1[PrimitiveDataType.ANYURI]);
            Assert.AreEqual("QName", tt1[PrimitiveDataType.QNAME]);
            Assert.AreEqual("NOTATION", tt1[PrimitiveDataType.NOTATION]);
            Assert.AreEqual("string", tt1[PrimitiveDataType.UNKNOWN]);
                                                                 
        }


		[Test]
		public void TestXsdtToNet()
		{
			XsdtTypeConverter tc = new XsdtTypeConverter();
		    //Assert.AreSame(dbl, tc.Parse("\"10\"^^xsdt:double"));
			Assert.AreEqual(tc.Parse("\"10\"^^<http://www.w3.org/2001/XMLSchema#double>"), tc.Parse("\"10\"^^xsdt:double"));
			var f = tc.Parse("\"10\"^^<http://www.w3.org/2001/XMLSchema#float>");
			var d = tc.Parse("\"10\"^^xsdt:double");
			Assert.IsTrue(f is float);
			Assert.IsTrue(d is double);
			var s = tc.Parse("\"hello world\""); 
			Assert.IsTrue(s is string);
			Assert.AreEqual(tc.Parse("\"P1D\"^^xsdt:duration"), tc.Parse("\"P1D\"^^<http://www.w3.org/2001/XMLSchema#duration>"));
			var ts = tc.Parse("\"P1Y2M3DT10H30M\"^^xsdt:duration");
			Assert.IsTrue(ts != null && ts is TimeSpan);
			var i = tc.Parse("10");
			Assert.IsTrue(i is int);
			var i2 = tc.Parse("10^^xsdt:integer");
			Assert.IsTrue(i2 is int);
		}
  
    }                                                            
}                                                                
