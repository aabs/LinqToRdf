using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using LinqToRdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdfMusic;
using System.Linq;

namespace RdfSerialisationTest 
{
	/// <summary>
	/// Summary description for JoinTests
	/// </summary>
	[TestClass]
	public class JoinTests : HighLevelTests
	{
		public JoinTests()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		[TestMethod]
		public void TestMethod1()
		{
			TripleStore ts = CreateSparqlTripleStore();
			IRdfQuery<Track> qry = new RDF(ts).ForType<Track>(); 
	        var q = from t in qry
				where t.Year == "2007" &&
				t.GenreName == "Rory Blyth: The Smartest Man in the World" 
				select new {t.Title, t.FileLocation};
			foreach(var track in q){
				Console.WriteLine(track.Title + ": " + track.FileLocation);
			}        
		}

	}
}
