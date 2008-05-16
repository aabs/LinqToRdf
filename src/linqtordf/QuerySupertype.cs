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
using System.IO;
using System.Linq.Expressions;
using System.Reflection;

namespace LinqToRdf
{
    public class QuerySupertype<T>
    {
        protected Dictionary<string, MethodCallExpression> expressions;
        protected string filterClause;
        protected TextWriter logger;
        protected NamespaceManager namespaceManager = new NamespaceManager();
        protected Type originalType = typeof (T);
        protected Delegate projection;
        protected HashSet<MemberInfo> projectionParameters = new HashSet<MemberInfo>();
        protected QueryFactory<T> queryFactory;
        protected HashSet<MemberInfo> queryGraphParameters = new HashSet<MemberInfo>();
        private bool shouldReuseResultset;
        public IRdfContext DataContext { get; set; }

        public IEnumerable<T> CachedResults
        {
            get
            {
                string hashcode = GetHashCode().ToString();
                if (DataContext.ResultsCache.ContainsKey(hashcode))
                {
                    return DataContext.ResultsCache[hashcode] as IEnumerable<T>;
                }
                return null;
            }
            set
            {
                string hashcode = GetHashCode().ToString();
                DataContext.ResultsCache[hashcode] = value;
            }
        }

        public Dictionary<string, MethodCallExpression> Expressions
        {
            get
            {
                if (expressions == null)
                    expressions = new Dictionary<string, MethodCallExpression>();
                return expressions;
            }
            set { expressions = value; }
        }

        public string FilterClause
        {
            get { return filterClause; }
            set { filterClause = value; }
        }

        public string PropertyReferenceTriple { get; set; }

        public TextWriter Logger
        {
            get { return logger; }
            set { logger = value; }
        }

        public Type OriginalType
        {
            get { return originalType; }
            set { originalType = value; }
        }

        public HashSet<MemberInfo> QueryGraphParameters
        {
            get { return queryGraphParameters; }
            set { queryGraphParameters = value; }
        }

        public HashSet<MemberInfo> ProjectionParameters
        {
            get { return projectionParameters; }
            set { projectionParameters = value; }
        }

        public Delegate Projection
        {
            get { return projection; }
            set { projection = value; }
        }

        public string QueryText { get; set; }

        public QueryFactory<T> QueryFactory
        {
            get { return queryFactory; }
            set { queryFactory = value; }
        }

        public bool ShouldReuseResultset
        {
            get { return shouldReuseResultset; }
            set
            {
                shouldReuseResultset = value;
                // if we stop caching & we already have results then erase them
                if (shouldReuseResultset == false && DataContext.ResultsCache != null)
                {
                    DataContext.ResultsCache.Clear();
                    DataContext.ResultsCache = null;
                }
            }
        }

        protected void BuildProjection(Expression expression)
        {
            var ue = ((MethodCallExpression) expression).Arguments[1] as UnaryExpression;
            var le = (LambdaExpression) ue.Operand;
            if (le == null)
                throw new ApplicationException("Incompatible expression type found when building ontology projection");
            projection = le.Compile();
            var mie = le.Body as NewExpression;
            if (le.Body is ParameterExpression) //  ie an identity projection
            {
                foreach (PropertyInfo i in OwlClassSupertype.GetAllPersistentProperties(originalType))
                    projectionParameters.Add(i);
            }
            else if (le.Body is MemberExpression)
            {
                var memex = le.Body as MemberExpression;
                projectionParameters.Add(memex.Member);
            }
            else
            {
                foreach (MemberExpression me in mie.Arguments)
                {
                    projectionParameters.Add(me.Member);
                }
            }
        }

        private void FindProperties(MemberBinding e)
        {
            if (!namespaceManager.HasOntologyFor(OriginalType))
                namespaceManager.Add(OriginalType);
            switch (e.BindingType)
            {
                case MemberBindingType.Assignment:
                    projectionParameters.Add(e.Member);
                    break;
                case MemberBindingType.ListBinding:
                    break;
                case MemberBindingType.MemberBinding:
                    projectionParameters.Add(e.Member);
                    break;
            }
        }

        internal void Log(string msg, params object[] args)
        {
            if (Logger != null)
                logger.WriteLine(string.Format("+ :" + msg, args));
        }
    }
}