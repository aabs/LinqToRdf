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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using LinqToRdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdfMusic;
using SemWeb;
using SemWeb.Inference;
using SemWeb.Query;

namespace RdfSerialisationTest
{
	[TestClass]
	public class LocalSparqlTests
	{
		#region query strings

		private string generatedQueryString2 =
			@"PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>
PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>
PREFIX xsdt: <http://www.w3.org/2001/XMLSchema#>
PREFIX fn: <http://www.w3.org/2005/xpath-functions#> 
PREFIX a: <http://aabs.purl.org/ontologies/2007/04/music#>

SELECT ?Title ?FileLocation 
WHERE {
_:t a:year ?Year ;
a:genreName ?GenreName ;
a:title ?Title ;
a:fileLocation ?FileLocation .
FILTER((regex(?Year, ""2007""))&&(regex(?GenreName, ""Rory Blyth: The Smartest Man in the World"") ))
}
";
		#endregion

		private static MemoryStore store;
		private static void CreateMemoryStore()
		{
			string serialisedLocation = @"C:\dev\semantic-web\linqtordf\src\unit-testing\unit-tests\store3.n3";
			store = new MemoryStore();
//			store.AddReasoner(new Euler(new N3Reader(MusicConstants.OntologyURL)));
			store.Import(new N3Reader(serialisedLocation));
		}

		[TestMethod]
		public void LocalSparqlQuery1()
		{
			CreateMemoryStore();
			var x = new {Title="foo", FileLocation="bar"};
			ObjectDeserialiserQuerySink sink = new ObjectDeserialiserQuerySink(typeof(Track), x.GetType());
			string qry = CreateQueryForArtist("Rory Blythe");
			Query query = new SparqlEngine(qry);
//			Query query = new GraphMatch(new N3Reader(new StringReader(qry)));
			query.Run(store, sink);

			foreach (object track in sink.DeserialisedObjects)
			{
				Console.WriteLine(track.ToString());
			}

		}
		private string CreateQueryForArtist(string artistName)
		{
//			string queryFmt = "@prefix m: <http://aabs.purl.org/ontologies/2007/04/music#> .\n";
//			foreach (PropertyInfo info in OwlClassSupertype.GetAllPersistentProperties(typeof(Track)))
//			{
//				queryFmt += string.Format("?track <{0}> ?{1} .\n", OwlClassSupertype.GetPropertyUri(typeof(Track), info.Name), info.Name);
//			}
//			queryFmt += string.Format("?track <{0}> \"{1}\" .\n", OwlClassSupertype.GetPropertyUri(typeof(Track), "ArtistName"), artistName);
			return generatedQueryString2;
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
	}
}
