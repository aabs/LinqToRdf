using System;
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
		private ITypeTranslator typeConverter;

		public IQueryFormatTranslator CreateExpressionTranslator()
		{
			switch (queryType)
			{
				case QueryType.RemoteSparqlStore:
					LinqToSparqlExpTranslator<T> translator = new LinqToSparqlExpTranslator<T>(new StringBuilder());
					translator.TypeTranslator = TypeTranslator;
					return translator;
				default:
					LinqToN3ExpTranslator<T> n3translator = new LinqToN3ExpTranslator<T>(new StringBuilder());
					n3translator.TypeTranslator = TypeTranslator;
					return n3translator;
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

		public ITypeTranslator TypeTranslator
		{
			get
			{
				if (typeConverter == null)
					typeConverter = new XsdtTypeConverter();
				return typeConverter;
			}
		}
	}
}