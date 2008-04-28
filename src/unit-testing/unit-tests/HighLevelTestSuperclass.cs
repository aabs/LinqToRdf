using System;
using System.Reflection;
using LinqToRdf;
using SemWeb;
using UnitTests.Properties;

namespace UnitTests
{
	public class HighLevelTests
	{
        protected static MemoryStore CreateMemoryStore()
		{
			string mp3s = Settings.Default.testStoreLocation;
            string musicOntology = Settings.Default.testStoreLocation;
            MemoryStore store = new MemoryStore();
//			store.AddReasoner(new Euler(new N3Reader(MusicConstants.OntologyURL)));
			store.Import(new N3Reader(musicOntology));
			store.Import(new N3Reader(mp3s));
            return store;
		}

		protected TripleStore CreateSparqlTripleStore()
		{
            return new TripleStore(CreateMemoryStore());
		}

		protected TripleStore CreateOnlineTripleStore()
		{
			TripleStore ts = new TripleStore(@"http://localhost/linqtordf/SparqlQuery.aspx");
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