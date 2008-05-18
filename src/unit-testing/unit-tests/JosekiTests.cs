using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqToRdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdfMusic;

namespace UnitTests
{
    [TestClass]
    public class JosekiTests
    {
        [TestMethod, Ignore]
        public void JosekiQueryWithProjection()
        {
            TripleStore ts = new TripleStore(@"http://localhost:2020/music");
			IRdfQuery<Track> qry = new RdfDataContext(ts).ForType<Track>();
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
