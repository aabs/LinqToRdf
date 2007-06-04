using System.Query;

namespace LinqToRdf
{
    public interface IRdfQuery<T> : IOrderedQueryable<T>
    {
    }
}