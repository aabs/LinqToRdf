/* 
 * Copyright (C) 2007, Andrew Matthews http://aabs.wordpress.com/
 *
 * This file is Free Software and part of LinqToRdf http://code.google.com/p/linqtordf/
 *
 * It is licensed under the following license:
 *   - Berkeley License, V2.0 or any newer version
 *
 * You may not use this file except in compliance with the above license.
 *
 * See http://code.google.com/p/linqtordf/ for the complete text of the license agreement.
 *
 */
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using LinqToRdf;
using LinqToRdf.Sparql;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdfMusic;
using EH = RdfSerialisationTest.ExpressionHelper;

namespace RdfSerialisationTest
{
	public static class ExpressionHelper
	{
		public static Expression CreateBinaryExpression(ExpressionType expressionType, object lh, object rh)
		{
			Expression left = (Expression) ((lh is Expression) ? lh : Expression.Constant(lh));
			Expression right = (Expression) ((rh is Expression) ? rh : Expression.Constant(rh));

			switch (expressionType)
			{
				case ExpressionType.Add:
					return Expression.Add(left, right);
				case ExpressionType.Subtract:
					return Expression.Subtract(left, right);
				case ExpressionType.Multiply:
					return Expression.Multiply(left, right);
				case ExpressionType.Divide:
					return Expression.Divide(left, right);
				case ExpressionType.And:
					return Expression.And(left, right);
				case ExpressionType.AndAlso:
					return Expression.AndAlso(left, right);
				case ExpressionType.Or:
					return Expression.Or(left, right);
				case ExpressionType.OrElse:
					return Expression.Or(left, right);
			}
			throw new ApplicationException("huh?");
		}

		public static Expression CreateUnaryExpression(ExpressionType expressionType, Expression exp)
		{
			throw new NotImplementedException();
		}

		public static Expression Member(MemberInfo mi)
		{
			if (mi is FieldInfo)
			{
				return Expression.Field(Expression.Parameter(mi.DeclaringType, "t"), mi as FieldInfo);
			}
			else if (mi is PropertyInfo)
			{
				return Expression.Property(Expression.Parameter(mi.DeclaringType, "t"), mi as PropertyInfo);
			}
			else if (mi is MethodInfo)
			{
				return Expression.Property(Expression.Parameter(mi.DeclaringType, "t"), mi as MethodInfo);
			}
			else throw new NotImplementedException("Member type not supported");
		}
	}

	/// <summary>
	///This is a test class for RdfSerialisation.SparqlExpressionTranslator&lt;T&gt; and is intended
	///to contain all RdfSerialisation.SparqlExpressionTranslator&lt;T&gt; Unit Tests
	///</summary>
	[TestClass()]
	public class SparqlExpressionTranslatorTest
	{
		public bool BooleanTestProperty
		{
			get { return (DateTime.Now.Second%2) == 0; }
		}

		public int IntTestProperty
		{
			get { return DateTime.Now.Second; }
			set { }
		}

		public int IntTestMethod(int arg)
		{
			return ++arg;
		}

		public int[] Ia
		{
			get { return ia; }
		}

		public int[] ia = new int[] {1, 2, 3, 4, 5, 6};

		private PropertyInfo GetProperty(string arg)
		{
			return GetType().GetProperty(arg);
		}

		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get { return testContextInstance; }
			set { testContextInstance = value; }
		}

		#region Additional test attributes

		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//Use TestInitialize to run code before running each test
		//
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//Use TestCleanup to run code after each test has run
		//
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//

		#endregion

		#region Unit Tests

		/// <summary>
		///A test for Add (Expression)
		///</summary>
		[TestMethod]
		public void AddTest()
		{
			LinqToSparqlExpTranslator<Track> target = new LinqToSparqlExpTranslator<Track>();
			target.TypeTranslator = new XsdtTypeConverter();

			Expression e = Expression.Add(
				EH.Member(GetType().GetProperty("IntTestProperty")),
				Expression.Constant(5));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.Add(e);
			string actualResult = sb.ToString();
			string expectedResult = "(?IntTestProperty)+(5)";
			Assert.AreEqual(expectedResult, actualResult, false, "invalid SPARQL Expression created for ExpressionType.And");
		}

		/// <summary>
		///A test for AddChecked (Expression)
		///</summary>
		[TestMethod]
		public void AddCheckedTest()
		{
			LinqToSparqlExpTranslator<Track> target = new LinqToSparqlExpTranslator<Track>();
			target.TypeTranslator = new XsdtTypeConverter();

			Expression e = Expression.AddChecked(
				EH.Member(GetType().GetProperty("IntTestProperty")),
				Expression.Constant(5));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.AddChecked(e);
			string actualResult = sb.ToString();
			string expectedResult = "(?IntTestProperty)+(5)";
			Assert.AreEqual(expectedResult, actualResult, false, "invalid SPARQL Expression created for ExpressionType.And");
		}

		/// <summary>
		///A test for And (Expression)
		///</summary>
		[TestMethod]
		public void AndTest()
		{
			LinqToSparqlExpTranslator<Track> target = new LinqToSparqlExpTranslator<Track>();
			target.TypeTranslator = new XsdtTypeConverter();

			Expression e = Expression.And(
				EH.Member(GetType().GetProperty("BooleanTestProperty")),
				Expression.Constant(true));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.And(e);
			string actualResult = sb.ToString();
			string expectedResult = "(?BooleanTestProperty)&&(True^^xsdt:boolean)";
			Assert.AreEqual(expectedResult, actualResult, false, "invalid SPARQL Expression created for ExpressionType.And");
		}

		/// <summary>
		///A test for AndAlso (Expression)
		///</summary>
		[TestMethod]
		public void AndAlsoTest()
		{
			LinqToSparqlExpTranslator<Track> target = new LinqToSparqlExpTranslator<Track>();
			target.TypeTranslator = new XsdtTypeConverter();

			Expression e = Expression.AndAlso(
				EH.Member(GetType().GetProperty("BooleanTestProperty")),
				Expression.Constant(true));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.AndAlso(e);
			string actualResult = sb.ToString();
			string expectedResult = "(?BooleanTestProperty)&&(True^^xsdt:boolean)";
			Assert.AreEqual(expectedResult, actualResult, false, "invalid SPARQL Expression created for ExpressionType.And");
		}

		/// <summary>
		///A test for TypeAs (Expression)
		///</summary>
		[TestMethod]
		[ExpectedException(typeof (NotImplementedException))]
		public void AsTest()
		{
			LinqToSparqlExpTranslator<Track> target = new LinqToSparqlExpTranslator<Track>();
			target.TypeTranslator = new XsdtTypeConverter();

			Expression e = Expression.TypeAs(Expression.Constant("true"), typeof (bool));
			target.StringBuilder = new StringBuilder();
			target.TypeAs(e);
		}

		/// <summary>
		///A test for BitwiseXor (Expression)
		///</summary>
		[TestMethod]
		[ExpectedException(typeof (NotImplementedException))]
		public void XorTest()
		{
			LinqToSparqlExpTranslator<Track> target = new LinqToSparqlExpTranslator<Track>();
			target.TypeTranslator = new XsdtTypeConverter();

			Expression e = Expression.ExclusiveOr(Expression.Constant(10), Expression.Constant(8));
			target.StringBuilder = new StringBuilder();
			target.ExclusiveOr(e);
		}

		/// <summary>
		///A test for Cast (Expression)
		///</summary>
		[TestMethod]
		[ExpectedException(typeof (ArgumentException))]
		public void TypeAsTest()
		{
			LinqToSparqlExpTranslator<Track> target = new LinqToSparqlExpTranslator<Track>();
			target.TypeTranslator = new XsdtTypeConverter();

			Expression e = Expression.TypeAs(Expression.Constant("3.14"), typeof (double));
			target.StringBuilder = new StringBuilder();
			target.TypeAs(e);
		}

		/// <summary>
		///A test for Coalesce (Expression)
		///</summary>
		[TestMethod]
		[ExpectedException(typeof (NotImplementedException))]
		public void CoalesceTest()
		{
			LinqToSparqlExpTranslator<Track> target = new LinqToSparqlExpTranslator<Track>();
			target.TypeTranslator = new XsdtTypeConverter();

			Expression e = Expression.Coalesce(Expression.Constant(10), Expression.Constant(15));
			target.StringBuilder = new StringBuilder();
			target.Coalesce(e);
		}

		/// <summary>
		///A test for Conditional (Expression)
		///</summary>
		[TestMethod]
		[ExpectedException(typeof (NotImplementedException))]
		public void ConditionalTest()
		{
			LinqToSparqlExpTranslator<Track> target = new LinqToSparqlExpTranslator<Track>();
			target.TypeTranslator = new XsdtTypeConverter();

			Expression e = Expression.Condition(
				Expression.Equal(EH.Member(GetType().GetProperty("BooleanTestProperty")),
				              Expression.Constant(true)),
				Expression.Constant(10), Expression.Constant(15));
			target.StringBuilder = new StringBuilder();
			target.Equal(e);
		}

		/// <summary>
		///A test for Convert (Expression)
		///</summary>
		[TestMethod]
		[ExpectedException(typeof (NotImplementedException))]
		public void ConvertTest()
		{
			LinqToSparqlExpTranslator<Track> target = new LinqToSparqlExpTranslator<Track>();
			target.TypeTranslator = new XsdtTypeConverter();

			Expression e = Expression.Convert(Expression.Constant(10), typeof (double));
			target.StringBuilder = new StringBuilder();
			target.Convert(e);
		}

		/// <summary>
		///A test for ConvertChecked (Expression)
		///</summary>
		[TestMethod]
		[ExpectedException(typeof (NotImplementedException))]
		public void ConvertCheckedTest()
		{
			LinqToSparqlExpTranslator<Track> target = new LinqToSparqlExpTranslator<Track>();
			target.TypeTranslator = new XsdtTypeConverter();

			Expression e = Expression.ConvertChecked(Expression.Constant(10), typeof (double));
			target.StringBuilder = new StringBuilder();
			target.ConvertChecked(e);
		}

		/// <summary>
		///A test for Divide (Expression)
		///</summary>
		[TestMethod]
		public void DivideTest()
		{
			LinqToSparqlExpTranslator<Track> target = new LinqToSparqlExpTranslator<Track>();
			target.TypeTranslator = new XsdtTypeConverter();

			Expression e = Expression.Divide(Expression.Constant(10), Expression.Constant(15));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.Divide(e);
			string actualResult = sb.ToString();
			string expectedResult = "(10)/(15)";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for Equal (Expression)
		///</summary>
		[TestMethod]
		public void EQTest()
		{
			LinqToSparqlExpTranslator<Track> target = new LinqToSparqlExpTranslator<Track>();
			target.TypeTranslator = new XsdtTypeConverter();

			Expression e = Expression.Equal(Expression.Constant(10), Expression.Constant(15));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.Equal(e);
			string actualResult = sb.ToString();
			string expectedResult = "10 = 15";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for Funclet (Expression)
		///</summary>
		[TestMethod, Ignore, ExpectedException(typeof (NotImplementedException))]
		public void FuncletTest()
		{
			// SparqlExpressionTranslator<T> target = new SparqlExpressionTranslator<T>();
			// 
			// Expression e = null; // TODO: Initialize to an appropriate value
			// 
			// target.Funclet(e);
			// 
			// Assert.Inconclusive("A method that does not return a value cannot be verified.");
			Assert.Inconclusive("Generics testing must be manually provided.");
		}

		/// <summary>
		///A test for GreaterThanOrEqual (Expression)
		///</summary>
		[TestMethod]
		public void GETest()
		{
			LinqToSparqlExpTranslator<Track> target = new LinqToSparqlExpTranslator<Track>();
			target.TypeTranslator = new XsdtTypeConverter();
			Expression e = Expression.GreaterThanOrEqual(Expression.Constant(10), Expression.Constant(15));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.GreaterThanOrEqual(e);
			string actualResult = sb.ToString();
			string expectedResult = "(10)>=(15)";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for GreaterThan (Expression)
		///</summary>
		[TestMethod]
		public void GTTest()
		{
			LinqToSparqlExpTranslator<Track> target = new LinqToSparqlExpTranslator<Track>();
			target.TypeTranslator = new XsdtTypeConverter();
			Expression e = Expression.GreaterThan(Expression.Constant(10), Expression.Constant(15));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.GreaterThan(e);
			string actualResult = sb.ToString();
			string expectedResult = "(10)>(15)";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for Index (Expression)
		///</summary>
		[TestMethod]
		public void IndexTest()
		{
			LinqToSparqlExpTranslator<Track> target = new LinqToSparqlExpTranslator<Track>();
			target.TypeTranslator = new XsdtTypeConverter();
			Expression e = Expression.ArrayIndex(EH.Member(GetType().GetProperty("Ia")), Expression.Constant(1));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.ArrayIndex(e);
			string actualResult = sb.ToString();
			string expectedResult = "?Ia[1]";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for Invoke (Expression)
		///</summary>
		[TestMethod]
		[Ignore, ExpectedException(typeof (NotImplementedException))]
		public void InvokeTest()
		{
			// not sure how to construct a lambda expression
		}

		/// <summary>
		///A test for TypeIs (Expression)
		///</summary>
		[TestMethod]
		[ExpectedException(typeof (NotImplementedException))]
		public void IsTest()
		{
			LinqToSparqlExpTranslator<Track> target = new LinqToSparqlExpTranslator<Track>();
			target.TypeTranslator = new XsdtTypeConverter();
			Expression e = Expression.TypeIs(Expression.Constant(10), typeof(int));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.TypeIs(e);
			string actualResult = sb.ToString();
			string expectedResult = "(10)>(15)";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for Lambda (Expression)
		///</summary>
		[TestMethod]
		[Ignore, ExpectedException(typeof (NotImplementedException))]
		public void LambdaTest()
		{
			// not sure how to construct a lambda expression
		}

		/// <summary>
		///A test for LessThanOrEqual (Expression)
		///</summary>
		[TestMethod]
		public void LETest()
		{
			LinqToSparqlExpTranslator<Track> target = new LinqToSparqlExpTranslator<Track>();
			target.TypeTranslator = new XsdtTypeConverter();
			Expression e = Expression.LessThanOrEqual(Expression.Constant(10), Expression.Constant(15));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.LessThanOrEqual(e);
			string actualResult = sb.ToString();
			string expectedResult = "(10)<=(15)";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for ArrayLength (Expression)
		///</summary>
		[TestMethod]
		[ExpectedException(typeof (NotImplementedException))]
		public void LenTest()
		{
			LinqToSparqlExpTranslator<Track> target = new LinqToSparqlExpTranslator<Track>();
			target.TypeTranslator = new XsdtTypeConverter();
			Expression e = Expression.ArrayLength(EH.Member(GetType().GetProperty("Ia")));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.ArrayLength(e);
			string actualResult = sb.ToString();
			string expectedResult = "(10)<=(15)";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for ListInit (Expression)
		///</summary>
		[TestMethod]
		[Ignore, ExpectedException(typeof (NotImplementedException))]
		public void ListInitTest()
		{
			// not sure how to build this
		}

		/// <summary>
		///A test for LShift (Expression)
		///</summary>
		[TestMethod]
		[ExpectedException(typeof (NotImplementedException))]
		public void LShiftTest()
		{
			LinqToSparqlExpTranslator<Track> target = new LinqToSparqlExpTranslator<Track>();
			target.TypeTranslator = new XsdtTypeConverter();
			Expression e = Expression.LeftShift(Expression.Constant(10), Expression.Constant(15));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.LeftShift(e);
			string actualResult = sb.ToString();
			string expectedResult = "(10)<=(15)";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for LessThan (Expression)
		///</summary>
		[TestMethod]
		public void LTTest()
		{
			LinqToSparqlExpTranslator<Track> target = new LinqToSparqlExpTranslator<Track>();
			target.TypeTranslator = new XsdtTypeConverter();
			Expression e = Expression.LessThan(Expression.Constant(10), Expression.Constant(15));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.LessThan(e);
			string actualResult = sb.ToString();
			string expectedResult = "(10)<(15)";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for MemberAccess (Expression)
		///</summary>
		[TestMethod]
		public void MemberAccessTest()
		{
			LinqToSparqlExpTranslator<Track> target = new LinqToSparqlExpTranslator<Track>();
			target.TypeTranslator = new XsdtTypeConverter();
			Expression e = EH.Member(GetType().GetProperty("Ia"));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.MemberAccess(e);
			string actualResult = sb.ToString();
			string expectedResult = "?Ia";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for MemberInit (Expression)
		///</summary>
		[TestMethod]
		[Ignore, ExpectedException(typeof (NotImplementedException))]
		public void MemberInitTest()
		{
			// not sure how to test this
		}

		/// <summary>
		///A test for MethodCall (Expression)
		///</summary>
		[Ignore, TestMethod]
		public void MethodCallTest()
		{
			LinqToSparqlExpTranslator<Track> target = new LinqToSparqlExpTranslator<Track>();
			target.TypeTranslator = new XsdtTypeConverter();
			Expression[] ea = new Expression[] { Expression.Constant(15) };
			Expression e = Expression.Call(GetType().GetMethod("IntTestMethod"), ea);
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.Call(e);
			string actualResult = sb.ToString();
			string expectedResult = "(t.IntTestMethod(15)";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for Modulo (Expression)
		///</summary>
		[TestMethod]
		[ExpectedException(typeof (NotImplementedException))]
		public void ModuloTest()
		{
			LinqToSparqlExpTranslator<Track> target = new LinqToSparqlExpTranslator<Track>();
			target.TypeTranslator = new XsdtTypeConverter();
			Expression e = Expression.Modulo(Expression.Constant(10), Expression.Constant(3));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.Modulo(e);
			string actualResult = sb.ToString();
			string expectedResult = "ignore";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for Multiply (Expression)
		///</summary>
		[TestMethod]
		public void MultiplyTest()
		{
			LinqToSparqlExpTranslator<Track> target = new LinqToSparqlExpTranslator<Track>();
			target.TypeTranslator = new XsdtTypeConverter();
			Expression e = EH.CreateBinaryExpression(ExpressionType.Multiply, 10, 15);
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.Multiply(e);
			string actualResult = sb.ToString();
			string expectedResult = "(10)*(15)";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for MultiplyChecked (Expression)
		///</summary>
		[TestMethod]
		public void MultiplyCheckedTest()
		{
			LinqToSparqlExpTranslator<Track> target = new LinqToSparqlExpTranslator<Track>();
			target.TypeTranslator = new XsdtTypeConverter();
			Expression e = Expression.MultiplyChecked(Expression.Constant(10), Expression.Constant(15));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.MultiplyChecked(e);
			string actualResult = sb.ToString();
			string expectedResult = "(10)*(15)";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for NotEqual (Expression)
		///</summary>
		[TestMethod]
		public void NETest()
		{
			LinqToSparqlExpTranslator<Track> target = new LinqToSparqlExpTranslator<Track>();
			target.TypeTranslator = new XsdtTypeConverter();
			Expression e = Expression.NotEqual(Expression.Constant(10), Expression.Constant(15));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.NotEqual(e);
			string actualResult = sb.ToString();
			string expectedResult = "(10)!=(15)";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for Negate (Expression)
		///</summary>
		[TestMethod]
		[ExpectedException(typeof (NotImplementedException))]
		public void NegateTest()
		{
			LinqToSparqlExpTranslator<Track> target = new LinqToSparqlExpTranslator<Track>();
			target.TypeTranslator = new XsdtTypeConverter();
			Expression e = Expression.Negate(EH.Member(GetProperty("IntTestProperty")));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.Negate(e);
			string actualResult = sb.ToString();
			string expectedResult = "!(?BooleanTestProperty)";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for New (Expression)
		///</summary>
		[TestMethod]
		[ExpectedException(typeof (NotImplementedException))]
		public void NewTest()
		{
			LinqToSparqlExpTranslator<Track> target = new LinqToSparqlExpTranslator<Track>();
			target.TypeTranslator = new XsdtTypeConverter();
			Expression e = Expression.New(GetType());
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.New(e);
			string actualResult = sb.ToString();
			string expectedResult = "(?BooleanTestProperty)";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for NewArrayBounds (Expression)
		///</summary>
		[TestMethod]
		[ExpectedException(typeof (NotImplementedException))]
		public void NewArrayBoundsTest()
		{
			LinqToSparqlExpTranslator<Track> target = new LinqToSparqlExpTranslator<Track>();
			target.TypeTranslator = new XsdtTypeConverter();
			Expression e = Expression.NewArrayBounds(GetType(), new Expression[] { Expression.Constant(10) }); //  array of ten
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.NewArrayBounds(e);
			string actualResult = sb.ToString();
			string expectedResult = "(?BooleanTestProperty)";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for NewArrayInit (Expression)
		///</summary>
		[TestMethod]
		[Ignore, ExpectedException(typeof (NotImplementedException))]
		public void NewArrayInitTest()
		{
			LinqToSparqlExpTranslator<Track> target = new LinqToSparqlExpTranslator<Track>();
			target.TypeTranslator = new XsdtTypeConverter();
			Expression e = Expression.NewArrayInit(GetType(), new Expression[] { Expression.Constant(10) }); //  array of ten
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.NewArrayInit(e);
			string actualResult = sb.ToString();
			string expectedResult = "(?BooleanTestProperty)";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for Not (Expression)
		///</summary>
		[TestMethod]
		public void NotTest()
		{
			LinqToSparqlExpTranslator<Track> target = new LinqToSparqlExpTranslator<Track>();
			target.TypeTranslator = new XsdtTypeConverter();
			Expression e = Expression.Not(EH.Member(GetType().GetProperty("BooleanTestProperty")));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.Not(e);
			string actualResult = sb.ToString();
			string expectedResult = "!(?BooleanTestProperty)";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for Or (Expression)
		///</summary>
		[TestMethod]
		public void OrTest()
		{
			LinqToSparqlExpTranslator<Track> target = new LinqToSparqlExpTranslator<Track>();
			target.TypeTranslator = new XsdtTypeConverter();
			Expression e =
				Expression.Or(EH.Member(GetProperty("BooleanTestProperty")),
				              Expression.Not(EH.Member(GetProperty("BooleanTestProperty"))));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.Or(e);
			string actualResult = sb.ToString();
			string expectedResult = "(?BooleanTestProperty)||(!(?BooleanTestProperty))";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for OrElse (Expression)
		///</summary>
		[TestMethod]
		public void OrElseTest()
		{
			LinqToSparqlExpTranslator<Track> target = new LinqToSparqlExpTranslator<Track>();
			target.TypeTranslator = new XsdtTypeConverter();
			Expression e =
				Expression.OrElse(EH.Member(GetProperty("BooleanTestProperty")),
				                  Expression.Not(EH.Member(GetProperty("BooleanTestProperty"))));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.OrElse(e);
			string actualResult = sb.ToString();
			string expectedResult = "(?BooleanTestProperty)||(!(?BooleanTestProperty))";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for Parameter (Expression)
		///</summary>
		[TestMethod]
		public void ParameterTest()
		{
			LinqToSparqlExpTranslator<Track> target = new LinqToSparqlExpTranslator<Track>();
			target.TypeTranslator = new XsdtTypeConverter();
			Expression e = Expression.Parameter(GetType(), "t");
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.Parameter(e);
			string actualResult = sb.ToString();
			string expectedResult = "t"; //  not sure that any output should be expected
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for Quote (Expression)
		///</summary>
		[TestMethod]
		public void QuoteTest()
		{
			LinqToSparqlExpTranslator<Track> target = new LinqToSparqlExpTranslator<Track>();
			target.TypeTranslator = new XsdtTypeConverter();
			Expression e = Expression.Quote(Expression.Constant(5));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.Quote(e);
			string actualResult = sb.ToString();
			string expectedResult = "\"5\""; //  assumption - it should retain its XSDT type?
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for RShift (Expression)
		///</summary>
		[TestMethod]
		[ExpectedException(typeof (NotImplementedException))]
		public void RShiftTest()
		{
			LinqToSparqlExpTranslator<Track> target = new LinqToSparqlExpTranslator<Track>();
			target.TypeTranslator = new XsdtTypeConverter();
			Expression e = Expression.RightShift(Expression.Constant(1), Expression.Constant(5));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.RightShift(e);
			string actualResult = sb.ToString();
			string expectedResult = ""; //  shouldn't make it to here
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for Sanitise (string)
		///</summary>
		[TestMethod, Ignore, ExpectedException(typeof (NotImplementedException))]
		public void SanitiseTest()
		{
			// Unit Test Generation Error: A private accessor could not be created for RdfSerialisation.SparqlExpressionTranslator<T>.Sanitise: Private accessors cannot be created for generic types
			Assert.Fail("Unit Test Generation Error: A private accessor could not be created for RdfSerial" +
			            "isation.SparqlExpressionTranslator<T>.Sanitise: Private accessors cannot be crea" +
			            "ted for generic types");
		}

		/// <summary>
		///A test for SparqlExpressionTranslator ()
		///</summary>
		[TestMethod, Ignore, ExpectedException(typeof (NotImplementedException))]
		public void ConstructorTest()
		{
			// SparqlExpressionTranslator<T> target = new SparqlExpressionTranslator<T>();
			// 
			// // TODO: Implement code to verify target
			// Assert.Inconclusive("TODO: Implement code to verify target");
			Assert.Inconclusive("Generics testing must be manually provided.");
		}

		/// <summary>
		///A test for SparqlExpressionTranslator (StringBuilder)
		///</summary>
		[TestMethod, Ignore, ExpectedException(typeof (NotImplementedException))]
		public void ConstructorTest1()
		{
			// StringBuilder stringBuilder = null; // TODO: Initialize to an appropriate value
			// 
			// SparqlExpressionTranslator<T> target = new SparqlExpressionTranslator<T>(stringBuilder);
			// 
			// // TODO: Implement code to verify target
			// Assert.Inconclusive("TODO: Implement code to verify target");
			Assert.Inconclusive("Generics testing must be manually provided.");
		}

		/// <summary>
		///A test for StringBuilder
		///</summary>
		[TestMethod, Ignore, ExpectedException(typeof (NotImplementedException))]
		public void StringBuilderTest()
		{
			// SparqlExpressionTranslator<T> target = new SparqlExpressionTranslator<T>();
			// 
			// StringBuilder val = null; // TODO: Assign to an appropriate value for the property
			// 
			// target.StringBuilder = val;
			// 
			// 
			// Assert.AreEqual(val, target.StringBuilder, "RdfSerialisation.SparqlExpressionTranslator<T>.StringBuilder was not set correctl" +
			//        "y.");
			// Assert.Inconclusive("Verify the correctness of this test method.");
			Assert.Inconclusive("Generics testing must be manually provided.");
		}

		/// <summary>
		///A test for Subtract (Expression)
		///</summary>
		[TestMethod]
		public void SubtractTest()
		{
			LinqToSparqlExpTranslator<Track> target = new LinqToSparqlExpTranslator<Track>();
			target.TypeTranslator = new XsdtTypeConverter();
			Expression e = Expression.Subtract(EH.Member(GetType().GetProperty("IntTestProperty")), Expression.Constant(1));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.Subtract(e);
			string actualResult = sb.ToString();
			string expectedResult = "(?IntTestProperty)-(1)";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for SubtractChecked (Expression)
		///</summary>
		[TestMethod]
		public void SubtractCheckedTest()
		{
			LinqToSparqlExpTranslator<Track> target = new LinqToSparqlExpTranslator<Track>();
			target.TypeTranslator = new XsdtTypeConverter();
			Expression e =
				Expression.SubtractChecked(EH.Member(GetType().GetProperty("IntTestProperty")), Expression.Constant(1));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.SubtractChecked(e);
			string actualResult = sb.ToString();
			string expectedResult = "(?IntTestProperty)-(1)";
			Assert.AreEqual(expectedResult, actualResult);
		}

		#endregion
	}
}