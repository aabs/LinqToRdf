using System;
using System.Diagnostics;
using System.Expressions;
using System.Query;
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
	public class IntegrationTests
	{
		private static MemoryStore store;
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
				where t.Year == 2006 &&
    				t.GenreName == "History 5 | Fall 2006 | UC Berkeley" 
    			select new {t.Title, t.FileLocation};
    		foreach(var track in q){
    			Trace.WriteLine(track.Title + ": " + track.FileLocation);
    		}        
    	}

		#endregion

		#region current tests

		[TestMethod]
		public void SparqlQuery()
        {
            string urlToRemoteSparqlEndpoint = @"http://localhost/MyMusicService/SparqlQuery.ashx";
			TripleStore ts = new TripleStore();
    		ts.EndpointUri = urlToRemoteSparqlEndpoint;
			ts.QueryType = QueryType.RemoteSparqlStore;
            IRdfQuery<Track> qry = new RDF(ts).ForType<Track>(); 
	        var q = from t in qry
				where t.Year == 2006 &&
				t.GenreName == "History 5 | Fall 2006 | UC Berkeley" 
				select new {t.Title, t.FileLocation};
			foreach(var track in q){
				Trace.WriteLine(track.Title + ": " + track.FileLocation);
			}        
        }
[TestMethod]
public void SparqlQueryWithTheLot()
{
    string urlToRemoteSparqlEndpoint = @"http://localhost/MyMusicService/SparqlQuery.ashx";
	TripleStore ts = new TripleStore();
	ts.EndpointUri = urlToRemoteSparqlEndpoint;
	ts.QueryType = QueryType.RemoteSparqlStore;
    IRdfQuery<Track> qry = new RDF(ts).ForType<Track>(); 
    var q = (from t in qry
		where t.Year == 2006 &&
		t.GenreName == "History 5 | Fall 2006 | UC Berkeley" 
		orderby t.FileLocation
		select new {t.Title, t.FileLocation}).Skip(10).Take(5);
	foreach(var track in q){
		Trace.WriteLine(track.Title + ": " + track.FileLocation);
	}        
}


		[TestMethod]
		public void SparqlQueryOrdered()
        {
            string urlToRemoteSparqlEndpoint = @"http://localhost/MyMusicService/SparqlQuery.ashx";
			TripleStore ts = new TripleStore();
    		ts.EndpointUri = urlToRemoteSparqlEndpoint;
			ts.QueryType = QueryType.RemoteSparqlStore;
            IRdfQuery<Track> qry = new RDF(ts).ForType<Track>(); 
	        var q = from t in qry
				where t.Year == 2006 &&
				t.GenreName == "History 5 | Fall 2006 | UC Berkeley" 
				orderby t.FileLocation
				select new {t.Title, t.FileLocation};
			foreach(var track in q){
				Trace.WriteLine(track.Title + ": " + track.FileLocation);
			}        
        }

		[TestMethod]
		public void ExplodedSparqlQuery()
		{
			ParameterExpression t = Expression.Parameter(typeof(Track), "t");
			string urlToRemoteSparqlEndpoint = "http://localhost/MyMusicService/SparqlQuery.ashx";
			MethodInfo artistNamePropInfo = propertyof(typeof(Track), "ArtistName");
			MethodInfo titlePropInfo = propertyof(typeof(Track), "Title");
			MemberExpression arg0 = Expression.Property(t,
														artistNamePropInfo);
			ConstantExpression arg1 = Expression.Constant("Jethro Tull", typeof(string));
			Expression[] whereClauseArgs = new Expression[] { 
			                                                	arg0, 
			                                                	arg1 
			                                                };
			MethodInfo eqop = typeof(string).GetMethod("op_Equality");
			MethodCallExpression whereClause = Expression.Call(eqop, t,
															   whereClauseArgs);
			Expression<Func<Track, bool>> whereLambda = Expression.Lambda<Func<Track, bool>>(
				whereClause,
				new ParameterExpression[] { t });
			Expression<Func<Track, string>> selectLambda = Expression.Lambda<Func<Track, string>>(
				Expression.Property(t, titlePropInfo),
				new ParameterExpression[] { t });
			TripleStore ts = new TripleStore();
			ts.EndpointUri = urlToRemoteSparqlEndpoint;
			RDF ctx = new RDF(ts);
			IRdfQuery<Track> qry = ctx.ForType<Track>();

			IQueryable<Track> qry2 = Queryable.Where<Track>(qry, whereLambda);
			IQueryable<string> qry3 = Queryable.Select<Track, string>(qry2, selectLambda);
			foreach (string title in qry3)
			{
				Console.WriteLine(title);
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
    		string serialisedLocation = @"";
    		Store s = new MemoryStore(new N3Reader(serialisedLocation));
			TripleStore ts = new TripleStore();
    		ts.LocalTripleStore = s;
    		IRdfQuery<Track> qry = new RDF(ts).ForType<Track>(); // should deduce that it is N3 and open correctly
    		var q = from t in qry
    		                  	where Convert.ToInt32(t.Year) > 1998 &&
    		                  	      t.GenreName == "Chillout" 
    		select t;
    		foreach(Track track in q){
    			Console.WriteLine(track.Title + ": " + track.FileLocation);
    		}        
    	}

		[TestMethod]
		public void Query5()
        {
            string urlToRemoteSparqlEndpoint = @"http://localhost/MyMusicService/SparqlQuery.ashx";
			TripleStore ts = new TripleStore();
    		ts.EndpointUri = urlToRemoteSparqlEndpoint;
            RDF ctx = new RDF(ts);
            IRdfQuery<Track> qry = ctx.ForType<Track>(); 
	        var q = from t in qry
		        where t.ArtistName == "Jethro Tull"
		        select t;
            foreach (Track track in q)
            {
                track.Rating = 5;
            }
            ctx.AcceptChanges();
        }

		#endregion

		#region Helpers

		private static void CreateMemoryStore()
		{
			string serialisedLocation = @"C:\dev\prototypes\semantic-web\src\RdfSerialisationTest\store3.n3";
			store = new MemoryStore();
			store.AddReasoner(new Euler(new N3Reader(MusicConstants.OntologyURL)));
			store.Import(new N3Reader(serialisedLocation));
		}

		private MethodInfo propertyof(Type t, string arg)
		{
			return t.GetProperty(arg).GetGetMethod();
		}

		private MethodInfo methodof(Type t, string arg)
		{
			return GetType().GetMethod(arg);
		}

		#endregion
	}
}
