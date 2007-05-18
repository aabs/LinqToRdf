using System;
using System.Collections;
using System.Collections.Generic;
using System.Expressions;
using System.Query;
using SemWeb;

namespace RdfSerialisation
{
    public class RdfSparqlQuery<T> : IRdfQuery<T>
    {
        private readonly string sparqlEndpoint;
        private Expression expression;
        private Type elementType = typeof(T);

        public RdfSparqlQuery(Store store)
        {
        }
        public RdfSparqlQuery(string sparqlEndpoint)
        {
            this.sparqlEndpoint = sparqlEndpoint;
        }

        #region IQueryable<T> Members

        public IQueryable<S> CreateQuery<S>(Expression expression)
        {
            this.expression = expression;
            throw new NotImplementedException("CreateQuery not implmented");
        }

        public S Execute<S>(Expression expression)
        {
            this.expression = expression;
            throw new NotImplementedException("Execute not implmented");
        }

        #endregion

        #region IEnumerable<T> Members

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
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable Members

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
            throw new NotImplementedException();
        }

        #endregion

        #region IQueryable Members

        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        public Expression Expression
        {
            get { return expression; }
        }

        public Type ElementType
        {
            get { return elementType; }
        }

        #endregion
    }
}