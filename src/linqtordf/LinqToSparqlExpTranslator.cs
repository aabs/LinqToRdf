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
using LinqToRdf;
using System.Collections.Generic;

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
                case ExpressionType.ArrayIndex:
                    ArrayIndex(expression);
                    break;
                case ExpressionType.ArrayLength:
                    ArrayLength(expression);
                    break;
                case ExpressionType.Call:
                    Call(expression);
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

        }

        public void ExclusiveOr(Expression expression)
        {
            throw new NotImplementedException();
        }

        public void NegateChecked(Expression expression)
        {
            throw new NotImplementedException();
        }

        public void Power(Expression expression)
        {
            throw new NotImplementedException();
        }

        public void UnaryPlus(Expression expression)
        {
            throw new NotImplementedException();
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

        public void TypeAs(Expression e)
        {
            throw new NotImplementedException("operation TypeAs not supported");
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

        public void Equal(Expression e)
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
                lh = mce.Arguments[0];
                rh = mce.Arguments[1];
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
                Log("Failure during generation of Equal expression");
            }
        }

        public void Funclet(Expression e)
        {
            throw new NotImplementedException("operation Funclet not supported");
        }

        public void GreaterThan(Expression e)
        {
            Debug.Assert(e is BinaryExpression && e.NodeType == ExpressionType.GreaterThan);
            GenerateBinaryExpression(e, ">");
        }

        public void GreaterThanOrEqual(Expression e)
        {
            Debug.Assert(e is BinaryExpression && e.NodeType == ExpressionType.GreaterThanOrEqual);
            GenerateBinaryExpression(e, ">=");
        }

        public void ArrayIndex(Expression e)
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

        public void TypeIs(Expression e)
        {
            throw new NotImplementedException("operation TypeIs not supported");
        }

        public void Lambda(Expression e)
        {
            Dispatch(((LambdaExpression)e).Body);
        }

        public void LessThanOrEqual(Expression e)
        {
            Debug.Assert(e is BinaryExpression && e.NodeType == ExpressionType.LessThanOrEqual);
            GenerateBinaryExpression(e, "<=");
        }

        public void ArrayLength(Expression e)
        {
            throw new NotImplementedException("operation ArrayLength not supported");
        }

        public void ListInit(Expression e)
        {
            throw new NotImplementedException("operation ListInit not supported");
        }

        public void LeftShift(Expression e)
        {
            throw new NotImplementedException("operation LShift not supported");
        }

        public void LessThan(Expression e)
        {
            Debug.Assert(e is BinaryExpression && e.NodeType == ExpressionType.LessThan);
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

        public void Call(Expression e)
        {
            MethodCallExpression mce = (MethodCallExpression)e;
            if (mce.Method.DeclaringType == typeof(string))
            {
                ProcessStringOperations(mce);
                return;
            }
            switch (mce.Method.Name)
            {
                case "HavingSubjectUri":
                    // well we caught it. Now What?
                    // 1 get the parameter name of the child instance
                    MemberExpression me = (MemberExpression)mce.Arguments[0];
                    ParameterExpression pe = (ParameterExpression)me.Expression;
                    string name = pe.Name;
                    // 2 get the URI of the relationship
                    string relnUri = me.Member.GetOwlResourceUri();
                    // 3 get the URI of the parent instance
                    string instanceUri = ((ConstantExpression)mce.Arguments[1]).Value.ToString();

                    QueryAppend("${0} <{1}> <{2}>.", name, relnUri, instanceUri);

                    break;
                case "HasInstanceUri":
                    // well we caught it. Now What?
                    // 1 get the parameter name of the child instance
                    ParameterExpression pe2 = (ParameterExpression)mce.Arguments[0];
                    string name2 = pe2.Name;
                    // 2 get the URI of the relationship
                    
                    string relnUri2 = pe2.Type.GetOwlResourceUri();
                    // 3 get the URI of the parent instance
                    string instanceUri2 = ((ConstantExpression)mce.Arguments[1]).Value.ToString();

                    QueryAppend("${0} = <{2}>", name2, relnUri2, instanceUri2);

                    break;
                case "ToInt16":
                case "ToInt32":
                case "ToInt64":
                case "ToFloat":
                case "ToDouble":
                case "ToDecimal":
                    ProcessCastOperators(e);
                    break;
                case "op_Equality":
                    Equal(e);
                    break;
                default:
                    Dispatch(mce.Object);
                    QueryAppend("." + mce.Method.Name + "(");
                    string sep = "";
                    for (int i = 0; i < mce.Arguments.Count; i++)
                    {
                        QueryAppend(sep);
                        Dispatch(mce.Arguments[i]);
                        sep = ", ";
                    }
                    QueryAppend(")");
                    break;
            }
        }

        string SafeDispatch(Expression e)
        {
            StringBuilder currentStringBuilder = StringBuilder;
            try
            {
                StringBuilder = new StringBuilder();
                // we can be confident that this really is ontology cast operation, therefore there will be only one parameter
                Dispatch(e);
                return StringBuilder.ToString();
            }
            finally
            {
            StringBuilder = currentStringBuilder;
            }
        }

        private void ProcessCastOperators(Expression e)
        {
            MethodCallExpression mce = (MethodCallExpression)e;
            XsdtTypeConverter tc = new XsdtTypeConverter();
            string typeToCastTo = tc.GetXsdtAttrFor(mce.Type).Name;
            string argName = SafeDispatch(mce.Arguments[0]);
            QueryAppend("xsdt:{1}({0})", argName, typeToCastTo);
        }

        public void MethodCallVirtual(Expression e)
        {
            MethodCallExpression mce = (MethodCallExpression)e;
            MethodInfo mi = mce.Method;

            // is it eligible for ontology regex operation?
            if (mi.DeclaringType == typeof(string))
            {
                ProcessStringOperations(mce);
                return;
            }
            throw new NotImplementedException("operation MethodCallVirtual not supported for Method '" + mi.Name + "'");
        }

        private void ProcessStringOperations(MethodCallExpression mce)
        {
            MethodInfo mi = mce.Method;
            switch (mi.Name)
            {
                case "Contains":
                    GenerateRegexComparison(mce);
                    return;
                case "StartsWith":
                    GenerateRegexStartsWith(mce);
                    return;
                case "EndsWith":
                    GenerateRegexEndsWith(mce);
                    return;
            }

            throw new NotImplementedException("operation MethodCallVirtual not supported for Method '" + mi.Name + "'");
        }
        private void GenerateRegexStartsWith(MethodCallExpression mce)
        {
            ConstantExpression constantExpression = (ConstantExpression)mce.Arguments[0];
            MemberExpression memberExpression = (MemberExpression)mce.Object;
            QueryAppend("regex({0}, \"^{1}\") ", "?" + memberExpression.Member.Name, constantExpression.Value);
        }

        private void GenerateRegexEndsWith(MethodCallExpression mce)
        {
            ConstantExpression constantExpression = (ConstantExpression)mce.Arguments[0];
            MemberExpression memberExpression = (MemberExpression)mce.Object;
            QueryAppend("regex({0}, \"{1}$\") ", "?" + memberExpression.Member.Name, constantExpression.Value);
        }

        /// <summary>
        /// Create ontology regex string comparison
        /// </summary>
        /// <param name="mce">the MethodCallExpression for ontology string.Compare</param>
        /// <remarks>
        /// <see cref="http://www.w3.org/TR/xpath-functions/#regex-syntax"/> for acceptable regex syntax
        /// <see cref="http://www.w3.org/TR/xpath-functions/#func-matches"/> for usage hints
        /// Should produce ontology filter regex of the form <c>regex(?name, "^ali", "i")</c>.
        /// The <see cref="System.String.Compare"/> is case-sensitive and culture-insensitive
        /// </remarks>
        private void GenerateRegexComparison(MethodCallExpression mce)
        {
            ConstantExpression constantExpression = (ConstantExpression)mce.Arguments[0];
            MemberExpression memberExpression = (MemberExpression)mce.Object;
            QueryAppend("regex({0}, \"{1}\") ", "?" + memberExpression.Member.Name, constantExpression.Value.ToString());
        }

        private void GenerateRegex(MemberExpression memberExpression, ConstantExpression constantExpression)
        {
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

        public void NotEqual(Expression e)
        {
            Debug.Assert(e is BinaryExpression && e.NodeType == ExpressionType.NotEqual);
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
            ParameterExpression pe = (ParameterExpression)e;
            stringBuilder.Append(pe.Name);
        }

        public void Quote(Expression e)
        {
            UnaryExpression q = (UnaryExpression)e;
            QueryAppend("\"");
            Dispatch(q.Operand);
            QueryAppend("\"");
        }

        public void RightShift(Expression e)
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
