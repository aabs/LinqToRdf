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
using System.Collections;
using System.Collections.Generic;
using LinqToRdf.Sparql;
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
		public RDF(TripleStore store)
		{
			this.store = store;
		}

		private Dictionary<string, IEnumerable> resultsCache = new Dictionary<string, IEnumerable>();
		protected TripleStore store;

		public Dictionary<string, IEnumerable> ResultsCache
		{
			get { return resultsCache; }
			set { resultsCache = value; }
		}

		public TripleStore Store
		{
			get { return store; }
			set { store = value; }
		}

		#region IRdfContext Members

		#endregion

		public void AcceptChanges()
		{
			throw new NotImplementedException();
		}

		public IRdfQuery<T> ForType<T>()
		{
			QueryFactory<T> qf = new QueryFactory<T>(Store.QueryType, this);
			switch (Store.QueryType)
			{
				case QueryType.LocalN3StoreInMemory:
				case QueryType.LocalN3StorePersistent:
					RdfN3Query<T> tmp = (RdfN3Query<T>) qf.CreateQuery<T>();
					tmp.Store = Store.LocalTripleStore;
					tmp.QueryFactory = qf;
					return tmp;
				case QueryType.RemoteSparqlStore:
					SparqlQuery<T> tmp2 = (SparqlQuery<T>)qf.CreateQuery<T>();
					tmp2.TripleStore = Store;
					tmp2.QueryFactory = qf;
					return tmp2;
				case QueryType.LocalSparqlStore:
					SparqlQuery<T> tmp3 = (SparqlQuery<T>)qf.CreateQuery<T>();
					tmp3.TripleStore = Store;
					tmp3.QueryFactory = qf;
					return tmp3;
				default:
					throw new ApplicationException("unrecognised query type requested");
			}
		}
	}
}