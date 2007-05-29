using System;
using System.Expressions;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdfMusic;
using LinqToRdf;

namespace RdfSerialisationTest
{
	[TestClass()]
	public class ConstantEncodingTests
	{
		[TestMethod()]
		public void ConstantTest_string()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();
			ConstantExpression ce = Expression.Constant("hello world"); // string
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.Constant(ce);
			string actualResult = sb.ToString();
			string expectedResult = "\"hello world\"^^xsdt:string";
			Assert.AreEqual(expectedResult, actualResult, false, "invalid SPARQL Expression created for ExpressionType.And");
		}

		[TestMethod()]
		public void ConstantTest_char()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();
			ConstantExpression ce = Expression.Constant('a'); // string
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.Constant(ce);
			string actualResult = sb.ToString();
			string expectedResult = "\"a\"^^xsdt:string";
			Assert.AreEqual(expectedResult, actualResult, false, "invalid SPARQL Expression created for ExpressionType.And");
		}

		[TestMethod()]
		public void ConstantTest_short()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();
			ConstantExpression ce = Expression.Constant((short)4); // string
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.Constant(ce);
			string actualResult = sb.ToString();
			string expectedResult = "4^^xsdt:short";
			Assert.AreEqual(expectedResult, actualResult, false, "invalid SPARQL Expression created for ExpressionType.And");
		}

		[TestMethod()]
		public void ConstantTest_int()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();
			ConstantExpression ce = Expression.Constant(4); // string
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.Constant(ce);
			string actualResult = sb.ToString();
			string expectedResult = "4^^xsdt:int";
			Assert.AreEqual(expectedResult, actualResult, false, "invalid SPARQL Expression created for ExpressionType.And");
		}

		[TestMethod()]
		public void ConstantTest_long()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();
			ConstantExpression ce = Expression.Constant((long)4); // string
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.Constant(ce);
			string actualResult = sb.ToString();
			string expectedResult = "4^^xsdt:long";
			Assert.AreEqual(expectedResult, actualResult, false, "invalid SPARQL Expression created for ExpressionType.And");
		}

		[TestMethod()]
		public void ConstantTest_float()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();
			ConstantExpression ce = Expression.Constant((float)3.14); // string
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.Constant(ce);
			string actualResult = sb.ToString();
			string expectedResult = "3.14^^xsdt:float";
			Assert.AreEqual(expectedResult, actualResult, false, "invalid SPARQL Expression created for ExpressionType.And");
		}

		[TestMethod()]
		public void ConstantTest_double()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();
			ConstantExpression ce = Expression.Constant(3.14); // string
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.Constant(ce);
			string actualResult = sb.ToString();
			string expectedResult = "3.14^^xsdt:double";
			Assert.AreEqual(expectedResult, actualResult, false, "invalid SPARQL Expression created for ExpressionType.And");
		}

		[TestMethod()]
		public void ConstantTest_decimal()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();
			ConstantExpression ce = Expression.Constant((decimal)3.14); // string
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.Constant(ce);
			string actualResult = sb.ToString();
			string expectedResult = "3.14^^xsdt:decimal";
			Assert.AreEqual(expectedResult, actualResult, false, "invalid SPARQL Expression created for ExpressionType.And");
		}

		/// <summary>
		///  warning this a very fragle test that only works from Australia during daylight savings time :-(
		/// </summary>
		[TestMethod()]
		public void ConstantTest_DateTime()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();
			ConstantExpression ce = Expression.Constant(new DateTime(2002,03,04,05,06,07));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.Constant(ce);
			string actualResult = sb.ToString();
			string expectedResult = "\"2002-03-04T05:06:07+11:00\"^^xsdt:dateTime";
			Assert.AreEqual(expectedResult, actualResult);
		}

	}
}
