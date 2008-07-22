using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdfMusic;

namespace UnitTests
{
    [TestClass]
    public class TestLetClause
    {
        [TestMethod]
        public void TestSimpleTestWithLet()
        {
            using (var ctx = new MusicDataContext("http://localhost/linqtordf/SparqlQuery.aspx"))
            {
                var q = from a in ctx.Albums
                        let c = a.Tracks.Count()
                        where c > 5
                        select a;
                var count = q.Count();
                Assert.IsTrue(count > 0);
 
            }
        }
    }
}
