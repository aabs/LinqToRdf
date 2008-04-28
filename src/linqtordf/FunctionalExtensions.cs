using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace linqtordf
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
    }

}
