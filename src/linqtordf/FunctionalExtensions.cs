using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;

namespace LinqToRdf
{
    public static class FunctionalExtensions
    {
        public static IEnumerable<R> Map<T, R>(this IEnumerable<T> seq, Func<T, R> f)
        {
            foreach (T t in seq)
                yield return f(t);
        }
        public static Func<T, T> On<T>(this Func<T, T> inner, Func<T, T> outer)
        {
            return t => outer(inner(t));
        }
        public static void ForEach<T>(this IEnumerable<T> seq, Action<T> task)
        {
            foreach (T t in seq)
            {
                task(t);
            }
        }
        public static bool ContainsAnyOf<T>(this IEnumerable<T> seq, IEnumerable<T> x)
        {
            bool result = false;
            x.ForEach(a => result |= seq.Contains(a));
            return result;
        }
        public static bool OccursAsStmtObjectWithUri<T>(this EntitySet<T> set, string Uri) where T : class
        {
            return true;
        }
        public static bool OccursAsStmtObjectWithUri(this OwlInstanceSupertype set, string Uri)
        {
            return true;
        }
        public static bool HasInstanceUri(this OwlInstanceSupertype set, string Uri)
        {
            return true;
        }
        public static bool StmtObjectWithSubjectAndPredicate(this OwlInstanceSupertype set, string subjectUri, string predicateUri)
        {
            return true;
        }
        public static bool StmtSubjectWithObjectAndPredicate(this OwlInstanceSupertype set, string objectUri, string predicateUri)
        {
            return true;
        }
    }

}
