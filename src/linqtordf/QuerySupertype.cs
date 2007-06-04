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
		protected Dictionary<string, MethodCallExpression> expressions;
		protected TextWriter logger;
		protected NamespaceManager namespaceManager = new NamespaceManager();
		protected Type originalType = typeof(T);
		protected Delegate projection;
		protected HashSet<MemberInfo> queryGraphParameters = new HashSet<MemberInfo>();
		protected HashSet<MemberInfo> projectionParameters = new HashSet<MemberInfo>();
		protected string query;
		protected QueryFactory<T> queryFactory;

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

		public string Query
		{
			get { return query; }
			set { query = value; }
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
			Trace.WriteLine(string.Format("+ :"+msg, args));
		}

	}
}