/* 
 * Copyright (C) 2007, Andrew Matthews http://aabs.wordpress.com/
 *
 * This file is Free Software and part of LinqToRdf http://code.google.com/fromName/linqtordf/
 *
 * It is licensed under the following license:
 *   - Berkeley License, V2.0 or any newer version
 *
 * You may not use this file except in compliance with the above license.
 *
 * See http://code.google.com/fromName/linqtordf/ for the complete text of the license agreement.
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;
using LinqToRdf;
using NUnit.Framework;
using RdfMusic;

namespace UnitTests
{
    /// <summary>
    /// Summary description for IntegrationTests
    /// </summary>
    [TestFixture]
    public class IntegrationTests : HighLevelTests
    {
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in ontology class have run
        // [TearDown]
        // public static void MyClassCleanup() { }
        //
        // Use SetUp to run code before running each test 
        // [SetUp]
        // public void MySetUp() { }
        //
        // Use TearDown to run code after each test has run

        [TearDown]
        public void MyTearDown()
        {
        }

        [Test]
        public void Query1()
        {
            var ts = new TripleStore(CreateMemoryStore());
            IQueryable<Track> qry = new RdfDataContext(ts).ForType<Track>();
            IQueryable<Track> q = from t in qry
                                  where t.ArtistName == "Thomas Laqueur"
                                  select t;
            var resultList = new List<Track>();
            resultList.AddRange(q);
        }

        [Test]
        public void Query3()
        {
            TripleStore ts = CreateOnlineTripleStore();
            IRdfQuery<Track> qry = new RdfDataContext(ts).ForType<Track>();
                // should deduce that it is N3 and open correctly
            Track[] q = (from t in qry
                         where Convert.ToInt32(t.Year) > 1998 &&
                               t.GenreName == "Rory Blyth: The Smartest Man in the World"
                         select t).ToArray();
            Assert.IsTrue(q.Length > 0);
        }

        [Test]
        public void Query5()
        {
            TripleStore ts = CreateOnlineTripleStore();
            var ctx = new RdfDataContext(ts);
            IRdfQuery<Track> qry = ctx.ForType<Track>();
            IQueryable<Track> q = from t in qry
                                  where t.GenreName == "Rory Blyth: The Smartest Man in the World"
                                  select t;
            foreach (Track track in q)
            {
                track.Rating = 5;
            }
            ctx.SubmitChanges();
        }

        [Test]
        public void QueryWithProjection()
        {
            var ctx = new MusicDataContext(@"http://localhost/linqtordf/SparqlQuery.aspx");
            var q = from t in ctx.Tracks
                    where t.Year == "2006" &&
                          t.GenreName == "History 5"
                    select new {t.Title, t.FileLocation};
            Assert.IsTrue(q.ToList().Count() == 2);
        }

        [Test]
        public void SparqlQuery()
        {
            TripleStore ts = CreateSparqlTripleStore();
            IRdfQuery<Track> qry = new RdfDataContext(ts).ForType<Track>();
            var q = from t in qry
                    where t.Year == "2007" &&
                          t.GenreName == "Rory Blyth: The Smartest Man in the World"
                    select new {t.Title, t.FileLocation};
            foreach (var track in q)
            {
                Console.WriteLine(track.Title + ": " + track.FileLocation);
            }
        }

        [Test]
        public void SparqlQueryAll()
        {
            TripleStore ts = CreateSparqlTripleStore();
            IRdfQuery<Track> qry = new RdfDataContext(ts).ForType<Track>();
            IQueryable<Track> q = from t in qry select t;
            var lt = new List<Track>(q);
            foreach (Track track in q)
            {
                Console.WriteLine("Track: " + track.Title);
            }
            Assert.IsTrue(lt.Count > 1);
        }

        [Test]
        public void SparqlQueryOrdered()
        {
            TripleStore ts = CreateSparqlTripleStore();
            IRdfQuery<Track> qry = new RdfDataContext(ts).ForType<Track>();
            var q = from t in qry
                    where t.Year == "2006" &&
                          t.GenreName == "History 5 | Fall 2006 | UC Berkeley"
                    orderby t.FileLocation
                    select new {t.Title, t.FileLocation};
            foreach (var track in q)
            {
                Console.WriteLine(track.Title + ": " + track.FileLocation);
            }
        }

        [Test]
        public void SparqlQueryUsingCachedResults()
        {
            TripleStore ts = CreateSparqlTripleStore();
            IRdfQuery<Track> qry = new RdfDataContext(ts).ForType<Track>();
            var q = from t in qry
                    where t.Year == "2007" &&
                          t.GenreName == "Rory Blyth: The Smartest Man in the World"
                    select new {t.Title, t.FileLocation};
            foreach (var track in q)
            {
                Console.WriteLine(track.Title + ": " + track.FileLocation);
            }
            // this should not invoke query parsing or execution
            foreach (var track in q)
            {
                Console.WriteLine("Title: " + track.Title);
            }
        }

        [Test]
        public void SparqlQueryUsingHttp()
        {
            TripleStore ts = CreateOnlineTripleStore();
            IRdfQuery<Track> qry = new RdfDataContext(ts).ForType<Track>();
            var q = from t in qry
                    where t.Year == "2007" &&
                          t.GenreName == "Rory Blyth: The Smartest Man in the World"
                    select new {t.Title, t.FileLocation};
            foreach (var track in q)
            {
                Console.WriteLine(track.Title + ": " + track.FileLocation);
            }
        }

        [Test]
        public void SparqlQueryWithTheLot()
        {
            TripleStore ts = CreateSparqlTripleStore();
            MusicDataContext ctx = new MusicDataContext(@"http://localhost/linqtordf/SparqlQuery.aspx");
            var q = (from t in ctx.Tracks
                     where t.Year == "2006" &&
                           t.GenreName == "History 5 | Fall 2006 | UC Berkeley"
                     orderby t.FileLocation
                     select new {t.Title, t.FileLocation}).Skip(10).Take(5);
            foreach (var track in q)
            {
                Console.WriteLine(track.Title + ": " + track.FileLocation);
            }
        }
    }
}