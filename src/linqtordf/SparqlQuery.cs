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
using System.Linq.Expressions;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LinqToRdf.Sparql
{
    public class SparqlQuery<T> : QuerySupertype<T>, IRdfQuery<T>
    {
        public SparqlQuery(IRdfContext context)
        {
            this.context = context;
            tripleStore = null;
            originalType = typeof(T);
            parser = new LinqToSparqlExpTranslator<T>();
        }

        private IQueryFormatTranslator parser;

        private TripleStore tripleStore;

        public IQueryFormatTranslator Parser
        {
            get { return parser; }
            set { parser = value; }
        }

        public TripleStore TripleStore
        {
            get { return tripleStore; }
            set { tripleStore = value; }
        }

        protected SparqlQuery<S> CloneQueryForNewType<S>()
        {
            SparqlQuery<S> newQuery = new SparqlQuery<S>(context);
            newQuery.TripleStore = tripleStore;
            newQuery.OriginalType = originalType;
            newQuery.Projection = projection;
            newQuery.QueryGraphParameters = queryGraphParameters;
            newQuery.FilterClause = FilterClause;
            newQuery.Logger = logger;
            newQuery.QueryFactory = new QueryFactory<S>(QueryFactory.QueryType, context);
            newQuery.Parser = QueryFactory.CreateExpressionTranslator();
            newQuery.Parser.StringBuilder = new StringBuilder(parser.StringBuilder.ToString());
            newQuery.Expressions = expressions;
            return newQuery;
        }

        protected void ParseQuery(Expression expression, StringBuilder sb)
        {
            Log("#Query {0:d}", DateTime.Now);
            StringBuilder tmp = Parser.StringBuilder;
            Parser.StringBuilder = sb;
            Parser.Dispatch(expression);
            Parser.StringBuilder = tmp;
        }

        protected IEnumerator<T> RunQuery()
        {
            if (CachedResults != null && ShouldReuseResultset)
                return CachedResults.GetEnumerator();
            if (QueryText == null)
            {
                StringBuilder sb = new StringBuilder();
                CreateSelectQuery(sb);
                QueryText = sb.ToString();
            }
            IRdfConnection<T> conn = QueryFactory.CreateConnection(this);
            IRdfCommand<T> cmd = conn.CreateCommand();
            cmd.CommandText = QueryText;
            return cmd.ExecuteQuery();
        }

        private void CreateSelectQuery(StringBuilder sb)
        {
            if (Expressions.ContainsKey("Where"))
            {
                // first parse the where expression to get the list of parameters to/from the query.
                StringBuilder sbTmp = new StringBuilder();
                UnaryExpression ue = Expressions["Where"].Arguments[1] as UnaryExpression;
                ParseQuery(ue.Operand, sbTmp);
                //sbTmp now contains the FILTER clause so save it somewhere useful.
                FilterClause = sbTmp.ToString();
                // now store the parameters where they can be used later on.
                if (Parser.Parameters != null)
                {
                    foreach (var item in Parser.Parameters)
                    {
                        queryGraphParameters.Add(item);
                    }
                }
                // we need to add the original type to the prolog to allow elements of the where clause to be optimised
            }
            CreateProlog(sb);
            CreateDataSetClause(sb);
            CreateProjection(sb);
            bool isIdentityProjection = OriginalType == typeof(T);
            bool shouldUseOptionalForAllProperties = !(Expressions.ContainsKey("Where")) || isIdentityProjection;
            if (shouldUseOptionalForAllProperties)
            {
                CreateOptionalWhereClause(sb);
            }
            else
            {
                CreateWhereClause(sb);
            }

            CreateSolutionModifier(sb);
        }

        private void CreateProlog(StringBuilder sb)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (OntologyAttribute ontology in assembly.GetAllOntologies())
                {
                    if (namespaceManager[ontology.Name] != null && namespaceManager[ontology.Name].BaseUri != ontology.BaseUri)
                    {
                        ontology.Prefix = namespaceManager.CreateNewPrefixFor(ontology);
                    }
                    namespaceManager[ontology.Prefix] = ontology;
                }
            }
            // now insert namespaces needed for the OwlClasses we're working with in this query
            foreach (var ont in namespaceManager.Ontologies)
            {
                sb.AppendFormat("PREFIX {0}: <{1}>\n", ont.Prefix, ont.BaseUri);
            }
            sb.Append("\n");
        }

        private void CreateDataSetClause(StringBuilder sb)
        {
            // related Issue: #12 http://code.google.com/p/linqtordf/issues/detail?id=12
            string graph = OriginalType.GetOntology().GraphName;
            if (!graph.Empty())
            {
                sb.AppendFormat("FROM NAMED <{0}>\n", graph);
            }
            return; // no named graphs just yet ()
        }

        private void CreateProjection(StringBuilder sb)
        {
            if (Expressions.ContainsKey("Select"))
                BuildProjection(Expressions["Select"]);

            if (projectionParameters.Count == 0)
            {
                sb.Append("SELECT ");
                foreach (MemberInfo mi in OwlClassSupertype.GetAllPersistentProperties(typeof(T)))
                {
                    sb.Append(" ?");
                    sb.Append(mi.Name);
                }
            }
            else
            {
                sb.Append("SELECT ");
                foreach (MemberInfo mi in projectionParameters)
                {
                    sb.Append(" ?");
                    sb.Append(mi.Name);
                }
            }
            sb.Append('\n');
        }

        private void CreateWhereClause(StringBuilder sb)
        {
            string instanceName = GetInstanceName();
            sb.Append("WHERE {\n");
            List<MemberInfo> parameters = new List<MemberInfo>(queryGraphParameters.Union(projectionParameters));

            if (parameters.Count == 0)
            {
                // is it an identity projection? If so, place all persistent properties into parameters
                if (OriginalType == typeof(T))
                {
                    foreach (PropertyInfo info in OwlClassSupertype.GetAllPersistentProperties(OriginalType))
                    {
                        parameters.Add(info);
                    }
                }
            }

            if (parameters.Count > 0)
            {
                sb.AppendFormat("$a a {0};\n", originalType.GetOntology().Prefix + ":" + originalType.GetOwlResource().RelativeUriReference);
            }

            for (int i = 0; i < parameters.Count; i++)
            {
                MemberInfo info = parameters[i];
                sb.AppendFormat("{0}{1} ?{2} ", originalType.GetOntology().Prefix + ":", info.GetOwlResource().RelativeUriReference, info.Name);
                sb.AppendFormat((i < parameters.Count - 1) ? ";\n" : ".\n");
            }
            if (FilterClause != null && FilterClause.Length > 0)
            {
                sb.AppendFormat("FILTER(\n{0}\n)\n", FilterClause);
            }
            sb.Append("}\n");
        }

        private void CreateOptionalWhereClause(StringBuilder sb)
        {
            string instanceName = GetInstanceName();
            sb.Append("WHERE {\n");
            List<MemberInfo> parameters = new List<MemberInfo>(queryGraphParameters.Union(projectionParameters));

            if (parameters.Count == 0)
            {
                // is it an identity projection? If so, place all persistent properties into parameters
                if (OriginalType == typeof(T))
                {
                    foreach (PropertyInfo info in OwlClassSupertype.GetAllPersistentProperties(OriginalType))
                    {
                        parameters.Add(info);
                    }
                }
            }

            if (parameters.Count > 0)
            {
                sb.AppendFormat("${0} ", instanceName);
                sb.AppendFormat(" a {0}.\n", originalType.GetOntology().Prefix + ":" + originalType.GetOwlResource().RelativeUriReference);
            }

            bool isIdentityProjection = OriginalType == typeof(T);
            if (isIdentityProjection)
            {
                foreach (MemberInfo mi in OwlClassSupertype.GetAllPersistentProperties(OriginalType))
                {
                    sb.AppendFormat("OPTIONAL {{_:{0} {1}{2} ?{3}. }}\n", instanceName, originalType.GetOntology().Prefix + ":", mi.GetOwlResource().RelativeUriReference, mi.Name);
                }
            }
            else
            {
                for (int i = 0; i < parameters.Count; i++)
                {
                    MemberInfo info = parameters[i];
                    sb.AppendFormat("OPTIONAL {{_:{0} {1}{2} ?{3}. }}\n", instanceName, originalType.GetOntology().Prefix + ":", info.GetOwlResource().RelativeUriReference, info.Name);
                }
            }
            if (FilterClause != null && FilterClause.Length > 0)
            {
                sb.AppendFormat("FILTER(\n{0}\n)\n", FilterClause);
            }
            sb.Append("}\n");
        }

        private string GetInstanceName()
        {
            if (Expressions.ContainsKey("Where"))
            {
                MethodCallExpression whereExp = Expressions["Where"];
                UnaryExpression ue = ((MethodCallExpression)whereExp).Arguments[1] as UnaryExpression;
                LambdaExpression le = (LambdaExpression)ue.Operand;
                ParameterExpression instance = le.Parameters[0];
                return instance.Name;
            }
            else
            {
                // no name supplied by LINQ so just give one at random.
                return "tmpInt";
            }
        }

        private void CreateSolutionModifier(StringBuilder sb)
        {
            CreateOrderClause(sb);
            CreateLimitClause(sb);
            CreateOffsetClause(sb);
        }

        private void CreateOrderClause(StringBuilder sb)
        {
            if (Expressions.ContainsKey("OrderBy"))
            {
                MethodCallExpression orderExp = Expressions["OrderBy"];
                UnaryExpression ue = (UnaryExpression)orderExp.Arguments[1];
                LambdaExpression descriminatingFunction = (LambdaExpression)ue.Operand;
                MemberExpression me = (MemberExpression)descriminatingFunction.Body;
                sb.AppendFormat("ORDER BY ?{0}\n", me.Member.Name);
            }
        }

        private void CreateLimitClause(StringBuilder sb)
        {
            if (Expressions.ContainsKey("Take"))
            {
                MethodCallExpression takeExpression = Expressions["Take"];
                ConstantExpression constantExpression = (ConstantExpression)takeExpression.Arguments[1];
                if (constantExpression.Value != null)
                {
                    sb.AppendFormat("LIMIT {0}\n", constantExpression.Value);
                }
            }
        }

        private void CreateOffsetClause(StringBuilder sb)
        {
            if (Expressions.ContainsKey("Skip"))
            {
                MethodCallExpression skipExpression = Expressions["Skip"];
                ConstantExpression constantExpression = (ConstantExpression)skipExpression.Arguments[1];
                if (constantExpression.Value != null)
                {
                    sb.AppendFormat("OFFSET {0}\n", constantExpression.Value);
                }
            }
        }

        public Type ElementType
        {
            get { return originalType; }
        }

        public Expression Expression
        {
            get { return System.Linq.Expressions.Expression.Constant(this); }
        }

        public IQueryable<S> CreateQuery<S>(Expression expression)
        {
            SparqlQuery<S> newQuery = CloneQueryForNewType<S>();

            MethodCallExpression call = expression as MethodCallExpression;
            if (call != null)
            {
                newQuery.Expressions[call.Method.Name] = call;
            }
            return newQuery;
        }

        public S Execute<S>(Expression expression)
        {
            //this.expression = expression;
            throw new NotImplementedException("Execute not implmented");
        }

        ///<summary>
        ///Returns an enumerator that iterates through the collection.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        ///</returns>
        ///<filterpriority>1</filterpriority>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return RunQuery();
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
            return this.CreateQuery<T>(expression);
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryProvider Provider
        {
            get { return this; }
        }
    }
}