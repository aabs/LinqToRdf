/* 
 * Copyright (C) 2007, Andrew Matthews http://aabs.wordpress.com/
 *
 * This file is Free Software and part of LinqToRdf http://code.google.com/p/linqtordf/
 *
 * It is licensed under the following license:
 *   - Berkeley License, V2.0 or any newer version
 *
 * You may not use this file except in compliance with the above license.
 *
 * See http://code.google.com/p/linqtordf/ for the complete text of the license agreement.
 *
 */
using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdfMusic;
using LinqToRdf;
using SemWeb;
using SemWeb.Inference;

namespace RdfSerialisationTest
{
    /// <summary>
    /// Summary description for IntegrationTests
    /// </summary>
    [TestClass]
    public class IntegrationTests : HighLevelTests
    {
        public IntegrationTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            if (store != null)
                store = null;
        }

        #endregion

        #region working tests

        [TestMethod]
        public void QueryWithProjection()
        {
            CreateMemoryStore();
            TripleStore ts = new TripleStore();
            ts.LocalTripleStore = store;
            IRdfQuery<Track> qry = new RDF(ts).ForType<Track>();
            var q = from t in qry
                    where t.Year == "2006" &&
                            t.GenreName == "History 5 | Fall 2006 | UC Berkeley"
                    select new { t.Title, t.FileLocation };
            foreach (var track in q)
            {
                Console.WriteLine(track.Title + ": " + track.FileLocation);
            }
        }

        #endregion

        #region current tests

        [TestMethod]
        public void SparqlQueryAll()
        {
            TripleStore ts = CreateSparqlTripleStore();
            IRdfQuery<Track> qry = new RDF(ts).ForType<Track>();
            var q = from t in qry select t;
            List<Track> lt = new List<Track>(q);
            foreach (var track in q)
            {
                Console.WriteLine("Track: " + track.Title);
            }
            Assert.IsTrue(lt.Count > 1);
        }

        [TestMethod]
        public void SparqlQuery()
        {
            TripleStore ts = CreateSparqlTripleStore();
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

        [TestMethod]
        public void SparqlQueryUsingHttp()
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

        [TestMethod]
        public void SparqlQueryUsingCachedResults()
        {
            TripleStore ts = CreateSparqlTripleStore();
            IRdfQuery<Track> qry = new RDF(ts).ForType<Track>();
            var q = from t in qry
                    where t.Year == "2007" &&
                    t.GenreName == "Rory Blyth: The Smartest Man in the World"
                    select new { t.Title, t.FileLocation };
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
        [TestMethod]
        public void SparqlQueryWithTheLot()
        {
            TripleStore ts = CreateSparqlTripleStore();
            IRdfQuery<Track> qry = new RDF(ts).ForType<Track>();
            var q = (from t in qry
                     where t.Year == "2006" &&
                     t.GenreName == "History 5 | Fall 2006 | UC Berkeley"
                     orderby t.FileLocation
                     select new { t.Title, t.FileLocation }).Skip(10).Take(5);
            foreach (var track in q)
            {
                Console.WriteLine(track.Title + ": " + track.FileLocation);
            }
        }


        [TestMethod]
        public void SparqlQueryOrdered()
        {
            TripleStore ts = CreateSparqlTripleStore();
            IRdfQuery<Track> qry = new RDF(ts).ForType<Track>();
            var q = from t in qry
                    where t.Year == "2006" &&
                    t.GenreName == "History 5 | Fall 2006 | UC Berkeley"
                    orderby t.FileLocation
                    select new { t.Title, t.FileLocation };
            foreach (var track in q)
            {
                Console.WriteLine(track.Title + ": " + track.FileLocation);
            }
        }

        #endregion

        #region unstarted tests

        [TestMethod]
        public void Query1()
        {
            CreateMemoryStore();
            TripleStore ts = new TripleStore();
            ts.LocalTripleStore = store;
            IQueryable<Track> qry = new RDF(ts).ForType<Track>();
            var q = from t in qry
                    where t.ArtistName == "Thomas Laqueur"
                    select t;
            List<Track> resultList = new List<Track>();
            resultList.AddRange(q);
        }

        [TestMethod]
        public void Query3()
        {
            TripleStore ts = CreateOnlineTripleStore();
            IRdfQuery<Track> qry = new RDF(ts).ForType<Track>(); // should deduce that it is N3 and open correctly
            var q = from t in qry
                    where Convert.ToInt32(t.Year) > 1998 &&
                    t.GenreName == "History 5 | Fall 2006 | UC Berkeley"
                    select t;
            foreach (Track track in q)
            {
                Console.WriteLine(track.Title + ": " + track.FileLocation);
            }
        }

        [TestMethod]
        public void Query5()
        {
            TripleStore ts = CreateOnlineTripleStore();
            RDF ctx = new RDF(ts);
            IRdfQuery<Track> qry = ctx.ForType<Track>();
            var q = from t in qry
                    where t.GenreName == "Rory Blyth: The Smartest Man in the World"
                    select t;
            foreach (Track track in q)
            {
                track.Rating = 5;
            }
            ctx.AcceptChanges();
        }

        #endregion

        #region Helpers

        #endregion
    }
}
