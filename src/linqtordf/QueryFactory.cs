using System.Text;

namespace LinqToRdf
{
	public class QueryFactory<T>
	{
		public QueryFactory(QueryType queryType)
		{
			this.queryType = queryType;
		}

		private readonly QueryType queryType;

		public IExpressionTranslator CreateExpressionTranslator()
		{
			switch (queryType)
			{
				case QueryType.RemoteSparqlStore:
					return new SparqlExpressionTranslator<T>(new StringBuilder());
				default:
					return new N3ExpressionTranslator<T>(new StringBuilder());
			}
		}

		public IRdfQuery<S> CreateQuery<S>()
		{
			switch (queryType)
			{
				case QueryType.RemoteSparqlStore:
					return new RdfSparqlQuery<S>();
				default:
					return new RdfN3Query<S>();
			}
		}

		public QueryType QueryType
		{
			get { return queryType; }
		}
	}
}