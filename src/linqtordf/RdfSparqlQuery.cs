using System;
using System.Collections;
using System.Collections.Generic;
using System.Expressions;
using System.Query;
using System.Text;
using C5;

namespace RdfSerialisation
{
	public class RdfSparqlQuery<T> : QuerySupertype<T>, IRdfQuery<T>
	{
		public RdfSparqlQuery(string sparqlEndpoint)
		{
			this.sparqlEndpoint = sparqlEndpoint;
			originalType = typeof (T);
			parser = new SparqlExpressionTranslator<T>();
		}

		private Expression expression;

		private SparqlExpressionTranslator<T> parser;

		private readonly string sparqlEndpoint;

		private List<T> result = null;

		public Type ElementType
		{
			get { return originalType; }
		}

		public Expression Expression
		{
			get { return Expression.Constant(this); }
		}

		public SparqlExpressionTranslator<T> Parser
		{
			get { return parser; }
			set { parser = value; }
		}

		public string SparqlEndpoint
		{
			get { return sparqlEndpoint; }
		}

		public IQueryable<S> CreateQuery<S>(Expression expression)
		{
			RdfSparqlQuery<S> newQuery = new RdfSparqlQuery<S>(sparqlEndpoint);
			newQuery.OriginalType = originalType;
			newQuery.Projection = projection;
			newQuery.Properties = properties;
			newQuery.Query = Query;
			newQuery.Logger = logger;
			newQuery.Parser = new SparqlExpressionTranslator<S>(new StringBuilder(parser.StringBuilder.ToString()));

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
			// establish connection to the sparql endpoint
			// put finishing touches to the query
			// present the query to the endpoint
			// retrieve the results
			// yield the results
			throw new NotImplementedException();
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

		private void BuildQuery(Expression q)
		{
			StringBuilder sb = new StringBuilder();
			ParseQuery(q, sb);
			Query = Parser.StringBuilder.ToString();
			Log(Query);
		}

		private void ParseQuery(Expression expression, StringBuilder sb)
		{
			sb.Append("#Query - " + DateTime.Now.ToLongTimeString());
			Parser.Dispatch(expression);
		}
	}
}