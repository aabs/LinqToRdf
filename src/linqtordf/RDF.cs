/* 
 * Copyright (C) 2007, Andrew Matthews http://aabs.wordpress.com/
 *
 * This file is Free Software and part of LinqToRdf http://code.google.com/fromName/linqtordf/
 *
 * It is licensed under the following license:
 *   - Berkeley License, V2.0 or any newer version
 *
 * You may not use this file except in compliance with the above license.
 *
 * See http://code.google.com/fromName/linqtordf/ for the complete text of the license agreement.
 *
 */
using System;
using System.Collections;
using System.Collections.Generic;
using LinqToRdf.Sparql;
using SemWeb;

namespace LinqToRdf
{
    /// <summary>
    /// A structure for storing the location and query idiom for a triple store
    /// </summary>
    /// <example>
    /// There a several special purpose ctors to allow you to easily construct an instance
    /// using typical values. the following example instantiates a local in-memory triple 
    /// store from locations stored in tasksontology and tasks. The query idiom will default to 
    /// LocalSparqlStore.
    /// <code language="csharp">
    /// MemoryStore store = new MemoryStore();
    /// store.AddReasoner(new Euler(new N3Reader(tasksOntology)));
    /// store.Import(new N3Reader(tasksOntology));
    /// store.Import(new N3Reader(tasks));
    /// TripleStore ts = new TripleStore(store);
    /// </code>
    /// </example>
    public class TripleStore
    {
        public TripleStore(Store localStore, string endpointUri, QueryType queryType)
        {
            QueryType = queryType;
            LocalTripleStore = localStore;
            EndpointUri = endpointUri;
        }
        public TripleStore(Store localStore, QueryType queryType) : this(localStore, null, queryType) { }
        public TripleStore(string endpointUri, QueryType queryType) : this(null, endpointUri, queryType) { }
        public TripleStore(Store localStore) : this(localStore, null, QueryType.LocalSparqlStore) { }
        public TripleStore(string endpointUri) : this(null, endpointUri, QueryType.RemoteSparqlStore) { }

        public QueryType QueryType { get; set; }
        public string EndpointUri { get; set; }
        public Store LocalTripleStore { get; set; }
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
            if (pendingQueue.Count == 0)
                return;
            if (store.QueryType != QueryType.LocalN3StoreInMemory)
                throw new NotImplementedException("No protocol exists to persist data to ontology remote store via SPARQL (yet). Unable to continue");
            MemoryStore ms = store.LocalTripleStore as MemoryStore;
            foreach (OwlInstanceSupertype inst in pendingQueue)
            {
                ms.Add(inst);
            }
        }

        public void DiscardChanges()
        {
            // cut loose the old queue (which should then be GCd)
            pendingQueue = new Queue<OwlInstanceSupertype>();
        }

        private Queue<OwlInstanceSupertype> pendingQueue = new Queue<OwlInstanceSupertype>();
        public void Add<T>(T entity) where T : OwlInstanceSupertype
        {
            if (entity == null)
                throw new ArgumentNullException("entity cannot be null");

            pendingQueue.Enqueue(entity);
        }

        public IRdfQuery<T> ForType<T>()
        {
            QueryFactory<T> qf = new QueryFactory<T>(Store.QueryType, this);
            switch (Store.QueryType)
            {
                case QueryType.LocalN3StoreInMemory:
                case QueryType.LocalN3StorePersistent:
                    RdfN3Query<T> tmp = (RdfN3Query<T>)qf.CreateQuery<T>();
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