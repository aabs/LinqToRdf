using System.Collections;
using System.Text;
using System.Collections.Generic;
using LinqToRdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdfMusic;
using System.Linq;

namespace RdfSerialisationTest
{
	[TestClass]
	public class StringOperationTests : HighLevelTests
	{
		[TestMethod]
		public void TestCompare()
		{
			TripleStore ts = CreateSparqlTripleStore();
			IRdfQuery<Album> qry = new RDF(ts).ForType<Album>();
			var q = from a in qry where a.Name.Contains("Thomas") select a;
			List<Album> al = new List<Album>(q);
			Assert.IsTrue(al.Count == 1);
			Assert.IsTrue(al[0].Name == "Thomas Laqueur - History Lectures");

		}
		[TestMethod]
		public void TestStartsWith()
		{
			TripleStore ts = CreateSparqlTripleStore();
			IRdfQuery<Album> qry = new RDF(ts).ForType<Album>();
			var q = from a in qry where a.Name.StartsWith("Thomas") select a;
			List<Album> al = new List<Album>(q);
			Assert.IsTrue(al.Count == 1);
			Assert.IsTrue(al[0].Name == "Thomas Laqueur - History Lectures");
		}
		[TestMethod]
		public void TestEndsWith()
		{
			TripleStore ts = CreateSparqlTripleStore();
			IRdfQuery<Album> qry = new RDF(ts).ForType<Album>();
			var q = from a in qry where a.Name.EndsWith("podcasts") select a;
			List<Album> al = new List<Album>(q);
			Assert.IsTrue(al.Count == 1);
			Assert.IsTrue(al[0].Name == "Rory Blyth - podcasts");
		}
	}
}