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
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using SemWeb;
using SemWeb.Query;

namespace LinqToRdf
{
    public class RdfN3Query<T> : QuerySupertype<T>, IRdfQuery<T>
    {
        private IQueryFormatTranslator parser;

        private Store store;

        public RdfN3Query(IRdfContext context)
        {
            DataContext = context;
            OriginalType = typeof (T);
            parser = new LinqToN3ExpTranslator<T>();
        }

        public Store Store
        {
            get { return store; }
            set { store = value; }
        }

        public IQueryFormatTranslator Parser
        {
            get { return parser; }
            set { parser = value; }
        }

        #region IRdfQuery<T> Members

        public Expression Expression
        {
            get { return Expression.Constant(this); }
        }

        public Type ElementType
        {
            get { return OriginalType; }
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            RdfN3Query<TElement> newQuery = CloneQueryForNewType<TElement>();

            var call = expression as MethodCallExpression;
            if (call != null)
            {
                switch (call.Method.Name)
                {
                    case "Where":
                        Logger.Debug("Processing the where expression");
                        newQuery.BuildQuery(call.Arguments[1]);
                        break;
                    case "Select":
                        Logger.Debug("Processing the select expression");
                        newQuery.BuildProjection(call);
                        break;
                }
            }
            return newQuery;
        }

        public S Execute<S>(Expression expression)
        {
            throw new NotImplementedException("Execute not implmented");
        }

        ///<summary>
        ///Returns an enumerator that iterates through ontology collection.
        ///</summary>
        ///
        ///<returns>
        ///An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        public IEnumerator GetEnumerator()
        {
            return RunQuery();
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return CreateQuery<T>(expression);
        }

        public object Execute(Expression expression)
        {
            return Execute<T>(expression);
        }

        ///<summary>
        ///Returns an enumerator that iterates through the collection.
        ///</summary>
        ///<returns>
        ///A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        ///</returns>
        ///<filterpriority>1</filterpriority>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return RunQuery();
        }

        public IQueryProvider Provider
        {
            get { return this; }
        }

        #endregion

        protected RdfN3Query<TElement> CloneQueryForNewType<TElement>()
        {
            var newQuery = new RdfN3Query<TElement>(DataContext);
            newQuery.Store = store;
            newQuery.OriginalType = OriginalType;
            newQuery.ProjectionFunction = ProjectionFunction;
            newQuery.QueryGraphParameters = QueryGraphParameters;
            newQuery.FilterClause = FilterClause;
            newQuery.QueryFactory = new QueryFactory<TElement>(QueryFactory.QueryType, DataContext);
            newQuery.Parser = QueryFactory.CreateExpressionTranslator();
            newQuery.Parser.StringBuilder = new StringBuilder(parser.StringBuilder.ToString());
            return newQuery;
        }

        protected IEnumerator<T> RunQuery()
        {
            if (CachedResults != null && ShouldReuseResultset)
            {
                return CachedResults.GetEnumerator();
            }
            FilterClause = ConstructQuery();
            PrepareQueryAndConnection();
            PresentQuery(FilterClause);
            return CachedResults.GetEnumerator();
        }

        private void PresentQuery(string qry)
        {
            Store ms = store;
            var sink = new ObjectDeserialiserQuerySink(OriginalType, typeof (T), "", Expressions.ContainsKey("Distinct"),
                                                       Expressions["Select"], (RdfDataContext) DataContext);
            Query graphMatchQuery = new GraphMatch(new N3Reader(new StringReader(qry)));
            graphMatchQuery.Run(ms, sink);
            var list = new List<T>();
            foreach (T t in sink.IncomingResults)
            {
                list.Add(t);
            }
            CachedResults = list;
        }

        private void PrepareQueryAndConnection()
        {
            // create ontology ObjectDeserialiserQuerySink and attach it to the store
            string q = string.Format("@prefix m: <{0}> .\n", OwlClassSupertype.GetOntologyBaseUri(OriginalType));
            FilterClause = q + FilterClause;
            foreach (PropertyInfo pi in OwlClassSupertype.GetAllPersistentProperties(typeof (T)))
            {
                FilterClause +=
                    string.Format("?{0} <{1}> ?{2} .\n", OriginalType.Name,
                                  pi.GetOwlResourceUri(), pi.Name);
            }
        }

        private string ConstructQuery()
        {
            return Parser.StringBuilder.ToString();
        }

        private void BuildQuery(Expression q)
        {
            var sb = new StringBuilder();
            ParseQuery(q, sb);
            FilterClause = Parser.StringBuilder.ToString();
            Logger.Debug(FilterClause);
        }

        private void ParseQuery(Expression expression, StringBuilder sb)
        {
            Logger.Debug("#Query - {0}", DateTime.Now.ToLongTimeString());
            Parser.Dispatch(expression);
        }
    }
}