/* 
 * Copyright (C) 2007, Andrew Matthews http://aabs.wordpress.com/
 *
 * This file is Free Software and part of LinqToRdf http://code.google.com/fromName/linqtordf/
 *
 * It is licensed under the following license:
 *   - Berkeley License, V2.0 or any newer version
 *
 * You may not use this file except in compliance with the above license.
 *
 * See http://code.google.com/fromName/linqtordf/ for the complete text of the license agreement.
 *
 */
using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Collections.Generic;

namespace LinqToRdf
{
	public class LinqToN3ExpTranslator<T> : IQueryFormatTranslator
	{
		public ITypeTranslator TypeTranslator
		{
			get { return typeTranslator; }
			set { typeTranslator = value; }
		}

		#region IQueryFormatTranslator Members

		public HashSet<MemberInfo> Parameters
		{
			get { return parameters; }
			set { parameters = value; }
		}

		#endregion

		private ITypeTranslator typeTranslator = null;
		public StringBuilder StringBuilder
		{
			get { return stringBuilder; }
			set { stringBuilder = value; }
		}

		private StringBuilder stringBuilder;

		public LinqToN3ExpTranslator()
		{
			stringBuilder = new StringBuilder();
		}

		public LinqToN3ExpTranslator(StringBuilder stringBuilder)
		{
			this.stringBuilder = stringBuilder;
		}

		public void Dispatch(Expression expression)
		{
			switch (expression.NodeType)
			{
				case ExpressionType.Add:
					Add(expression);
					break;
				case ExpressionType.AddChecked:
					AddChecked(expression);
					break;
				case ExpressionType.And:
					And(expression);
					break;
				case ExpressionType.AndAlso:
					AndAlso(expression);
					break;
				case ExpressionType.ArrayIndex:
					Index(expression);
					break;
				case ExpressionType.ArrayLength:
					ArrayLength(expression);
					break;
				case ExpressionType.Call:
					MethodCall(expression);
					break;
				case ExpressionType.Coalesce:
					Coalesce(expression);
					break;
				case ExpressionType.Conditional:
					Conditional(expression);
					break;
				case ExpressionType.Constant:
					Constant(expression);
					break;
				case ExpressionType.Convert:
					Convert(expression);
					break;
				case ExpressionType.ConvertChecked:
					ConvertChecked(expression);
					break;
				case ExpressionType.Divide:
					Divide(expression);
					break;
				case ExpressionType.Equal:
					Equal(expression);
					break;
				case ExpressionType.ExclusiveOr:
					ExclusiveOr(expression);
					break;
				case ExpressionType.GreaterThan:
					GreaterThan(expression);
					break;
				case ExpressionType.GreaterThanOrEqual:
					GreaterThanOrEqual(expression);
					break;
				case ExpressionType.Invoke:
					Invoke(expression);
					break;
				case ExpressionType.Lambda:
					Lambda(expression);
					break;
				case ExpressionType.LeftShift:
					LeftShift(expression);
					break;
				case ExpressionType.LessThan:
					LessThan(expression);
					break;
				case ExpressionType.LessThanOrEqual:
					LessThanOrEqual(expression);
					break;
				case ExpressionType.ListInit:
					ListInit(expression);
					break;
				case ExpressionType.MemberAccess:
					MemberAccess(expression);
					break;
				case ExpressionType.MemberInit:
					MemberInit(expression);
					break;
				case ExpressionType.Modulo:
					Modulo(expression);
					break;
				case ExpressionType.Multiply:
					Multiply(expression);
					break;
				case ExpressionType.MultiplyChecked:
					MultiplyChecked(expression);
					break;
				case ExpressionType.Negate:
					Negate(expression);
					break;
				case ExpressionType.NegateChecked:
					NegateChecked(expression);
					break;
				case ExpressionType.New:
					New(expression);
					break;
				case ExpressionType.NewArrayBounds:
					NewArrayBounds(expression);
					break;
				case ExpressionType.NewArrayInit:
					NewArrayInit(expression);
					break;
				case ExpressionType.Not:
					Not(expression);
					break;
				case ExpressionType.NotEqual:
					NotEqual(expression);
					break;
				case ExpressionType.Or:
					Or(expression);
					break;
				case ExpressionType.OrElse:
					OrElse(expression);
					break;
				case ExpressionType.Parameter:
					Parameter(expression);
					break;
				case ExpressionType.Power:
					Power(expression);
					break;
				case ExpressionType.Quote:
					Quote(expression);
					break;
				case ExpressionType.RightShift:
					RightShift(expression);
					break;
				case ExpressionType.Subtract:
					Subtract(expression);
					break;
				case ExpressionType.SubtractChecked:
					SubtractChecked(expression);
					break;
				case ExpressionType.TypeAs:
					TypeAs(expression);
					break;
				case ExpressionType.TypeIs:
					TypeIs(expression);
					break;
				case ExpressionType.UnaryPlus:
					UnaryPlus(expression);
					break;
			}
			#region old code
			/*switch (expression.NodeType)
			{
				case ExpressionType.Add:
					Add(expression);
					break;
				case ExpressionType.AddChecked:
					AddChecked(expression);
					break;
				case ExpressionType.And:
					And(expression);
					break;
				case ExpressionType.AndAlso:
					AndAlso(expression);
					break;
				case ExpressionType.TypeAs:
					TypeAs(expression);
					break;
				case ExpressionType.BitwiseAnd:
					BitwiseAnd(expression);
					break;
				case ExpressionType.BitwiseNot:
					BitwiseNot(expression);
					break;
				case ExpressionType.BitwiseOr:
					BitwiseOr(expression);
					break;
				case ExpressionType.BitwiseXor:
					BitwiseXor(expression);
					break;
				case ExpressionType.Cast:
					Cast(expression);
					break;
				case ExpressionType.Coalesce:
					Coalesce(expression);
					break;
				case ExpressionType.Conditional:
					Conditional(expression);
					break;
				case ExpressionType.Constant:
					Constant(expression);
					break;
				case ExpressionType.Convert:
					Convert(expression);
					break;
				case ExpressionType.ConvertChecked:
					ConvertChecked(expression);
					break;
				case ExpressionType.Divide:
					Divide(expression);
					break;
				case ExpressionType.Equal:
					Equal(expression);
					break;
				case ExpressionType.Funclet:
					Funclet(expression);
					break;
				case ExpressionType.GreaterThan:
					GreaterThan(expression);
					break;
				case ExpressionType.GreaterThanOrEqual:
					GreaterThanOrEqual(expression);
					break;
				case ExpressionType.Index:
					Index(expression);
					break;
				case ExpressionType.Invoke:
					Invoke(expression);
					break;
				case ExpressionType.TypeIs:
					TypeIs(expression);
					break;
				case ExpressionType.Lambda:
					Lambda(expression);
					break;
				case ExpressionType.LessThanOrEqual:
					LessThanOrEqual(expression);
					break;
				case ExpressionType.ArrayLength:
					ArrayLength(expression);
					break;
				case ExpressionType.ListInit:
					ListInit(expression);
					break;
				case ExpressionType.LShift:
					LShift(expression);
					break;
				case ExpressionType.LessThan:
					LessThan(expression);
					break;
				case ExpressionType.MemberAccess:
					MemberAccess(expression);
					break;
				case ExpressionType.MemberInit:
					MemberInit(expression);
					break;
				case ExpressionType.MethodCall:
					MethodCall(expression);
					break;
				case ExpressionType.MethodCallVirtual:
					MethodCallVirtual(expression);
					break;
				case ExpressionType.Modulo:
					Modulo(expression);
					break;
				case ExpressionType.Multiply:
					Multiply(expression);
					break;
				case ExpressionType.MultiplyChecked:
					MultiplyChecked(expression);
					break;
				case ExpressionType.Negate:
					Negate(expression);
					break;
				case ExpressionType.NotEqual:
					NotEqual(expression);
					break;
				case ExpressionType.New:
					New(expression);
					break;
				case ExpressionType.NewArrayInit:
					NewArrayInit(expression);
					break;
				case ExpressionType.NewArrayBounds:
					NewArrayBounds(expression);
					break;
				case ExpressionType.Not:
					Not(expression);
					break;
				case ExpressionType.Or:
					Or(expression);
					break;
				case ExpressionType.OrElse:
					OrElse(expression);
					break;
				case ExpressionType.Parameter:
					Parameter(expression);
					break;
				case ExpressionType.Quote:
					Quote(expression);
					break;
				case ExpressionType.RShift:
					RShift(expression);
					break;
				case ExpressionType.Subtract:
					Subtract(expression);
					break;
				case ExpressionType.SubtractChecked:
					SubtractChecked(expression);
					break;
			}
			*/
			#endregion
		}

		private void UnaryPlus(Expression expression)
		{
			throw new NotImplementedException();
		}

		private void ExclusiveOr(Expression expression)
		{
			throw new NotImplementedException();
		}

		private void LeftShift(Expression expression)
		{
			throw new NotImplementedException();
		}

		private void NegateChecked(Expression expression)
		{
			throw new NotImplementedException();
		}

		private void Power(Expression expression)
		{
			throw new NotImplementedException();
		}

		private void RightShift(Expression expression)
		{
			throw new NotImplementedException();
		}

		private void QueryAppend(string fmt, params object[] args)
		{
			stringBuilder.AppendFormat(fmt, args);
			Console.WriteLine(string.Format(fmt, args));
		}

		public string InstancePlaceholderName
		{
			get
			{
				return "?" + Sanitise(typeof(T).Name);
			}
		}

		private string Sanitise(string s)
		{
			return s.Replace("<", "").Replace(">", "").Replace("'", "");
		}

		public static readonly string tripleFormatString = "{0} <{1}> {2} .\n";
		public static readonly string tripleFormatStringLiteral = "{0} <{1}> \"{2}\" .\n";
		private HashSet<MemberInfo> parameters;

		#region Node Handlers

		public void Add(Expression e)
		{
			//QueryAppend("+ :{0} Handled", e.NodeType);
			throw new NotImplementedException("operation Add not supported");
		}

		public void AddChecked(Expression e)
		{
			//QueryAppend("+ :{0} Handled", e.NodeType);
			throw new NotImplementedException("operation AddChecked not supported");
		}

		public void And(Expression e)
		{
			//QueryAppend("+ :{0} Handled", e.NodeType);
			throw new NotImplementedException("operation And not supported");
		}

		public void AndAlso(Expression e)
		{
			BinaryExpression be = e as BinaryExpression;
			if (be != null)
			{
				Dispatch(be.Left);
				Dispatch(be.Right);
			}
			Console.WriteLine(string.Format("+ :{0} Handled", e.NodeType));
		}

		public void TypeAs(Expression e)
		{
			//QueryAppend("+ :{0} Handled", e.NodeType);
			throw new NotImplementedException("operation TypeAs not supported");
		}

		public void BitwiseAnd(Expression e)
		{
			//QueryAppend("+ :{0} Handled", e.NodeType);
			throw new NotImplementedException("operation BitwiseAnd not supported");
		}

		public void BitwiseNot(Expression e)
		{
			//QueryAppend("+ :{0} Handled", e.NodeType);
			throw new NotImplementedException("operation BitwiseNot not supported");
		}

		public void BitwiseOr(Expression e)
		{
			//QueryAppend("+ :{0} Handled", e.NodeType);
			throw new NotImplementedException("operation BitwiseOr not supported");
		}

		public void BitwiseXor(Expression e)
		{
			//QueryAppend("+ :{0} Handled", e.NodeType);
			throw new NotImplementedException("operation BitwiseXor not supported");
		}

		public void Cast(Expression e)
		{
			//QueryAppend("+ :{0} Handled", e.NodeType);
			throw new NotImplementedException("operation Cast not supported");
		}

		public void Coalesce(Expression e)
		{
			//QueryAppend("+ :{0} Handled", e.NodeType);
			throw new NotImplementedException("operation Coalesce not supported");
		}

		public void Conditional(Expression e)
		{
			//QueryAppend("+ :{0} Handled", e.NodeType);
			throw new NotImplementedException("operation Conditional not supported");
		}

		public void Constant(Expression e)
		{
			//QueryAppend("+ :{0} Handled", e.NodeType);
			throw new NotImplementedException("operation Constant not supported");
		}

		public void Convert(Expression e)
		{
			//QueryAppend("+ :{0} Handled", e.NodeType);
			throw new NotImplementedException("operation Convert not supported");
		}

		public void ConvertChecked(Expression e)
		{
			//QueryAppend("+ :{0} Handled", e.NodeType);
			throw new NotImplementedException("operation ConvertChecked not supported");
		}

		public void Divide(Expression e)
		{
			//QueryAppend("+ :{0} Handled", e.NodeType);
			throw new NotImplementedException("operation Divide not supported");
		}

		public void Equal(Expression e)
		{
			BinaryExpression be = e as BinaryExpression;
			if (be != null)
			{
				MemberExpression me = be.Left as MemberExpression;
				ConstantExpression ce = be.Right as ConstantExpression;
				QueryAppend(tripleFormatStringLiteral, InstancePlaceholderName, me.Member.GetOwlResourceUri(), ce.Value.ToString());
				Console.WriteLine(string.Format(tripleFormatStringLiteral, be.Left, "=", be.Right));
			}
			MethodCallExpression mce = e as MethodCallExpression;
			if (mce != null && mce.Method.Name == "op_Equality")
			{
				MemberExpression me = mce.Arguments[0] as MemberExpression;
				ConstantExpression ce = mce.Arguments[1] as ConstantExpression;
                QueryAppend(tripleFormatStringLiteral, InstancePlaceholderName, me.Member.GetOwlResourceUri(), ce.Value.ToString());
                Console.WriteLine(string.Format(tripleFormatStringLiteral, InstancePlaceholderName, me.Member.GetOwlResourceUri(), ce.Value.ToString()));
			}
			Console.WriteLine(string.Format("+ :{0} Handled", e.NodeType));
		}

		public void Funclet(Expression e)
		{
			//QueryAppend("+ :{0} Handled", e.NodeType);
			throw new NotImplementedException("operation Funclet not supported");
		}

		public void GreaterThan(Expression e)
		{
			BinaryExpression be = e as BinaryExpression;
			if (be != null)
			{
				QueryAppend(tripleFormatString, be.Left, " > ", be.Right);
				Console.WriteLine(string.Format(tripleFormatString, InstancePlaceholderName+"!"+be.Left, ">", be.Right));
			}
			Console.WriteLine(string.Format("+ :{0} Handled", e.NodeType));
			/*
			throw new NotImplementedException("operation > is not supported");
			*/
		}

		public void GreaterThanOrEqual(Expression e)
		{
			/*
						BinaryExpression be = e as BinaryExpression;
						if (be != null)
						{
							QueryAppend(tripleFormatString, be.Left.ToString(), be.Right.ToString());
							Console.WriteLine(string.Format(tripleFormatString, InstancePlaceholderName + "!" + be.Left, ">=", be.Right));
						}
						Console.WriteLine(string.Format("+ :{0} Handled", e.NodeType));
			*/
			throw new NotImplementedException("operation >= is not supported");
		}

		public void Index(Expression e)
		{
			//QueryAppend("+ :{0} Handled", e.NodeType);
			throw new NotImplementedException("operation Index not supported");
		}

		public void Invoke(Expression e)
		{
			//QueryAppend("+ :{0} Handled", e.NodeType);
			throw new NotImplementedException("operation Invoke not supported");
		}

		public void TypeIs(Expression e)
		{
			//QueryAppend("+ :{0} Handled", e.NodeType);
			throw new NotImplementedException("operation TypeIs not supported");
		}

		public void Lambda(Expression e)
		{
			Dispatch(((LambdaExpression)e).Body);
		}

		public void LessThanOrEqual(Expression e)
		{
			//QueryAppend("+ :{0} Handled", e.NodeType);
			throw new NotImplementedException("operation LessThanOrEqual not supported");
		}

		public void ArrayLength(Expression e)
		{
			//QueryAppend("+ :{0} Handled", e.NodeType);
			throw new NotImplementedException("operation ArrayLength not supported");
		}

		public void ListInit(Expression e)
		{
			//QueryAppend("+ :{0} Handled", e.NodeType);
			throw new NotImplementedException("operation ListInit not supported");
		}

		public void LShift(Expression e)
		{
			//QueryAppend("+ :{0} Handled", e.NodeType);
			throw new NotImplementedException("operation LShift not supported");
		}

		public void LessThan(Expression e)
		{
			//QueryAppend("+ :{0} Handled", e.NodeType);
			throw new NotImplementedException("operation LessThan not supported");
		}

		public void MemberAccess(Expression e)
		{
			//QueryAppend("+ :{0} Handled", e.NodeType);
			throw new NotImplementedException("operation MemberAccess not supported");
		}

		public void MemberInit(Expression e)
		{
			//QueryAppend("+ :{0} Handled", e.NodeType);
			throw new NotImplementedException("operation MemberInit not supported");
		}

		public void MethodCall(Expression e)
		{
			MethodCallExpression mce = e as MethodCallExpression;
			if (mce != null)
			{
				switch (mce.Method.Name)
				{
					case "op_Equality":
						Equal(e);
						break;
				}
			}
			Console.WriteLine(string.Format("+ :{0} Handled", e.NodeType));
		}

		public void MethodCallVirtual(Expression e)
		{
			//QueryAppend("+ :{0} Handled", e.NodeType);
			throw new NotImplementedException("operation MethodCallVirtual not supported");
		}

		public void Modulo(Expression e)
		{
			//QueryAppend("+ :{0} Handled", e.NodeType);
			throw new NotImplementedException("operation Modulo not supported");
		}

		public void Multiply(Expression e)
		{
			//QueryAppend("+ :{0} Handled", e.NodeType);
			throw new NotImplementedException("operation Multiply not supported");
		}

		public void MultiplyChecked(Expression e)
		{
			//QueryAppend("+ :{0} Handled", e.NodeType);
			throw new NotImplementedException("operation MultiplyChecked not supported");
		}

		public void Negate(Expression e)
		{
			//QueryAppend("+ :{0} Handled", e.NodeType);
			throw new NotImplementedException("operation Negate not supported");
		}

		public void NotEqual(Expression e)
		{
			//QueryAppend("+ :{0} Handled", e.NodeType);
			throw new NotImplementedException("operation NotEqual not supported");
		}

		public void New(Expression e)
		{
			//QueryAppend("+ :{0} Handled", e.NodeType);
			throw new NotImplementedException("operation New not supported");
		}

		public void NewArrayInit(Expression e)
		{
			//QueryAppend("+ :{0} Handled", e.NodeType);
			throw new NotImplementedException("operation NewArrayInit not supported");
		}

		public void NewArrayBounds(Expression e)
		{
			//QueryAppend("+ :{0} Handled", e.NodeType);
			throw new NotImplementedException("operation NewArrayBounds not supported");
		}

		public void Not(Expression e)
		{
			//QueryAppend("+ :{0} Handled", e.NodeType);
			throw new NotImplementedException("operation Not not supported");
		}

		public void Or(Expression e)
		{
			//QueryAppend("+ :{0} Handled", e.NodeType);
			throw new NotImplementedException("operation Or not supported");
		}

		public void OrElse(Expression e)
		{
			/*
						BinaryExpression be = e as BinaryExpression;
						if (be != null)
						{
							Dispatch(be.Left);
							Dispatch(be.Right);
						}
						Console.WriteLine(string.Format("+ :{0} Handled", e.NodeType));
			*/
			throw new NotImplementedException("Operator not supported");
		}

		public void Parameter(Expression e)
		{
			//QueryAppend("+ :{0} Handled", e.NodeType);
			throw new NotImplementedException("operation Parameter not supported");
		}

		public void Quote(Expression e)
		{
			//QueryAppend("+ :{0} Handled", e.NodeType);
			UnaryExpression ue = (UnaryExpression)e;
			Dispatch(ue.Operand);
		}

		public void RShift(Expression e)
		{
			//QueryAppend("+ :{0} Handled", e.NodeType);
			throw new NotImplementedException("operation RShift not supported");
		}

		public void Subtract(Expression e)
		{
			//QueryAppend("+ :{0} Handled", e.NodeType);
			throw new NotImplementedException("operation Subtract not supported");
		}

		public void SubtractChecked(Expression e)
		{
			//QueryAppend("+ :{0} Handled", e.NodeType);
			throw new NotImplementedException("operation SubtractChecked not supported");
		}

		#endregion
	}
}
