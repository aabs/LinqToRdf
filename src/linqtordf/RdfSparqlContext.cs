using System;
using SemWeb;

namespace LinqToRdf
{
    public class RdfSparqlContext : IRdfContext
    {
        private readonly string sparqlEndpoint;
        protected Store store;

        public RdfSparqlContext(string sparqlEndpoint)
        {
            this.sparqlEndpoint = sparqlEndpoint;
        }

        public Store Store
        {
            get { return store; }
            set { store = value; }
        }

        public RdfSparqlContext(Store store)
        {
            this.store = store;
        }

        public void AcceptChanges()
        {
            throw new NotImplementedException();
        }

		public IRdfQuery<T> ForType<T>()
		{
			QueryFactory<T> qf = new QueryFactory<T>(QueryType.RemoteSparqlStore);
			RdfSparqlQuery<T> result = (RdfSparqlQuery<T>) qf.CreateQuery<T>();
			result.SparqlEndpoint = sparqlEndpoint;
			result.QueryFactory = qf;
			return result;
		}
    }
}