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
using System.Collections.Generic;
using System.Diagnostics;
using System.Expressions;
using System.IO;
using System.Reflection;
using System.Text;
using C5;

namespace LinqToRdf
{
	public class QuerySupertype<T>{
		protected IRdfContext context;
		protected Dictionary<string, MethodCallExpression> expressions;
		protected TextWriter logger;
		protected NamespaceManager namespaceManager = new NamespaceManager();
		protected Type originalType = typeof(T);
		protected Delegate projection;
		protected HashSet<MemberInfo> queryGraphParameters = new HashSet<MemberInfo>();
		protected HashSet<MemberInfo> projectionParameters = new HashSet<MemberInfo>();
		protected string filterClause;
		protected QueryFactory<T> queryFactory;

		public IRdfContext Context
		{
			get { return context; }
		}

		public Dictionary<string, MethodCallExpression> Expressions
		{
			get
			{
				if(expressions == null)
					expressions = new Dictionary<string, MethodCallExpression>();
				return expressions;
			}
			set { expressions = value; }
		}

		public TextWriter Logger
		{
			get { return logger; }
			set { logger = value; }
		}

		public Type OriginalType
		{
			get { return originalType; }
			set { originalType = value; }
		}

		public HashSet<MemberInfo> QueryGraphParameters
		{
			get { return queryGraphParameters; }
			set { queryGraphParameters = value; }
		}

		public HashSet<MemberInfo> ProjectionParameters
		{
			get { return projectionParameters; }
			set { projectionParameters = value; }
		}

		public Delegate Projection
		{
			get { return projection; }
			set { projection = value; }
		}

		public string FilterClause
		{
			get { return filterClause; }
			set { filterClause = value; }
		}

		public QueryFactory<T> QueryFactory
		{
			get { return queryFactory; }
			set { queryFactory = value; }
		}

		protected void BuildProjection(Expression expression)
		{
			LambdaExpression le = ((MethodCallExpression)expression).Parameters[1] as LambdaExpression;
			if (le == null) throw new ApplicationException("Incompatible expression type found when building a projection");
			projection = le.Compile();
			MemberInitExpression mie = le.Body as MemberInitExpression;
			if (mie != null)
				foreach (Binding b in mie.Bindings)
					FindProperties(b);
			else
				foreach (PropertyInfo i in originalType.GetProperties())
					projectionParameters.Add(i);
		}

		private void FindProperties(Binding e)
		{
			namespaceManager.RegisterType(OriginalType);
			switch (e.BindingType)
			{
				case BindingType.MemberAssignment:
					projectionParameters.Add(e.Member);
					break;
				case BindingType.MemberListBinding:
					break;
				case BindingType.MemberMemberBinding:
					projectionParameters.Add(e.Member);
					break;
			}
		}

		internal void Log(string msg, params object[] args)
		{
			if(Logger != null)
				logger.WriteLine(string.Format("+ :"+msg, args));
		}
	}
}