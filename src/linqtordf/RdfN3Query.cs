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
using System.Expressions;
using System.IO;
using System.Query;
using System.Reflection;
using System.Text;
using SemWeb;
using SemWeb.Query;

namespace LinqToRdf
{
    public class RdfN3Query<T> : QuerySupertype<T>, IRdfQuery<T>
    {
        private Expression expression;
        private IQueryFormatTranslator parser;

        private Store store;

        public RdfN3Query(IRdfContext context)
        {
            this.context = context;
            originalType = typeof (T);
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

            MethodCallExpression call = expression as MethodCallExpression;
            if (call != null)
            {
                switch (call.Method.Name)
                {
                    case "Where":
                        Log("Processing the where expression");
                        newQuery.BuildQuery(call.Parameters[1]);
                        break;
                    case "Select":
                        Log("Processing the select expression");
                        newQuery.BuildProjection(call);
                        break;
                }
            }
            return newQuery;
        }

        public S Execute<S>(Expression expression)
        {
            this.expression = expression;
            throw new NotImplementedException("Execute not implmented");
        }

        ///<summary>
        ///Returns an enumerator that iterates through a collection.
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

        #endregion

        protected RdfN3Query<TElement> CloneQueryForNewType<TElement>()
        {
            RdfN3Query<TElement> newQuery = new RdfN3Query<TElement>(context);
            newQuery.Store = store;
            newQuery.OriginalType = originalType;
            newQuery.Projection = projection;
            newQuery.QueryGraphParameters = queryGraphParameters;
            newQuery.FilterClause = FilterClause;
            newQuery.Logger = logger;
            newQuery.QueryFactory = new QueryFactory<TElement>(QueryFactory.QueryType, context);
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
            filterClause = ConstructQuery();
            PrepareQueryAndConnection();
            PresentQuery(filterClause);
            return CachedResults.GetEnumerator();
        }

        private void PresentQuery(string qry)
        {
            Store ms = store;
            ObjectDeserialiserQuerySink sink = new ObjectDeserialiserQuerySink(originalType, typeof (T));
            Query graphMatchQuery = new GraphMatch(new N3Reader(new StringReader(qry)));
            graphMatchQuery.Run(ms, sink);
            List<T> list = new List<T>();
            foreach (T t in sink.DeserialisedObjects)
            {
                list.Add(t);
            }
            CachedResults = list;
        }

        private void PrepareQueryAndConnection()
        {
            // create a ObjectDeserialiserQuerySink and attach it to the store
            string q = string.Format("@prefix m: <{0}> .\n", OwlInstanceSupertype.GetOntologyBaseUri(originalType));
            filterClause = q + filterClause;
            foreach (PropertyInfo pi in OwlClassSupertype.GetAllPersistentProperties(typeof (T)))
            {
                filterClause +=
                    string.Format("?{0} <{1}> ?{2} .\n", originalType.Name,
                                  OwlInstanceSupertype.GetPropertyUri(originalType, pi.Name), pi.Name);
            }
        }

        private string ConstructQuery()
        {
            return Parser.StringBuilder.ToString();
        }

        private void BuildQuery(Expression q)
        {
            StringBuilder sb = new StringBuilder();
            ParseQuery(q, sb);
            FilterClause = Parser.StringBuilder.ToString();
            Log(FilterClause);
        }

        private void ParseQuery(Expression expression, StringBuilder sb)
        {
            sb.Append("#Query - " + DateTime.Now.ToLongTimeString());
            Parser.Dispatch(expression);
        }
    }
}