using System;
using System.Collections;
using System.Collections.Generic;
using System.Expressions;
using System.Query;
using System.Reflection;
using System.Text;

namespace LinqToRdf
{
	public class RdfSparqlQuery<T> : QuerySupertype<T>, IRdfQuery<T>
	{
		public RdfSparqlQuery()
		{
			this.sparqlEndpoint = "";
			originalType = typeof (T);
			parser = new LinqToSparqlExpTranslator<T>();
		}

		private Expression expression;

		private IQueryFormatTranslator parser;

		private string sparqlEndpoint;

		private List<T> result = null;

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
			RdfSparqlQuery<S> newQuery = CloneQueryForNewType<S>();

			MethodCallExpression call = expression as MethodCallExpression;
			if (call != null)
			{
				switch (call.Method.Name)
				{
					case "Where":
						Log("Processing the where expression");
						newQuery.BuildQuery(call.Parameters[1]);
						break;
					case "Select":
						Log("Processing the select expression");
						newQuery.BuildProjection(call);
						break;
				}
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
			StringBuilder sbPrefixes = new StringBuilder();
			StringBuilder sbClauses = new StringBuilder();
			StringBuilder sbQuery = new StringBuilder();

			foreach (string prefix in namespaceManager.namespaceUris.Keys)
			{
				sbPrefixes.AppendFormat("@prefix {0}: <{1}> .\n", prefix, namespaceManager.namespaceUris[prefix]);
			}
			var ppq = from pi in originalType.GetProperties()
					where pi.GetCustomAttributes(typeof(OwlPropertyAttribute), true).Length == 1
					select pi;
			foreach (PropertyInfo propInfo in ppq)
			{
				sbClauses.AppendFormat("?{0} <{1}> ?{2} .\n", originalType.Name, OwlClassSupertype.GetPropertyUri(originalType, propInfo.Name), propInfo.Name);
			}
			sbQuery.AppendFormat("{0}\nSELECT {1} \nWHERE\n{{ {2} \n FILTER{{ {3} }}}}", sbPrefixes, GetParameterString(), sbClauses, Query);
			Query = sbQuery.ToString();
			// establish connection to the sparql endpoint
			// put finishing touches to the query
			// present the query to the endpoint
			// retrieve the results
			// yield the results
			return result.GetEnumerator();
		}

		private string GetParameterString()
		{
			List<string> args = new List<string>(from p in properties select "?"+p.Name);
			return string.Join(", ", args.ToArray());
		}

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
			throw new NotImplementedException();
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
			StringBuilder sb = new StringBuilder();
			ParseQuery(q, sb);
			Query = sb.ToString();
			Log(Query);
		}

		protected void ParseQuery(Expression expression, StringBuilder sb)
		{
			Log("#Query {0:d}", DateTime.Now);
			Parser.StringBuilder = sb;
			Parser.Dispatch(expression);
		}

		protected RdfSparqlQuery<S> CloneQueryForNewType<S>()
		{
			RdfSparqlQuery<S> newQuery = new RdfSparqlQuery<S>();
			newQuery.SparqlEndpoint = sparqlEndpoint;
			newQuery.OriginalType = originalType;
			newQuery.Projection = projection;
			newQuery.Properties = properties;
			newQuery.Query = Query;
			newQuery.Logger = logger;
			newQuery.QueryFactory = new QueryFactory<S>(QueryFactory.QueryType);
			newQuery.Parser = QueryFactory.CreateExpressionTranslator();
			newQuery.Parser.StringBuilder = new StringBuilder(parser.StringBuilder.ToString());
			return newQuery;
		}
	}
}