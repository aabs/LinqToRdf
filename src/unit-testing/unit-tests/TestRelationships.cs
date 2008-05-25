using System;
using System.Linq;
using LinqToRdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdfMusic;

namespace UnitTests
{
    /// <summary>
    /// Summary description for TestRelationships
    /// </summary>
    [TestClass]
    public class TestRelationships
    {
        [TestMethod]
        public void TestAlbumToTracks()
        {
            var ctx = new MusicDataContext(@"http://localhost/linqtordf/SparqlQuery.aspx");
            Album album = (from a in ctx.Albums
                           where a.Name.StartsWith("Thomas")
                           select a).First();
            Assert.IsNotNull(album);
            Assert.IsTrue(album.Tracks.Count() > 1);
        }


        [TestMethod]
        public void TestDirectReference()
        {
            var ctx = new MusicDataContext(@"http://localhost/linqtordf/SparqlQuery.aspx");
            IQueryable<Track> q = from t in ctx.Tracks
                                  where t.HasInstanceUri("http://aabs.purl.org/ontologies/2007/04/music#Track_-861912094")
                                  select t;
            Assert.IsTrue(q.Count() == 1);
        }

        [TestMethod]
        public void TestParentToChild()
        {
            var ctx = new MusicDataContext(@"http://localhost/linqtordf/SparqlQuery.aspx");
            var album = (from a in ctx.Albums
                          where a.Name.StartsWith("Thomas")
                          select a).First();

            var tracks = from t in ctx.Tracks
                         where t.Album.HavingSubjectUri(album.InstanceUri)
                         select t;

            Assert.IsTrue(tracks.Count()>0);
            Console.WriteLine("Album: " + album.Name);
            foreach (var track in tracks)
            {
                Console.WriteLine("Track: " + track.FileLocation);
            }
        }

        [TestMethod]
        public void TestChildToParent()
        {
            var ctx = new MusicDataContext(@"http://localhost/linqtordf/SparqlQuery.aspx");
            var album = (from a in ctx.Albums
                          where a.Name.StartsWith("Thomas")
                          select a).First();

            var tracks = from t in ctx.Tracks
                         where t.Album.HavingSubjectUri(album.InstanceUri)
                         select t;

            Assert.IsTrue(tracks.Count()>0);
            Console.WriteLine("Album: " + album.Name);
            foreach (var track in tracks)
            {
                Console.WriteLine("Track: " + track.FileLocation);
            }
        }

        [TestMethod]
        public void TestGetAlbum()
        {
            var ctx = new MusicDataContext(@"http://localhost/linqtordf/SparqlQuery.aspx");
            IQueryable<Album> q1 = from a in ctx.Albums where a.Name == "Thomas Laqueur - History Lectures" select a;
            Album[] aa = q1.ToArray();
            Assert.IsTrue(aa.Count() == 1);
        }

        [TestMethod]
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