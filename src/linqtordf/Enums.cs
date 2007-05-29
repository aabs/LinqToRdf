namespace LinqToRdf
{
	/// <summary>
	/// indictates what types of store can be used, and how they can be interacted with.
	/// </summary>
	public enum QueryType : int
	{
		LocalN3StoreInMemory,	// uses SemWeb to talk to a local in-memory store
		LocalN3StorePersistent,	// uses SemWeb to talk to a local RDBMS-based store (uses a suplementary connection string to identify type)
		RemoteSparqlStore		// uses SPARQL to communicate via HTTP to a remote (or local) triple store (plpatform agnostic)
	}
}