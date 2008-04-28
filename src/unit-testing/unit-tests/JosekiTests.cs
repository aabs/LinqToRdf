using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqToRdf;
using NUnit.Framework;
using RdfMusic;

namespace UnitTests
{
    [TestFixture]
    public class JosekiTests
    {
        [Test]
        public void JosekiQueryWithProjection()
        {
            TripleStore ts = new TripleStore(@"http://localhost:2020/music");
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
    }
}
