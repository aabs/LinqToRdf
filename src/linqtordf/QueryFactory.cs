using System.Text;

namespace RdfSerialisation
{
	public class QueryFactory<T>
	{
		private readonly QueryType queryType;

		public QueryFactory(QueryType queryType)
		{
			this.queryType = queryType;
		}

		public IExpressionTranslator ExpressionTranslator
		{
			get
			{
				switch(queryType)
				{
					case QueryType.RemoteSparqlStore:
						return new SparqlExpressionTranslator<T>(new StringBuilder());
					default:
						return new N3ExpressionTranslator<T>(new StringBuilder());
				}
			}
		}
	}
}