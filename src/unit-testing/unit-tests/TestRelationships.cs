using System.Linq;
using NUnit.Framework;
using RdfMusic;

namespace UnitTests
{
    /// <summary>
    /// Summary description for TestRelationships
    /// </summary>
    [TestFixture]
    public class TestRelationships
    {

        [Test]
        public void TestGetAlbum()
        {
            var ctx = new MusicDataContext(@"http://localhost/linqtordf/SparqlQuery.aspx");
            var q1 = from a in ctx.Albums where a.Name == "Thomas Laqueur - History Lectures" select a;
            Album[] aa = q1.ToArray();
            Assert.IsTrue(aa.Count() == 1);
        }
  
        [Test]
        public void TestTrackToAlbum1()
        {
            var ctx = new MusicDataContext(@"http://localhost/linqtordf/SparqlQuery.aspx");
            IQueryable<Track> q = from t in ctx.Tracks
                                  where t.Year == "2006" &&
                                        t.GenreName == "History 5"
                                  select t;
            Track[] ta = q.ToArray();
            Assert.IsTrue(ta.Count() == 2);
            Assert.IsTrue(ta[0].AlbumName == "Thomas Laqueur - History Lectures");
            Assert.IsTrue(ta[0].Album != null);
        }
    }
}