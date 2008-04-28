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
using System.Collections.Generic;
using System.Linq.Expressions;
using SemWeb.Query;
using SemWeb.Remote;

namespace LinqToRdf.Sparql
{
    public class SparqlCommand<T> : IRdfCommand<T>
    {
        private string commandText;
        private IRdfConnection<T> connection;

        public IRdfConnection<T> Connection
        {
            get { return connection; }
            set { connection = value; }
        }

        #region IRdfCommand<T> Members

        public string CommandText
        {
            get { return commandText; }
            set { commandText = value; }
        }

        public IEnumerator<T> ExecuteQuery()
        {
            MethodCallExpression e = null;
            IList<T> results = new List<T>();
            switch (Connection.Store.QueryType)
            {
                case QueryType.LocalSparqlStore:
                    SparqlLocalConnection<T> localConnection = (SparqlLocalConnection<T>) Connection;
                    if (localConnection.SparqlQuery.Expressions.ContainsKey("Select"))
                    {
                        e = localConnection.SparqlQuery.Expressions["Select"];
                    }
                    ObjectDeserialiserQuerySink sinkLocal =
                        new ObjectDeserialiserQuerySink(localConnection.SparqlQuery.OriginalType, typeof (T), ElideDuplicates, e);
                    DateTime beforeQueryCompilation = DateTime.Now;
                    Query query = new SparqlEngine(CommandText);
                    DateTime afterQueryCompilation = DateTime.Now;
                    DateTime beforeQueryRun = DateTime.Now;
                    query.Run(localConnection.Store.LocalTripleStore, sinkLocal);
                    DateTime afterQueryRun = DateTime.Now;
                    ExtractResultsIntoList(results, sinkLocal);
                    RegisterResults(localConnection.SparqlQuery, results);
                    localConnection.SparqlQuery.Log(
                        string.Format("+ :Query: Parsing ({0}ms) Execution ({1}ms).",
                                      (afterQueryCompilation - beforeQueryCompilation).Milliseconds,
                                      (afterQueryRun - beforeQueryRun).Milliseconds));
                    break;

                case QueryType.RemoteSparqlStore:
                    SparqlConnection<T> remoteConnection = (SparqlConnection<T>) Connection;
                    if (remoteConnection.SparqlQuery.Expressions.ContainsKey("Select"))
                    {
                        e = remoteConnection.SparqlQuery.Expressions["Select"];
                    }
                    ObjectDeserialiserQuerySink sinkRemote =
                        new ObjectDeserialiserQuerySink(remoteConnection.SparqlQuery.OriginalType, typeof(T), ElideDuplicates, e);
                    SparqlHttpSource source = new SparqlHttpSource(remoteConnection.Store.EndpointUri);
                    source.RunSparqlQuery(CommandText, sinkRemote);
                    ExtractResultsIntoList(results, sinkRemote);
                    RegisterResults(remoteConnection.SparqlQuery, results);
                    break;

                default:
                    break;
            }
            return results.GetEnumerator();
        }

        public bool ElideDuplicates
        {
            get;
            set;
        }

        #endregion

        private void RegisterResults(SparqlQuery<T> query, IEnumerable<T> results)
        {
            string queryHashCode = query.GetHashCode().ToString();

            //discard any old results (not sure whether this will ever get invoked)
            if (query.CachedResults != null)
            {
                query.Context.ResultsCache.Remove(queryHashCode);
            }
            query.CachedResults = results;
        }

        private static void ExtractResultsIntoList(IList<T> list, ObjectDeserialiserQuerySink querySink)
        {
            if (querySink.DeserialisedObjects != null)
            {
                foreach (T deserialisedObject in querySink.DeserialisedObjects)
                {
                    list.Add(deserialisedObject);
                }
            }
        }
    }
}