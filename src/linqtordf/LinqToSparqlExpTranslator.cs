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
using System.Diagnostics;
using System.Expressions;
using System.Reflection;
using System.Text;
using C5;
using LinqToRdf;

namespace LinqToRdf.Sparql
{
	public class LinqToSparqlExpTranslator<T> : RdfExpressionTranslator<T>, IQueryFormatTranslator
	{
		public LinqToSparqlExpTranslator()
		{
			stringBuilder = new StringBuilder();
		}

		public LinqToSparqlExpTranslator(StringBuilder stringBuilder)
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
				case ExpressionType.As:
					As(expression);
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
				case ExpressionType.EQ:
					EQ(expression);
					break;
				case ExpressionType.Funclet:
					Funclet(expression);
					break;
				case ExpressionType.GT:
					GT(expression);
					break;
				case ExpressionType.GE:
					GE(expression);
					break;
				case ExpressionType.Index:
					Index(expression);
					break;
				case ExpressionType.Invoke:
					Invoke(expression);
					break;
				case ExpressionType.Is:
					Is(expression);
					break;
				case ExpressionType.Lambda:
					Lambda(expression);
					break;
				case ExpressionType.LE:
					LE(expression);
					break;
				case ExpressionType.Len:
					Len(expression);
					break;
				case ExpressionType.ListInit:
					ListInit(expression);
					break;
				case ExpressionType.LShift:
					LShift(expression);
					break;
				case ExpressionType.LT:
					LT(expression);
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
				case ExpressionType.NE:
					NE(expression);
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

		}

		private void GenerateBinaryExpression(Expression e, string op)
		{
			if (e == null)
				throw new ArgumentNullException("e was null");
			if (op == null)
				throw new ArgumentNullException("op was null");
			if (op.Length == 0)
				throw new ArgumentNullException("op.Length was empty");
			BinaryExpression be = e as BinaryExpression;
			if (be != null)
			{
				QueryAppend("(");
				Dispatch(be.Left);
				QueryAppend(")" + op + "(");
				Dispatch(be.Right);
				QueryAppend(")");
				Log("+ :{0} Handled", e.NodeType);
			}
		}

		public static readonly string tripleFormatString = "{0} <{1}> {2} .\n";
		public static readonly string tripleFormatStringLiteral = "{0} <{1}> \"{2}\" .\n";

		public HashSet<MemberInfo> Parameters
		{
			get
			{
				if (parameters == null)
					parameters = new HashSet<MemberInfo>();
				return parameters;
			}
			set { parameters = value; }
		}

		private HashSet<MemberInfo> parameters;

		#region Expression Node Handlers

		public void Add(Expression e)
		{
			Debug.Assert(e is BinaryExpression && e.NodeType == ExpressionType.Add);
			GenerateBinaryExpression(e, "+");
		}

		public void AddChecked(Expression e)
		{
			Debug.Assert(e is BinaryExpression && e.NodeType == ExpressionType.AddChecked);
			GenerateBinaryExpression(e, "+");
		}

		public void And(Expression e)
		{
			Debug.Assert(e is BinaryExpression && e.NodeType == ExpressionType.And);
			GenerateBinaryExpression(e, "&&");
		}

		public void AndAlso(Expression e)
		{
			Debug.Assert(e is BinaryExpression && e.NodeType == ExpressionType.AndAlso);
			GenerateBinaryExpression(e, "&&");
		}

		public void As(Expression e)
		{
			throw new NotImplementedException("operation As not supported");
		}

		public void BitwiseAnd(Expression e)
		{
			throw new NotImplementedException("operation Bitwise And not supported");
			/*
						BinaryExpression be = e as BinaryExpression;
						if (be != null)
						{
							QueryAppend("(");
							Dispatch(be.Left);
							QueryAppend(")&(");
							Dispatch(be.Right);
							QueryAppend(")");
						}
						Log("+ :{0} Handled", e.NodeType);
			*/
		}

		public void BitwiseNot(Expression e)
		{
			throw new NotImplementedException("operation BitwiseNot not supported");
		}

		public void BitwiseOr(Expression e)
		{
			throw new NotImplementedException("operation BitwiseOr not supported");
		}

		public void BitwiseXor(Expression e)
		{
			throw new NotImplementedException("operation BitwiseXor not supported");
		}

		public void Cast(Expression e)
		{
			throw new NotImplementedException("operation Cast not supported");
		}

		public void Coalesce(Expression e)
		{
			throw new NotImplementedException("operation Coalesce not supported");
		}

		public void Conditional(Expression e)
		{
			throw new NotImplementedException("operation Conditional not supported");
		}

		public void Constant(Expression e)
		{
			ConstantExpression ce = (ConstantExpression)e;
			QueryAppend(TypeTranslator.Get(e.Type, ce.Value).ToString());
		}

		public void Convert(Expression e)
		{
			throw new NotImplementedException("operation Convert not supported");
		}

		public void ConvertChecked(Expression e)
		{
			throw new NotImplementedException("operation ConvertChecked not supported");
		}

		public void Divide(Expression e)
		{
			BinaryExpression be = e as BinaryExpression;
			if (be != null)
			{
				QueryAppend("(");
				Dispatch(be.Left);
				QueryAppend(")/(");
				Dispatch(be.Right);
				QueryAppend(")");
			}
			Log("+ :{0} Handled", e.NodeType);
		}

		public void EQ(Expression e)
		{
			Expression lh;
			Expression rh;
			if (e is BinaryExpression)
			{
				BinaryExpression be = e as BinaryExpression;
				lh = be.Left;
				rh = be.Right;
			}
			else if (e is MethodCallExpression)
			{
				MethodCallExpression mce = e as MethodCallExpression;
				lh = mce.Parameters[0];
				rh = mce.Parameters[1];
			}
			else
			{
				throw new ApplicationException("Unrecognised equality expression type");
			}

			if (lh != null && rh != null)
			{
				XsdtPrimitiveDataType dt = TypeTranslator.GetDataType(lh.Type);
				if (dt == XsdtPrimitiveDataType.XsdtString)
				{
					QueryAppend("regex(");
					Dispatch(lh);
					QueryAppend(", ");
					Dispatch(rh);
					QueryAppend(") ");
				}
				else
				{
					//QueryAppend("(");
					Dispatch(lh);
					QueryAppend(" = ");
					//QueryAppend(")=(");
					Dispatch(rh);
					//QueryAppend(")");
					//Log("+ :{0} Handled", e.NodeType);
				}
			}
			else
			{
				Log("Failure during generation of EQ expression");
			}
		}

		public void Funclet(Expression e)
		{
			throw new NotImplementedException("operation Funclet not supported");
		}

		public void GT(Expression e)
		{
			Debug.Assert(e is BinaryExpression && e.NodeType == ExpressionType.GT);
			GenerateBinaryExpression(e, ">");
		}

		public void GE(Expression e)
		{
			Debug.Assert(e is BinaryExpression && e.NodeType == ExpressionType.GE);
			GenerateBinaryExpression(e, ">=");
		}

		public void Index(Expression e)
		{
			BinaryExpression be = (BinaryExpression)e;
			Dispatch(be.Left);
			QueryAppend("[");
			Dispatch(be.Right);
			QueryAppend("]");
		}

		public void Invoke(Expression e)
		{
			throw new NotImplementedException("operation Invoke not supported");
		}

		public void Is(Expression e)
		{
			throw new NotImplementedException("operation Is not supported");
		}

		public void Lambda(Expression e)
		{
			Dispatch(((LambdaExpression)e).Body);
		}

		public void LE(Expression e)
		{
			Debug.Assert(e is BinaryExpression && e.NodeType == ExpressionType.LE);
			GenerateBinaryExpression(e, "<=");
		}

		public void Len(Expression e)
		{
			throw new NotImplementedException("operation Len not supported");
		}

		public void ListInit(Expression e)
		{
			throw new NotImplementedException("operation ListInit not supported");
		}

		public void LShift(Expression e)
		{
			throw new NotImplementedException("operation LShift not supported");
		}

		public void LT(Expression e)
		{
			Debug.Assert(e is BinaryExpression && e.NodeType == ExpressionType.LT);
			GenerateBinaryExpression(e, "<");
		}

		public void MemberAccess(Expression e)
		{
			MemberExpression me = e as MemberExpression;
			if (me != null)
			{
				stringBuilder.Append("?" + me.Member.Name);
				if (me.Member.MemberType == MemberTypes.Property)
					Parameters.Add(me.Member);
			}
		}

		public void MemberInit(Expression e)
		{
			throw new NotImplementedException("operation MemberInit not supported");
		}

		public void MethodCall(Expression e)
		{
			MethodCallExpression mce = (MethodCallExpression)e;
			switch (mce.Method.Name)
			{
				case "op_Equality":
					EQ(e);
					break;
				default:
					Dispatch(mce.Object);
					QueryAppend("." + mce.Method.Name + "(");
					string sep = "";
					for (int i = 0; i < mce.Parameters.Count; i++)
					{
						QueryAppend(sep);
						Dispatch(mce.Parameters[i]);
						sep = ", ";
					}
					QueryAppend(")");
					break;
			}
			Log("+ :{0} Handled", e.NodeType);
		}

		public void MethodCallVirtual(Expression e)
		{
			throw new NotImplementedException("operation MethodCallVirtual not supported");
		}

		public void Modulo(Expression e)
		{
			throw new NotImplementedException("operation Modulo not supported");
		}

		public void Multiply(Expression e)
		{
			Debug.Assert(e is BinaryExpression && e.NodeType == ExpressionType.Multiply);
			GenerateBinaryExpression(e, "*");
		}

		public void MultiplyChecked(Expression e)
		{
			Debug.Assert(e is BinaryExpression && e.NodeType == ExpressionType.MultiplyChecked);
			GenerateBinaryExpression(e, "*");
		}

		public void Negate(Expression e)
		{
			throw new NotImplementedException("operation Negate not supported");
		}

		public void NE(Expression e)
		{
			Debug.Assert(e is BinaryExpression && e.NodeType == ExpressionType.NE);
			GenerateBinaryExpression(e, "!=");
		}

		public void New(Expression e)
		{
			throw new NotImplementedException("operation New not supported");
		}

		public void NewArrayInit(Expression e)
		{
			throw new NotImplementedException("operation NewArrayInit not supported");
		}

		public void NewArrayBounds(Expression e)
		{
			throw new NotImplementedException("operation NewArrayBounds not supported");
		}

		public void Not(Expression e)
		{
			Debug.Assert(e is UnaryExpression && e.NodeType == ExpressionType.Not);
			UnaryExpression ue = (UnaryExpression)e;
			QueryAppend("!(");
			Dispatch(ue.Operand);
			QueryAppend(")");
		}

		public void Or(Expression e)
		{
			Debug.Assert(e is BinaryExpression && e.NodeType == ExpressionType.Or);
			GenerateBinaryExpression(e, "||");
		}

		public void OrElse(Expression e)
		{
			Debug.Assert(e is BinaryExpression && e.NodeType == ExpressionType.OrElse);
			GenerateBinaryExpression(e, "||");
		}

		public void Parameter(Expression e)
		{
			e.BuildString(stringBuilder);
		}

		public void Quote(Expression e)
		{
			UnaryExpression q = (UnaryExpression)e;
			QueryAppend("\"");
			Dispatch(q.Operand);
			QueryAppend("\"");
		}

		public void RShift(Expression e)
		{
			throw new NotImplementedException("operation RShift not supported");
		}

		public void Subtract(Expression e)
		{
			Debug.Assert(e is BinaryExpression && e.NodeType == ExpressionType.Subtract);
			GenerateBinaryExpression(e, "-");
		}

		public void SubtractChecked(Expression e)
		{
			Debug.Assert(e is BinaryExpression && e.NodeType == ExpressionType.SubtractChecked);
			GenerateBinaryExpression(e, "-");
		}

		#endregion
	}
}
