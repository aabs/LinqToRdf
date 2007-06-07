/* 
 * Copyright (C) 2007, Andrew Matthews http://aabs.wordpress.com/
 *
 * This file is Free Software and part of LinqToRdf http://code.google.com/p/linqtordf/
 *
 * It is licensed under the following license:
 *   - Berkeley License, V2.0 or any newer version
 *
 * You may not use this file except in compliance with the above license.
 *
 * See http://code.google.com/p/linqtordf/ for the complete text of the license agreement.
 *
 */
using System;

namespace LinqToRdf.Sparql
{
	public class SparqlConnection<T> : IRdfConnection<T>
	{
		public SparqlQuery<T> SparqlQuery
		{
			get { return sparqlQuery; }
		}

		private readonly SparqlQuery<T> sparqlQuery;

		public TripleStore Store
		{
			get { return store; }
		}

		private readonly TripleStore store;

		public SparqlConnection(SparqlQuery<T> sparqlQuery)
		{
			this.sparqlQuery = sparqlQuery;
			store = sparqlQuery.TripleStore;
		}

		public IRdfCommand<T> CreateCommand()
		{
			SparqlCommand<T> result = new SparqlCommand<T>();
			result.Connection = this;
			return result;
		}
	}

	public class SparqlLocalConnection<T> : IRdfConnection<T>
	{
		public SparqlQuery<T> SparqlQuery
		{
			get { return sparqlQuery; }
		}

		private readonly SparqlQuery<T> sparqlQuery;

		public TripleStore Store
		{
			get { return store; }
		}

		private readonly TripleStore store;

		public SparqlLocalConnection(SparqlQuery<T> sparqlQuery)
		{
			this.sparqlQuery = sparqlQuery;
			store = sparqlQuery.TripleStore;
		}

		#region IDisposable Members

		///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		///<filterpriority>2</filterpriority>
		public void Dispose()
		{
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