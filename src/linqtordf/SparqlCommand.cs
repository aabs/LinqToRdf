using System.Collections.Generic;
using SemWeb.Remote;

namespace LinqToRdf.Sparql
{
	public class SparqlCommand<T> : IRdfCommand<T>
	{
		private string commandText;
		private IRdfConnection<T> connection;

		#region IRdfCommand Members

		public string CommandText
		{
			get { return commandText; }
			set { commandText = value; }
		}

		public IRdfConnection<T> Connection
		{
			get { return connection; }
			set { connection = value; }
		}

		#region IRdfCommand<T> Members

		public IEnumerator<T> ExecuteQuery()
		{
			SparqlConnection<T> conn = (SparqlConnection<T>)Connection;
			SparqlHttpSource source = new SparqlHttpSource(conn.Endpoint);
			ObjectDeserialiserQuerySink sink = new ObjectDeserialiserQuerySink(conn.SparqlQuery.OriginalType, typeof(T));
			source.RunSparqlQuery(CommandText, sink);
			return sink.DeserialisedObjects as IEnumerator<T>;
		}

		#endregion

		#endregion
	}
}