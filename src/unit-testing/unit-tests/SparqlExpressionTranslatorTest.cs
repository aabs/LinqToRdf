using System;
using System.Expressions;
using System.Reflection;
using System.Text;
using LinqToRdf;
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
		[TestMethod()]
		public void AddTest()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();

			Expression e = Expression.Add(
				EH.Member(GetType().GetProperty("IntTestProperty")),
				Expression.Constant(5));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.Add(e);
			string actualResult = sb.ToString();
			string expectedResult = "(t.IntTestProperty)+(5^^xsdt:int)";
			Assert.AreEqual(expectedResult, actualResult, false, "invalid SPARQL Expression created for ExpressionType.And");
		}

		/// <summary>
		///A test for AddChecked (Expression)
		///</summary>
		[TestMethod()]
		public void AddCheckedTest()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();

			Expression e = Expression.AddChecked(
				EH.Member(GetType().GetProperty("IntTestProperty")),
				Expression.Constant(5));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.AddChecked(e);
			string actualResult = sb.ToString();
			string expectedResult = "(t.IntTestProperty)+(5^^xsdt:int)";
			Assert.AreEqual(expectedResult, actualResult, false, "invalid SPARQL Expression created for ExpressionType.And");
		}

		/// <summary>
		///A test for And (Expression)
		///</summary>
		[TestMethod()]
		public void AndTest()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();

			Expression e = Expression.And(
				EH.Member(GetType().GetProperty("BooleanTestProperty")),
				Expression.Constant(true));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.And(e);
			string actualResult = sb.ToString();
			string expectedResult = "(t.BooleanTestProperty)&&(True^^xsdt:boolean)";
			Assert.AreEqual(expectedResult, actualResult, false, "invalid SPARQL Expression created for ExpressionType.And");
		}

		/// <summary>
		///A test for AndAlso (Expression)
		///</summary>
		[TestMethod()]
		public void AndAlsoTest()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();

			Expression e = Expression.AndAlso(
				EH.Member(GetType().GetProperty("BooleanTestProperty")),
				Expression.Constant(true));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.AndAlso(e);
			string actualResult = sb.ToString();
			string expectedResult = "(t.BooleanTestProperty)&&(True^^xsdt:boolean)";
			Assert.AreEqual(expectedResult, actualResult, false, "invalid SPARQL Expression created for ExpressionType.And");
		}

		/// <summary>
		///A test for As (Expression)
		///</summary>
		[TestMethod()]
		[ExpectedException(typeof (NotImplementedException))]
		public void AsTest()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();

			Expression e = Expression.As(Expression.Constant("true"), typeof (bool));
			target.StringBuilder = new StringBuilder();
			target.As(e);
		}

		/// <summary>
		///A test for BitwiseAnd (Expression)
		///</summary>
		[TestMethod()]
		[ExpectedException(typeof (NotImplementedException))]
		public void BitwiseAndTest()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();

			Expression e = Expression.BitAnd(
				EH.Member(GetType().GetProperty("BooleanTestProperty")),
				Expression.Constant(true));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.BitwiseAnd(e);
			string actualResult = sb.ToString();
			string expectedResult = "(t.BooleanTestProperty)&(True^^xsdt:boolean)";
			Assert.AreEqual(expectedResult, actualResult, false, "invalid SPARQL Expression created for ExpressionType.And");
		}

		/// <summary>
		///A test for BitwiseNot (Expression)
		///</summary>
		[TestMethod()]
		[ExpectedException(typeof (NotImplementedException))]
		public void BitwiseNotTest()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();

			Expression e = Expression.BitNot(EH.Member(GetType().GetProperty("BooleanTestProperty")));
			target.StringBuilder = new StringBuilder();
			target.BitwiseNot(e);
		}

		/// <summary>
		///A test for BitwiseOr (Expression)
		///</summary>
		[TestMethod()]
		[ExpectedException(typeof (NotImplementedException))]
		public void BitwiseOrTest()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();

			Expression e = Expression.BitOr(
				EH.Member(GetType().GetProperty("BooleanTestProperty")),
				Expression.Constant(true));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.BitwiseOr(e);
			string actualResult = sb.ToString();
			string expectedResult = "(t.BooleanTestProperty)|(True^^xsdt:boolean)";
			Assert.AreEqual(expectedResult, actualResult, false, "invalid SPARQL Expression created for ExpressionType.And");
		}

		/// <summary>
		///A test for BitwiseXor (Expression)
		///</summary>
		[TestMethod()]
		[ExpectedException(typeof (NotImplementedException))]
		public void BitwiseXorTest()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();

			Expression e = Expression.BitXor(Expression.Constant(10), Expression.Constant(8));
			target.StringBuilder = new StringBuilder();
			target.BitwiseXor(e);
		}

		/// <summary>
		///A test for Cast (Expression)
		///</summary>
		[TestMethod()]
		[ExpectedException(typeof (ArgumentException))]
		public void CastTest()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();

			Expression e = Expression.Cast(Expression.Constant("3.14"), typeof (double));
			target.StringBuilder = new StringBuilder();
			target.Cast(e);
		}

		/// <summary>
		///A test for Coalesce (Expression)
		///</summary>
		[TestMethod()]
		[ExpectedException(typeof (NotImplementedException))]
		public void CoalesceTest()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();

			Expression e = Expression.Coalesce(Expression.Constant(10), Expression.Constant(15));
			target.StringBuilder = new StringBuilder();
			target.Coalesce(e);
		}

		/// <summary>
		///A test for Conditional (Expression)
		///</summary>
		[TestMethod()]
		[ExpectedException(typeof (NotImplementedException))]
		public void ConditionalTest()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();

			Expression e = Expression.Condition(
				Expression.EQ(EH.Member(GetType().GetProperty("BooleanTestProperty")),
				              Expression.Constant(true)),
				Expression.Constant(10), Expression.Constant(15));
			target.StringBuilder = new StringBuilder();
			target.Conditional(e);
		}

		/// <summary>
		///A test for Convert (Expression)
		///</summary>
		[TestMethod()]
		[ExpectedException(typeof (NotImplementedException))]
		public void ConvertTest()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();

			Expression e = Expression.Convert(Expression.Constant(10), typeof (double));
			target.StringBuilder = new StringBuilder();
			target.Convert(e);
		}

		/// <summary>
		///A test for ConvertChecked (Expression)
		///</summary>
		[TestMethod()]
		[ExpectedException(typeof (NotImplementedException))]
		public void ConvertCheckedTest()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();

			Expression e = Expression.ConvertChecked(Expression.Constant(10), typeof (double));
			target.StringBuilder = new StringBuilder();
			target.ConvertChecked(e);
		}

		/// <summary>
		///A test for Divide (Expression)
		///</summary>
		[TestMethod()]
		public void DivideTest()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();

			Expression e = Expression.Divide(Expression.Constant(10), Expression.Constant(15));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.Divide(e);
			string actualResult = sb.ToString();
			string expectedResult = "(10^^xsdt:int)/(15^^xsdt:int)";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for EQ (Expression)
		///</summary>
		[TestMethod()]
		public void EQTest()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();

			Expression e = Expression.EQ(Expression.Constant(10), Expression.Constant(15));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.EQ(e);
			string actualResult = sb.ToString();
			string expectedResult = "(10^^xsdt:int)=(15^^xsdt:int)";
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
		///A test for GE (Expression)
		///</summary>
		[TestMethod()]
		public void GETest()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();
			Expression e = Expression.GE(Expression.Constant(10), Expression.Constant(15));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.GE(e);
			string actualResult = sb.ToString();
			string expectedResult = "(10^^xsdt:int)>=(15^^xsdt:int)";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for GT (Expression)
		///</summary>
		[TestMethod()]
		public void GTTest()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();
			Expression e = Expression.GT(Expression.Constant(10), Expression.Constant(15));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.GT(e);
			string actualResult = sb.ToString();
			string expectedResult = "(10^^xsdt:int)>(15^^xsdt:int)";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for Index (Expression)
		///</summary>
		[TestMethod()]
		public void IndexTest()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();
			Expression e = Expression.Index(EH.Member(GetType().GetProperty("Ia")), Expression.Constant(1));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.Index(e);
			string actualResult = sb.ToString();
			string expectedResult = "t.Ia[1^^xsdt:int]";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for Invoke (Expression)
		///</summary>
		[TestMethod()]
		[Ignore, ExpectedException(typeof (NotImplementedException))]
		public void InvokeTest()
		{
			// not sure how to construct a lambda expression
		}

		/// <summary>
		///A test for Is (Expression)
		///</summary>
		[TestMethod()]
		[ExpectedException(typeof (NotImplementedException))]
		public void IsTest()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();
			Expression e = Expression.Is(Expression.Constant(10), typeof (int));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.Is(e);
			string actualResult = sb.ToString();
			string expectedResult = "(10^^xsdt:int)>(15^^xsdt:int)";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for Lambda (Expression)
		///</summary>
		[TestMethod()]
		[Ignore, ExpectedException(typeof (NotImplementedException))]
		public void LambdaTest()
		{
			// not sure how to construct a lambda expression
		}

		/// <summary>
		///A test for LE (Expression)
		///</summary>
		[TestMethod()]
		public void LETest()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();
			Expression e = Expression.LE(Expression.Constant(10), Expression.Constant(15));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.LE(e);
			string actualResult = sb.ToString();
			string expectedResult = "(10^^xsdt:int)<=(15^^xsdt:int)";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for Len (Expression)
		///</summary>
		[TestMethod()]
		[ExpectedException(typeof (NotImplementedException))]
		public void LenTest()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();
			Expression e = Expression.Len(EH.Member(GetType().GetProperty("Ia")));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.Len(e);
			string actualResult = sb.ToString();
			string expectedResult = "(10^^xsdt:int)<=(15^^xsdt:int)";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for ListInit (Expression)
		///</summary>
		[TestMethod()]
		[Ignore, ExpectedException(typeof (NotImplementedException))]
		public void ListInitTest()
		{
			// not sure how to build this
		}

		/// <summary>
		///A test for LShift (Expression)
		///</summary>
		[TestMethod()]
		[ExpectedException(typeof (NotImplementedException))]
		public void LShiftTest()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();
			Expression e = Expression.LShift(Expression.Constant(10), Expression.Constant(15));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.LShift(e);
			string actualResult = sb.ToString();
			string expectedResult = "(10^^xsdt:int)<=(15^^xsdt:int)";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for LT (Expression)
		///</summary>
		[TestMethod()]
		public void LTTest()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();
			Expression e = Expression.LT(Expression.Constant(10), Expression.Constant(15));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.LT(e);
			string actualResult = sb.ToString();
			string expectedResult = "(10^^xsdt:int)<(15^^xsdt:int)";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for MemberAccess (Expression)
		///</summary>
		[TestMethod()]
		public void MemberAccessTest()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();
			Expression e = EH.Member(GetType().GetProperty("Ia"));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.MemberAccess(e);
			string actualResult = sb.ToString();
			string expectedResult = "t.Ia";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for MemberInit (Expression)
		///</summary>
		[TestMethod()]
		[Ignore, ExpectedException(typeof (NotImplementedException))]
		public void MemberInitTest()
		{
			// not sure how to test this
		}

		/// <summary>
		///A test for MethodCall (Expression)
		///</summary>
		[Ignore, TestMethod()]
		public void MethodCallTest()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();
			Expression[] ea = new Expression[] {Expression.Constant(15)};
			Expression e = Expression.Call(GetType().GetMethod("IntTestMethod"), Expression.Constant(this), ea);
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.MethodCall(e);
			string actualResult = sb.ToString();
			string expectedResult = "(t.IntTestMethod(15)";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for MethodCallVirtual (Expression)
		///</summary>
		[Ignore, TestMethod()]
		public void MethodCallVirtualTest()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();
			Expression e = Expression.CallVirtual(GetType().GetMethod("IntTestMethod"), Expression.Constant(15));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.MethodCallVirtual(e);
			string actualResult = sb.ToString();
			string expectedResult = "(t.IntTestMethod(15))";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for Modulo (Expression)
		///</summary>
		[TestMethod()]
		[ExpectedException(typeof (NotImplementedException))]
		public void ModuloTest()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();
			Expression e = Expression.Modulo(Expression.Constant(10), Expression.Constant(3));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.MethodCallVirtual(e);
			string actualResult = sb.ToString();
			string expectedResult = "ignore";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for Multiply (Expression)
		///</summary>
		[TestMethod()]
		public void MultiplyTest()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();
			Expression e = EH.CreateBinaryExpression(ExpressionType.Multiply, 10, 15);
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.Multiply(e);
			string actualResult = sb.ToString();
			string expectedResult = "(10^^xsdt:int)*(15^^xsdt:int)";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for MultiplyChecked (Expression)
		///</summary>
		[TestMethod()]
		public void MultiplyCheckedTest()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();
			Expression e = Expression.MultiplyChecked(Expression.Constant(10), Expression.Constant(15));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.MultiplyChecked(e);
			string actualResult = sb.ToString();
			string expectedResult = "(10^^xsdt:int)*(15^^xsdt:int)";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for NE (Expression)
		///</summary>
		[TestMethod()]
		public void NETest()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();
			Expression e = Expression.NE(Expression.Constant(10), Expression.Constant(15));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.NE(e);
			string actualResult = sb.ToString();
			string expectedResult = "(10^^xsdt:int)!=(15^^xsdt:int)";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for Negate (Expression)
		///</summary>
		[TestMethod()]
		[ExpectedException(typeof (NotImplementedException))]
		public void NegateTest()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();
			Expression e = Expression.Negate(EH.Member(GetProperty("IntTestProperty")));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.Negate(e);
			string actualResult = sb.ToString();
			string expectedResult = "!(t.BooleanTestProperty)";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for New (Expression)
		///</summary>
		[TestMethod()]
		[ExpectedException(typeof (NotImplementedException))]
		public void NewTest()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();
			Expression e = Expression.New(GetType());
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.New(e);
			string actualResult = sb.ToString();
			string expectedResult = "(t.BooleanTestProperty)";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for NewArrayBounds (Expression)
		///</summary>
		[TestMethod()]
		[ExpectedException(typeof (NotImplementedException))]
		public void NewArrayBoundsTest()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();
			Expression e = Expression.NewArrayBounds(GetType(), new Expression[] {Expression.Constant(10)}); //  array of ten
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.NewArrayBounds(e);
			string actualResult = sb.ToString();
			string expectedResult = "(t.BooleanTestProperty)";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for NewArrayInit (Expression)
		///</summary>
		[TestMethod()]
		[Ignore, ExpectedException(typeof (NotImplementedException))]
		public void NewArrayInitTest()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();
			Expression e = Expression.NewArrayInit(GetType(), new Expression[] {Expression.Constant(10)}); //  array of ten
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.NewArrayInit(e);
			string actualResult = sb.ToString();
			string expectedResult = "(t.BooleanTestProperty)";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for Not (Expression)
		///</summary>
		[TestMethod()]
		public void NotTest()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();
			Expression e = Expression.Not(EH.Member(GetType().GetProperty("BooleanTestProperty")));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.Not(e);
			string actualResult = sb.ToString();
			string expectedResult = "!(t.BooleanTestProperty)";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for Or (Expression)
		///</summary>
		[TestMethod()]
		public void OrTest()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();
			Expression e =
				Expression.Or(EH.Member(GetProperty("BooleanTestProperty")),
				              Expression.Not(EH.Member(GetProperty("BooleanTestProperty"))));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.Or(e);
			string actualResult = sb.ToString();
			string expectedResult = "(t.BooleanTestProperty)||(!(t.BooleanTestProperty))";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for OrElse (Expression)
		///</summary>
		[TestMethod()]
		public void OrElseTest()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();
			Expression e =
				Expression.OrElse(EH.Member(GetProperty("BooleanTestProperty")),
				                  Expression.Not(EH.Member(GetProperty("BooleanTestProperty"))));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.OrElse(e);
			string actualResult = sb.ToString();
			string expectedResult = "(t.BooleanTestProperty)||(!(t.BooleanTestProperty))";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for Parameter (Expression)
		///</summary>
		[TestMethod()]
		public void ParameterTest()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();
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
		[TestMethod()]
		public void QuoteTest()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();
			Expression e = Expression.Quote(Expression.Constant(5));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.Quote(e);
			string actualResult = sb.ToString();
			string expectedResult = "\"5^^xsdt:int\""; //  assumption - it should retain its XSDT type?
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for RShift (Expression)
		///</summary>
		[TestMethod()]
		[ExpectedException(typeof (NotImplementedException))]
		public void RShiftTest()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();
			Expression e = Expression.RShift(Expression.Constant(1), Expression.Constant(5));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.RShift(e);
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
		[TestMethod()]
		public void SubtractTest()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();
			Expression e = Expression.Subtract(EH.Member(GetType().GetProperty("IntTestProperty")), Expression.Constant(1));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.Subtract(e);
			string actualResult = sb.ToString();
			string expectedResult = "(t.IntTestProperty)-(1^^xsdt:int)";
			Assert.AreEqual(expectedResult, actualResult);
		}

		/// <summary>
		///A test for SubtractChecked (Expression)
		///</summary>
		[TestMethod()]
		public void SubtractCheckedTest()
		{
			SparqlExpressionTranslator<Track> target = new SparqlExpressionTranslator<Track>();
			Expression e =
				Expression.SubtractChecked(EH.Member(GetType().GetProperty("IntTestProperty")), Expression.Constant(1));
			StringBuilder sb = new StringBuilder();
			target.StringBuilder = sb;
			target.SubtractChecked(e);
			string actualResult = sb.ToString();
			string expectedResult = "(t.IntTestProperty)-(1^^xsdt:int)";
			Assert.AreEqual(expectedResult, actualResult);
		}

		#endregion
	}
}