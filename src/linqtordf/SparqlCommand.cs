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
		protected Logger Logger = new Logger(typeof(SparqlCommand<T>));
		public IRdfConnection<T> Connection { get; set; }

		#region IRdfCommand<T> Members

		public string CommandText { get; set; }
		public string InstanceName { get; set; }
		public IEnumerator<T> ExecuteQuery()
		{
			using (var loggingScope = new LoggingScope("SparqlCommand<T>.ExecuteQuery()"))
			{
				#region Tracing
#line hidden
				if (Logger.IsDebugEnabled)
				{
					Logger.Debug("{0}.", CommandText);
				}
#line default
				#endregion
				MethodCallExpression e = null;
				IList<T> results = new List<T>();
				switch (Connection.Store.QueryType)
				{
					case QueryType.LocalSparqlStore:
						SparqlLocalConnection<T> localConnection = (SparqlLocalConnection<T>)Connection;
						if (localConnection.SparqlQuery.Expressions.ContainsKey("Select"))
						{
							e = localConnection.SparqlQuery.Expressions["Select"];
						}
						ObjectDeserialiserQuerySink sinkLocal =
							new ObjectDeserialiserQuerySink(localConnection.SparqlQuery.OriginalType, typeof(T), InstanceName, ElideDuplicates, e, (RdfDataContext)localConnection.SparqlQuery.DataContext);
						DateTime beforeQueryCompilation = DateTime.Now;
						Query query = new SparqlEngine(CommandText);
						DateTime afterQueryCompilation = DateTime.Now;
						DateTime beforeQueryRun = DateTime.Now;
						query.Run(localConnection.Store.LocalTripleStore, sinkLocal);
						DateTime afterQueryRun = DateTime.Now;
						ExtractResultsIntoList(results, sinkLocal);
						RegisterResults(localConnection.SparqlQuery, results);
						#region Tracing
#line hidden
						if (Logger.IsDebugEnabled)
						{
							Logger.Debug("Query: Parsing ({0}ms) Execution ({1}ms).",
								(afterQueryCompilation - beforeQueryCompilation).Milliseconds,
								(afterQueryRun - beforeQueryRun).Milliseconds);
						}
#line default
						#endregion
						break;

					case QueryType.RemoteSparqlStore:
						SparqlConnection<T> remoteConnection = (SparqlConnection<T>)Connection;
						if (remoteConnection.SparqlQuery.Expressions.ContainsKey("Select"))
						{
							e = remoteConnection.SparqlQuery.Expressions["Select"];
						}
						ObjectDeserialiserQuerySink sinkRemote =
							new ObjectDeserialiserQuerySink(remoteConnection.SparqlQuery.OriginalType, typeof(T), InstanceName, ElideDuplicates, e, (RdfDataContext)remoteConnection.SparqlQuery.DataContext);
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
			foreach (T t in results)
			{
				var i = t as OwlInstanceSupertype;
				if (i != null)
				{
					i.DataContext = query.DataContext;
				}
			}

			//discard any old results (not sure whether this will ever get invoked)
			if (query.CachedResults != null)
			{
				query.DataContext.ResultsCache.Remove(queryHashCode);
			}
			query.CachedResults = results;
		}

		private void ExtractResultsIntoList(IList<T> list, ObjectDeserialiserQuerySink querySink)
		{
			if (querySink.DeserialisedObjects != null)
			{
				foreach (T t in querySink.DeserialisedObjects)
				{
					list.Add(t);
				}
			}
		}
	}
}