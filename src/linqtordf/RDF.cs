using System;
using SemWeb;

namespace LinqToRdf
{
	public class TripleStore
	{
		private QueryType queryType;
		private string endpointUri;
		private Store localTripleStore;

		public QueryType QueryType
		{
			get { return queryType; }
			set { queryType = value; }
		}

		public string EndpointUri
		{
			get { return endpointUri; }
			set { endpointUri = value; }
		}

		public Store LocalTripleStore
		{
			get { return localTripleStore; }
			set { localTripleStore = value; }
		}
	}
	public class RDF : IRdfContext
	{
		public TripleStore Store
		{
			get { return store; }
			set { store = value; }
		}

		protected TripleStore store;

		public RDF(TripleStore store)
		{
			this.store = store;
		}

		public void AcceptChanges()
		{
			throw new NotImplementedException();
		}

		public IRdfQuery<T> ForType<T>()
		{
			QueryFactory<T> qf = new QueryFactory<T>(Store.QueryType);
			switch (Store.QueryType)
			{
				case QueryType.LocalN3StoreInMemory:
				case QueryType.LocalN3StorePersistent:
					RdfN3Query<T> tmp = (RdfN3Query<T>) qf.CreateQuery<T>();
					tmp.Store = Store.LocalTripleStore;
					tmp.QueryFactory = qf;
					return tmp;
				case QueryType.RemoteSparqlStore:
					RdfSparqlQuery<T> tmp2 = (RdfSparqlQuery<T>) qf.CreateQuery<T>();
					tmp2.SparqlEndpoint = Store.EndpointUri;
					tmp2.QueryFactory = qf;
					return tmp2;
				default:
					throw new ApplicationException("unrecognised query type requested");
			}
		}
	}
}