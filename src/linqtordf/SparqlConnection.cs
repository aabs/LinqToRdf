using System;
using System.Collections.Generic;
using LinqToRdf;

namespace LinqToRdf.Sparql
{
	public class SparqlConnection<T> : IRdfConnection<T>
	{
		public SparqlQuery<T> SparqlQuery
		{
			get { return sparqlQuery; }
		}

		private readonly SparqlQuery<T> sparqlQuery;

		public string Endpoint
		{
			get { return endpoint; }
		}

		private readonly string endpoint;

		public SparqlConnection(SparqlQuery<T> sparqlQuery)
		{
			this.sparqlQuery = sparqlQuery;
			endpoint = sparqlQuery.SparqlEndpoint;
		}

		#region IDisposable Members

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		///<filterpriority>2</filterpriority>
		public void Dispose()
		{
			throw new NotImplementedException();
		}

		#endregion

		public IRdfCommand<T> CreateCommand()
		{
			SparqlCommand<T> result = new SparqlCommand<T>();
			result.Connection = this;
			return result;
		}
	}
}