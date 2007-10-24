using System;
using System.Reflection;
using LinqToRdf;
using SemWeb;
using RdfSerialisationTest.Properties;

namespace RdfSerialisationTest
{
	public class HighLevelTests
	{
		protected static MemoryStore store;

		protected static void CreateMemoryStore()
		{
			string mp3s = Settings.Default.testStoreLocation;
            string musicOntology = Settings.Default.testStoreLocation;
			store = new MemoryStore();
//			store.AddReasoner(new Euler(new N3Reader(MusicConstants.OntologyURL)));
			store.Import(new N3Reader(musicOntology));
			store.Import(new N3Reader(mp3s));
		}

		protected TripleStore CreateSparqlTripleStore()
		{
			CreateMemoryStore();
			TripleStore ts = new TripleStore();
			ts.LocalTripleStore = store;
			ts.QueryType = QueryType.LocalSparqlStore;
			return ts;
		}

		protected TripleStore CreateOnlineTripleStore()
		{
			TripleStore ts = new TripleStore();
            ts.EndpointUri = @"http://localhost/linqtordf/SparqlQuery.aspx";
			ts.QueryType = QueryType.RemoteSparqlStore;
			return ts;
		}

		protected MethodInfo propertyof(Type t, string arg)
		{
			return t.GetProperty(arg).GetGetMethod();
		}

		protected MethodInfo methodof(Type t, string arg)
		{
			return GetType().GetMethod(arg);
		}

	}
}