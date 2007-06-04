using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Expressions;
using System.Query;
using System.Reflection;
using System.Text;

namespace LinqToRdf.Sparql
{
	public class SparqlQuery<T> : QuerySupertype<T>, IRdfQuery<T>
	{
		public SparqlQuery()
		{
			this.sparqlEndpoint = "";
			originalType = typeof(T);
			parser = new LinqToSparqlExpTranslator<T>();
		}

		private Expression expression;

		private IQueryFormatTranslator parser;

		private string sparqlEndpoint;

		public Type ElementType
		{
			get { return originalType; }
		}

		public Expression Expression
		{
			get { return Expression.Constant(this); }
		}

		public IQueryFormatTranslator Parser
		{
			get { return parser; }
			set { parser = value; }
		}

		public string SparqlEndpoint
		{
			get { return sparqlEndpoint; }
			set { sparqlEndpoint = value; }
		}

		public IQueryable<S> CreateQuery<S>(Expression expression)
		{
			SparqlQuery<S> newQuery = CloneQueryForNewType<S>();

			MethodCallExpression call = expression as MethodCallExpression;
			if (call != null)
			{
				newQuery.Expressions[call.Method.Name] = call;
			}
			return newQuery;
		}

		public S Execute<S>(Expression expression)
		{
			this.expression = expression;
			throw new NotImplementedException("Execute not implmented");
		}

		///<summary>
		///Returns an enumerator that iterates through the collection.
		///</summary>
		///
		///<returns>
		///A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
		///</returns>
		///<filterpriority>1</filterpriority>
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			StringBuilder sb = new StringBuilder();
			CreateQuery(sb);
			using (IRdfConnection<T> conn = QueryFactory.CreateConnection(this))
			{
				IRdfCommand<T> cmd = conn.CreateCommand();
				cmd.CommandText = Query = sb.ToString();
				return cmd.ExecuteQuery();
			}
		}

		#region SPARQL Query Construction

		private void CreateQuery(StringBuilder sb)
		{
			if(Expressions.ContainsKey("Where"))
			{
				// first parse the where expression to get the list of parameters to/from the query.
				StringBuilder sbTmp = new StringBuilder();
				ParseQuery(Expressions["Where"].Parameters[1], sbTmp);
				//sbTmp now contains the FILTER clause so save it somewhere useful.
				Query = sbTmp.ToString();
				// now store the parameters where they can be used later on.
				queryGraphParameters.AddAll(Parser.Parameters);
				// we need to add the original type to the prolog to allow elements of the where clause to be optimised
				namespaceManager.RegisterType(OriginalType);
			}
			CreateProlog(sb);
			CreateDataSetClause(sb);
			CreateProjection(sb);
			CreateWhereClause(sb);
			CreateSolutionModifier(sb);
		}

		private void CreateProlog(StringBuilder sb)
		{
			// insert the standard prefixes as per http://www.w3.org/TR/rdf-sparql-query/#docNamespaces
			sb.Append("@prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .\n");
			sb.Append("@prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#> .\n");
			sb.Append("@prefix xsdt: <http://www.w3.org/2001/XMLSchema#> .\n"); // for the datatypes
			sb.Append("@prefix fn: <http://www.w3.org/2005/xpath-functions#>  .\n");

			// now insert namespaces needed for the OwlClasses we're working with in this query
			foreach (string prefix in namespaceManager.namespaceUris.Keys)
			{
				sb.AppendFormat("@prefix {0}: <{1}> .\n", prefix, namespaceManager.namespaceUris[prefix]);
			}
			sb.Append("\n");
		}

		private void CreateDataSetClause(StringBuilder sb)
		{
			return; // no named graphs just yet (issue #12 created - http://code.google.com/p/linqtordf/issues/detail?id=12&can=2&q=)
		}

		private void CreateProjection(StringBuilder sb)
		{
			BuildProjection(Expressions["Select"]);
			List<string> argList = new List<string>(from p in projectionParameters select "?"+p.Name);
			if(argList.Count > 0)
			{
				string projection = string.Join(" ", argList.ToArray());
				sb.AppendFormat("SELECT {0} \n", projection) ;
			}
			else
			{ // this creates a projection for all the variables that you asked for in the graph (http://www.w3.org/TR/rdf-sparql-query/#select)
				sb.Append("SELECT * \n");
			}
		}

		/// <summary>
		/// Get all of the original properties that take part in the projection
		/// </summary>
		/// <returns>enums memberinfo of each property</returns>
		IEnumerable<MemberInfo> GetProjectedOriginalProperties()
		{
			ReadOnlyCollection<Expression> collection = Expressions["Where"].Parameters;
			yield break;
		}

		private void CreateWhereClause(StringBuilder sb)
		{
			string instanceName = GetInstanceName();
			sb.Append("WHERE {\n");
			foreach (MemberInfo info in queryGraphParameters.Union(projectionParameters))
			{
				sb.AppendFormat("?{0} {1}{2} ?{3} .\n", instanceName, namespaceManager.typeMappings[originalType]+":",OwlClassSupertype.GetPropertyUri(originalType, info.Name, true), info.Name);
			}
			if (Query != null && Query.Length > 0)
			{
				sb.AppendFormat("FILTER {{\n{0}\n}}\n", Query);
			}
			sb.Append("}\n");
		}

		private string GetInstanceName()
		{
			MethodCallExpression whereExp = Expressions["Where"];
			LambdaExpression le = (LambdaExpression)Expressions["Where"].Parameters[1];
			ParameterExpression instance = le.Parameters[0];
			return instance.Name;
		}

		private void CreateSolutionModifier(StringBuilder sb)
		{
			CreateOrderClause(sb);
			CreateLimitClause(sb);
			CreateOffsetClause(sb);
		}

		private void CreateOrderClause(StringBuilder sb)
		{
			if (Expressions.ContainsKey("OrderBy"))
			{
				MethodCallExpression orderExp = Expressions["OrderBy"];
				LambdaExpression descriminatingFunction = (LambdaExpression)orderExp.Parameters[1];
				MemberExpression me = (MemberExpression)descriminatingFunction.Body;
				sb.AppendFormat("ORDER BY ?{0}\n", me.Member.Name);
			}
		}

		private void CreateLimitClause(StringBuilder sb)
		{
			if (Expressions.ContainsKey("Take"))
			{
				MethodCallExpression takeExpression = Expressions["Take"];
				ConstantExpression constantExpression = (ConstantExpression)takeExpression.Parameters[1];
				if (constantExpression.Value != null)
				{
					sb.AppendFormat("LIMIT {0}\n", constantExpression.Value);
				}
			}
		}

		private void CreateOffsetClause(StringBuilder sb)
		{
			if (Expressions.ContainsKey("Skip"))
			{
			MethodCallExpression skipExpression = Expressions["Skip"];
				ConstantExpression constantExpression = (ConstantExpression)skipExpression.Parameters[1];
				if (constantExpression.Value != null)
				{
					sb.AppendFormat("OFFSET {0}\n", constantExpression.Value);
				}
			}
		}

		#endregion

		///<summary>
		///Returns an enumerator that iterates through a collection.
		///</summary>
		///
		///<returns>
		///An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
		///</returns>
		///<filterpriority>2</filterpriority>
		public IEnumerator GetEnumerator()
		{
			using (IRdfConnection<T> conn = QueryFactory.CreateConnection(this))
			{
				IRdfCommand<T> cmd = conn.CreateCommand();
				cmd.CommandText = Query;
				return cmd.ExecuteQuery();
			}
		}

		public IQueryable CreateQuery(Expression expression)
		{
			throw new NotImplementedException();
		}

		public object Execute(Expression expression)
		{
			throw new NotImplementedException();
		}

		protected void BuildQuery(Expression q)
		{
			StringBuilder sbPrefixes = new StringBuilder();
			StringBuilder sbClauses = new StringBuilder();
			StringBuilder sbQuery = new StringBuilder();
			StringBuilder sbFilter = new StringBuilder();
			ParseQuery(q, sbFilter);

			foreach (string prefix in namespaceManager.namespaceUris.Keys)
			{
				sbPrefixes.AppendFormat("@prefix {0}: <{1}> .\n", prefix, namespaceManager.namespaceUris[prefix]);
			}

			foreach (PropertyInfo propInfo in OwlClassSupertype.GetAllPersistentProperties(originalType))
			{
				if (queryGraphParameters.Contains(propInfo))
					sbClauses.AppendFormat("?{0} <{1}> ?{2} .\n", originalType.Name, OwlClassSupertype.GetPropertyUri(originalType, propInfo.Name), propInfo.Name);
			}
			//			sbQuery.AppendFormat("{0}\nSELECT {1} \nWHERE\n{{ {2} \n FILTER{{ {3} }}\n}}", sbPrefixes, GetParameterString(), sbClauses, Query);
			Query = sbQuery.ToString();
			Trace.WriteLine(Query);
		}

		protected void ParseQuery(Expression expression, StringBuilder sb)
		{
			Log("#Query {0:d}", DateTime.Now);
			StringBuilder tmp = Parser.StringBuilder;
			Parser.StringBuilder = sb;
			Parser.Dispatch(expression);
			Parser.StringBuilder = tmp;
		}

		protected SparqlQuery<S> CloneQueryForNewType<S>()
		{
			SparqlQuery<S> newQuery = new SparqlQuery<S>();
			newQuery.SparqlEndpoint = sparqlEndpoint;
			newQuery.OriginalType = originalType;
			newQuery.Projection = projection;
			newQuery.QueryGraphParameters = queryGraphParameters;
			newQuery.Query = Query;
			newQuery.Logger = logger;
			newQuery.QueryFactory = new QueryFactory<S>(QueryFactory.QueryType);
			newQuery.Parser = QueryFactory.CreateExpressionTranslator();
			newQuery.Parser.StringBuilder = new StringBuilder(parser.StringBuilder.ToString());
			newQuery.Expressions = expressions;
			return newQuery;
		}
	}
}