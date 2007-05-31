using System;
using LinqToRdf.Sparql;

namespace LinqToRdf
{
	public interface IRdfConnection<T> : IDisposable
	{
		IRdfCommand<T> CreateCommand();
	}
}