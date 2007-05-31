using System.Collections.Generic;

namespace LinqToRdf
{
	public interface IRdfCommand<T>
	{
		string CommandText { get; set; }
		IEnumerator<T> ExecuteQuery();
	}
}