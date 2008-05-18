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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace LinqToRdf.Sparql
{
    public class SparqlQuery<T> : QuerySupertype<T>, IRdfQuery<T>
    {
        private IQueryFormatTranslator parser;

        private TripleStore tripleStore;

        public SparqlQuery(IRdfContext context)
        {
            DataContext = context;
            tripleStore = null;
            originalType = typeof (T);
            parser = new LinqToSparqlExpTranslator<T>();
        }

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

        #region IRdfQuery<T> Members

        public Type ElementType
        {
            get { return originalType; }
        }

        public Expression Expression
        {
            get { return Expression.Constant(this); }
        }

        public IQueryable<S> CreateQuery<S>(Expression expression)
        {
            Expression exp;
            if (!ExpressionWillCauseInfiniteLoop(expression))
                exp = Evaluator.PartialEval(expression);
            else
                exp = expression;
            SparqlQuery<S> newQuery = CloneQueryForNewType<S>();

            var call = exp as MethodCallExpression;
            if (call != null)
            {
                newQuery.Expressions[call.Method.Name] = call;
            }
            return newQuery;
        }

        private bool ExpressionWillCauseInfiniteLoop(Expression e)
        {
            MethodCallExpression mce = e as MethodCallExpression;
            if(mce == null) return false;
            if (mce.Method.Name == "Skip")
                return true;
            if (mce.Method.Name == "Take")
                return true;
            return false;
        }

        public S Execute<S>(Expression expression)
        {
            var e = this.AsEnumerable();
            MethodCallExpression mce = expression as MethodCallExpression;
            object x = null;
            if (mce != null)
            {
                switch(mce.Method.Name)
                {
                    case "Skip":
                        return (S) e.Skip<T>(1);
                        break;
                    case "Count":
                        x = e.Count();
                        break;
                    case "First":
                        x = e.First();
                        break;
                    case "FirstOrDefault":
                        x = e.FirstOrDefault();
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            return (S)x;
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
            return CreateQuery<T>(expression);
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryProvider Provider
        {
            get { return this; }
        }

        #endregion

        protected SparqlQuery<S> CloneQueryForNewType<S>()
        {
            var newQuery = new SparqlQuery<S>(DataContext);
            newQuery.TripleStore = tripleStore;
            newQuery.OriginalType = originalType;
            newQuery.Projection = projection;
            newQuery.QueryGraphParameters = queryGraphParameters;
            newQuery.FilterClause = FilterClause;
            newQuery.Logger = logger;
            newQuery.QueryFactory = new QueryFactory<S>(QueryFactory.QueryType, DataContext);
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
                var sb = new StringBuilder();
                CreateSelectQuery(sb);
                QueryText = sb.ToString();
            }
            IRdfConnection<T> conn = QueryFactory.CreateConnection(this);
            IRdfCommand<T> cmd = conn.CreateCommand();
            cmd.ElideDuplicates = Expressions.ContainsKey("Distinct");
            cmd.CommandText = QueryText;
            cmd.InstanceName = GetInstanceName();
            return cmd.ExecuteQuery();
        }

        private void CreateSelectQuery(StringBuilder sb)
        {
            if (Expressions.ContainsKey("Where"))
            {
                // first parse the where expression to get the list of parameters to/from the query.
                var sbTmp = new StringBuilder();
                var ue = Expressions["Where"].Arguments[1] as UnaryExpression;
                ParseQuery(ue.Operand, sbTmp);

                if (ExpressionIsObjectPropertyReference(ue.Operand))
                {
                    PropertyReferenceTriple = sbTmp.ToString();
                }
                else
                {
                    //sbTmp now contains the FILTER clause so save it somewhere useful.
                    FilterClause = sbTmp.ToString();
                }
                // now store the parameters where they can be used later on.
                if (Parser.Parameters != null)
                {
                    foreach (MemberInfo item in Parser.Parameters)
                    {
                        queryGraphParameters.Add(item);
                    }
                }
                // we need to add the original type to the prolog to allow elements of the where clause to be optimised
            }
            CreateProlog(sb);
            CreateDataSetClause(sb);
            CreateProjection(sb);
            CreateWhereClause(sb);

            CreateSolutionModifier(sb);
        }

        private bool ExpressionIsObjectPropertyReference(Expression e)
        {
            if (e is LambdaExpression)
            {
                LambdaExpression le = (LambdaExpression) e;
                if (le.Body is MethodCallExpression)
                {
                    MethodCallExpression mce = (MethodCallExpression) le.Body;
                    if (mce.Method.Name == "HavingSubjectUri")
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void CreateProlog(StringBuilder sb)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (OntologyAttribute ontology in assembly.GetAllOntologies())
                {
                    if (namespaceManager[ontology.Name] != null &&
                        namespaceManager[ontology.Name].BaseUri != ontology.BaseUri)
                    {
                        ontology.Prefix = namespaceManager.CreateNewPrefixFor(ontology);
                    }
                    namespaceManager[ontology.Prefix] = ontology;
                }
            }
            // now insert namespaces needed for the OwlClasses we're working with in this query
            foreach (OntologyAttribute ont in namespaceManager.Ontologies)
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
            string distinct = Expressions.ContainsKey("Distinct") ? "DISTINCT " : "";

            if (Expressions.ContainsKey("Select"))
            {
                BuildProjection(Expressions["Select"]);
            }

            sb.Append("SELECT " + distinct);
            if (projectionParameters.Count == 0)
            {
                foreach (MemberInfo mi in OwlClassSupertype.GetAllPersistentProperties(typeof (T)))
                {
                    sb.Append(" ?");
                    sb.Append(mi.Name);
                }
            }
            else
            {
                foreach (MemberInfo mi in projectionParameters)
                {
                    sb.Append(" ?");
                    sb.Append(mi.Name);
                }
            }

            sb.AppendLine(" $" + GetInstanceName());
        }

        private void CreateWhereClause(StringBuilder sb)
        {
            // if using an identity projection then every available property of the type must be returned
            bool isIdentityProjection = OriginalType == typeof (T);
            // if there is no where clause then we want every instance back (and we won't be filtering)
            // the logic around this is a little tricky (or debatable) - 
            // Given that you could have instances that are partially complete in the triple store (i.e. not all triples are present)
            // you need to be able to ensure that a query that does not explicitly include the missing properties does not
            // exclude any instances where those properties are missing.
            // I've reasoned that if you perform an identity projection, then you're saying "get me whatever you can", whereas if 
            // you specifically request a certain property (via a projection) then you really must want a value for that, and thus
            // instances must be excluded where there is no value _known_ - Hence the '|| IsIdentityProjection'.
            bool getAnythingThatYouCan = !(Expressions.ContainsKey("Where")) || isIdentityProjection /* */;
            // using "$" distinguishes this varName from anything that could be introduced from the properties of the type
            // therefore the varName is 'safe' in the sense that there can never be a name clash.
            string varName = "$" + GetInstanceName();

            sb.Append("WHERE {\n");
            // if parameters have been defined somewhere. If using an identity projection then we will not be getting anything from projectionParameters
            // if we have no WHERE expression, then we also won't be getting anything from queryGraphParameters
            var parameters = new List<MemberInfo>(queryGraphParameters.Union(projectionParameters));

            if (parameters.Count == 0)
            {
                // is it an identity projection? If so, place all persistent properties into parameters
                if (isIdentityProjection)
                {
                    foreach (PropertyInfo info in OwlClassSupertype.GetAllPersistentProperties(OriginalType))
                    {
                        parameters.Add(info);
                    }
                }
            }

            if (parameters.Count > 0)
            {
                sb.AppendFormat("{0} a {1}:{2}.\n", varName, originalType.GetOntology().Prefix,
                                originalType.GetOwlResource().RelativeUriReference);
            }
            else
            {
                // I don't think there is any way to get into to this point unless the object is persistent, but has no 
                throw new ApplicationException(
                    "No persistent properties defined on the entity. Unable to generate a query.");
            }

            // temp var to get the object variables list
            IEnumerable<MemberInfo> args;
            // a temp string to get the tripleFormat that will be used to generate query triples.
            string tripleFormat = "OPTIONAL{{{0} {1}:{2} ?{3}.}}\n";

            if (!getAnythingThatYouCan)
                tripleFormat = "{0} {1}:{2} ?{3} .\n";

            if (isIdentityProjection)
                args = OwlClassSupertype.GetAllPersistentProperties(OriginalType);
            else
                args = parameters;

            foreach (MemberInfo arg in args)
            {
                sb.AppendFormat(tripleFormat,
                                varName,
                                originalType.GetOntology().Prefix,
                                arg.GetOwlResource().RelativeUriReference,
                                arg.Name);
            }

            if (!string.IsNullOrEmpty(PropertyReferenceTriple))
            {
                sb.AppendLine(PropertyReferenceTriple);
            }

            if (!string.IsNullOrEmpty(FilterClause))
            {
                sb.AppendFormat("FILTER( {0} )\n", FilterClause);
            }

            sb.Append("}\n");
        }

        private string GetInstanceName()
        {
            if (Expressions.ContainsKey("Where"))
            {
                MethodCallExpression whereExp = Expressions["Where"];
                var ue = (whereExp).Arguments[1] as UnaryExpression;
                var le = (LambdaExpression) ue.Operand;
                ParameterExpression instance = le.Parameters[0];
                return instance.Name;
            }
            // no name supplied by LINQ so just give one at random.
            return "tmpInt";
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
                var ue = (UnaryExpression) orderExp.Arguments[1];
                var descriminatingFunction = (LambdaExpression) ue.Operand;
                var me = (MemberExpression) descriminatingFunction.Body;
                sb.AppendFormat("ORDER BY ?{0}\n", me.Member.Name);
            }
        }

        private void CreateLimitClause(StringBuilder sb)
        {
            if (Expressions.ContainsKey("Take"))
            {
                MethodCallExpression takeExpression = Expressions["Take"];
                var constantExpression = (ConstantExpression) takeExpression.Arguments[1];
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
                var constantExpression = (ConstantExpression) skipExpression.Arguments[1];
                if (constantExpression.Value != null)
                {
                    sb.AppendFormat("OFFSET {0}\n", constantExpression.Value);
                }
            }
        }
    }
}