using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqToRdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdfMusic;

namespace RdfSerialisationTest
{
    [TestClass]
    public class JosekiTests
    {
        [TestMethod]
        public void JosekiQueryWithProjection()
        {
			TripleStore ts = CreateOnlineTripleStore();
			IRdfQuery<Track> qry = new RDF(ts).ForType<Track>();
			var q = from t in qry
							where t.Year == "2007" &&
							t.GenreName == "Rory Blyth: The Smartest Man in the World"
							select new { t.Title, t.FileLocation };
			foreach (var track in q)
			{
				Console.WriteLine(track.Title + ": " + track.FileLocation);
			}
        }

        protected TripleStore CreateOnlineTripleStore()
        {
            TripleStore ts = new TripleStore();
            ts.EndpointUri = @"http://localhost:2020/music";
            ts.QueryType = QueryType.RemoteSparqlStore;
            return ts;
        }

    }
}
