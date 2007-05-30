using System;
using System.Collections;
using System.Collections.Generic;
using System.Expressions;
using System.IO;
using System.Query;
using System.Reflection;
using System.Text;
using SemWeb;
using SemWeb.Query;

namespace LinqToRdf
{
	public class RdfN3Query<T> : QuerySupertype<T>, IRdfQuery<T>
	{
		public RdfN3Query()
		{
			originalType = typeof(T);
			parser = new LinqToN3ExpTranslator<T>();
		}

		private IQueryFormatTranslator parser;
		private Expression expression;

		public Store Store
		{
			get { return store; }
			set { store = value; }
		}

		private Store store;
		private List<T> result = null;

		public IQueryFormatTranslator Parser
		{
			get { return parser; }
			set { parser = value; }
		}

		public Expression Expression
		{
			get { return System.Expressions.Expression.Constant(this); }
		}

		public Type ElementType
		{
			get { return OriginalType; }
		}
		protected RdfN3Query<TElement> CloneQueryForNewType<TElement>()
		{
			RdfN3Query<TElement> newQuery = new RdfN3Query<TElement>();
			newQuery.Store = store;
			newQuery.OriginalType = originalType;
			newQuery.Projection = projection;
			newQuery.Properties = properties;
			newQuery.Query = Query;
			newQuery.Logger = logger;
			newQuery.QueryFactory = new QueryFactory<TElement>(QueryFactory.QueryType);
			newQuery.Parser = QueryFactory.CreateExpressionTranslator();
			newQuery.Parser.StringBuilder = new StringBuilder(parser.StringBuilder.ToString());
			return newQuery;
		}

		public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
		{
			RdfN3Query<TElement> newQuery = CloneQueryForNewType<TElement>();

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
		///Returns an enumerator that iterates through a collection.
		///</summary>
		///
		///<returns>
		///An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
		///</returns>
		///<filterpriority>2</filterpriority>
		public IEnumerator GetEnumerator()
		{
			string qry = ConstructQuery();
			PrepareQueryAndConnection();
			PresentQuery(qry);
			return (IEnumerator<T>)result;
		}

		public IQueryable CreateQuery(Expression expression)
		{
			return CreateQuery<T>(expression);
		}

		public object Execute(Expression expression)
		{
			return Execute<T>(expression);
		}

		///<summary>
		///Returns an enumerator that iterates through the collection.
		///</summary>
		///<returns>
		///A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
		///</returns>
		///<filterpriority>1</filterpriority>
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			if (result != null)
				return result.GetEnumerator();
			query = ConstructQuery();
			PrepareQueryAndConnection();
			PresentQuery(query);
			return result.GetEnumerator();
		}

		private void PresentQuery(string qry)
		{
			Store ms = store;
			ObjectDeserialiserQuerySink sink = new ObjectDeserialiserQuerySink(originalType, typeof(T));
			Query graphMatchQuery = new GraphMatch(new N3Reader(new StringReader(qry)));
			graphMatchQuery.Run(ms, sink);
			result = new List<T>();
			foreach (T t in sink.DeserialisedObjects)
			{
				result.Add(t);
			}
		}

		private void PrepareQueryAndConnection()
		{
			// create a ObjectDeserialiserQuerySink and attach it to the store
			string q = string.Format("@prefix m: <{0}> .\n", OwlInstanceSupertype.GetOntologyBaseUri(originalType));
			query = q + query;
			foreach (PropertyInfo pi in OwlClassSupertype.GetAllPersistentProperties(typeof(T)))
			{
				query += string.Format("?{0} <{1}> ?{2} .\n", originalType.Name, OwlInstanceSupertype.GetPropertyUri(originalType, pi.Name), pi.Name);
			}
		}

		string ConstructQuery()
		{
			return Parser.StringBuilder.ToString();
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

