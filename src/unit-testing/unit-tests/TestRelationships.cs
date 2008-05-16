using System;
using System.Linq;
using LinqToRdf;
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
        public void TestAlbumToTracks()
        {
            var ctx = new MusicDataContext(@"http://localhost/linqtordf/SparqlQuery.aspx");
            Album album = (from a in ctx.Albums
                           where a.Name.StartsWith("Thomas")
                           select a).ToList().First();
            Assert.IsNotNull(album);
            Assert.IsTrue(album.Tracks.ToList().Count() > 1);
        }


        [Test]
        public void TestDirectReference()
        {
            var ctx = new MusicDataContext(@"http://localhost/linqtordf/SparqlQuery.aspx");
            IQueryable<Album> q = from a in ctx.Albums
                                  where a.Tracks.HavingSubjectUri("http://SomeUri.tempuri.com")
                                  select a;
            Assert.IsTrue(q.AsEnumerable().Count() > 0);
        }

        [Test]
        public void TestParentToChild()
        {
            var ctx = new MusicDataContext(@"http://localhost/linqtordf/SparqlQuery.aspx");
            var album = (from a in ctx.Albums
                          where a.Name.StartsWith("Thomas")
                          select a).AsEnumerable().First();

            var tracks = from t in ctx.Tracks
                         where t.Album.HavingSubjectUri(album.InstanceUri)
                         select t;

            Assert.IsTrue(tracks.AsEnumerable().Count()>0);
            Console.WriteLine("Album: " + album.Name);
            foreach (var track in tracks)
            {
                Console.WriteLine("Track: " + track.FileLocation);
            }
        }

        [Test]
        public void TestGetAlbum()
        {
            var ctx = new MusicDataContext(@"http://localhost/linqtordf/SparqlQuery.aspx");
            IQueryable<Album> q1 = from a in ctx.Albums where a.Name == "Thomas Laqueur - History Lectures" select a;
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