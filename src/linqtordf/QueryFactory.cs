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
using System.Text;
using LinqToRdf.Sparql;

namespace LinqToRdf
{
	public class QueryFactory<T>
	{
		public QueryFactory(QueryType queryType, IRdfContext context)
		{
			this.queryType = queryType;
			this.context = context;
		}

		private readonly QueryType queryType;
		private readonly IRdfContext context;
		private ITypeTranslator typeConverter;

		public IQueryFormatTranslator CreateExpressionTranslator()
		{
			switch (queryType)
			{
				case QueryType.RemoteSparqlStore:
				case QueryType.LocalSparqlStore:
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
					return new SparqlQuery<S>(context);
				case QueryType.LocalSparqlStore:
					return new SparqlQuery<S>(context);
				default:
					return new RdfN3Query<S>(context);
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

		public IRdfConnection<T> CreateConnection(IRdfQuery<T> qry)
		{
			switch(queryType)
			{
				case QueryType.LocalSparqlStore:
					SparqlLocalConnection<T> sparqlLocalConnection = new SparqlLocalConnection<T>((SparqlQuery<T>)qry);
					return sparqlLocalConnection;
				case QueryType.RemoteSparqlStore:
					SparqlConnection<T> sparqlConnection = new SparqlConnection<T>((SparqlQuery<T>)qry);
					return sparqlConnection;
				default:
					throw new ApplicationException("Only sparql queries currently support the ADO.NET style APIs");
			}
		}
	}
}