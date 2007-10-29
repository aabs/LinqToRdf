using NUnit.Framework;
using SystemScanner.Core;
using System.Collections.Generic;
using System.Reflection;
using SemWeb;
using SemWeb.Util;
using LinqToRdf;
using System.IO;

namespace SystemScanner.UnitTests
{
    public static class Extensions
    {
        public static Statement GetIdentityStatement<T>(this T super) where T : OwlInstanceSupertype
        {
            return new Statement((Entity)super.InstanceUri, OntologyHelper.rdfType, (Entity)OwlClassSupertype.GetOwlClassUri(typeof(T)));
        }

        public static System.Reflection.Assembly GetAssembly(this AssemblyName name)
        {
            return System.Reflection.Assembly.Load(name);
        }

        public static void Scan(AssemblyName name, Dictionary<string, AssemblyName> store)
        {
            if (store.ContainsKey(name.FullName)) return;
            store.Add(name.FullName, name);
            System.Reflection.Assembly thisAssembly = name.GetAssembly();
            foreach (System.Reflection.AssemblyName subname in thisAssembly.GetReferencedAssemblies())
            {
                Scan(subname, store);
            }
        }
    }
	[TestFixture]
	public class Test1
	{
        [SetUp]
        public void SetUp()
        {
			string loc = @"C:\etc\dev\prototypes\linqtordf\SystemScanner\rdf\sys.artifacts.n3";
            File.Delete(loc);
            FileStream fs = File.Create(loc);
            fs.Close();
        }

		[Test]
		public void TestAddingArtifactToStore()
		{
			string loc = @"C:\etc\dev\prototypes\linqtordf\SystemScanner\rdf\sys.artifacts.n3";
            ArtifactStore store = new ArtifactStore(loc);
            store.Add(new SystemScannerModel.Assembly(GetType().Assembly));
            Assert.AreEqual(8, store.TripleStore.StatementCount);
            //store.Save(loc);
		}
		[Test]
		public void TestScanArtifacts()
		{
			string loc = @"C:\etc\dev\prototypes\linqtordf\SystemScanner\rdf\sys.artifacts.n3";
            ArtifactStore store = new ArtifactStore(loc);
            Dictionary<string, AssemblyName> tmpStore = new Dictionary<string, AssemblyName>();
            Extensions.Scan(GetType().Assembly.GetName(), tmpStore);
            foreach (AssemblyName asmName in tmpStore.Values)
            {
                store.Add(new SystemScannerModel.Assembly(asmName.GetAssembly()));
            }
            Assert.AreEqual(344, store.TripleStore.StatementCount);
		}
	}
}

