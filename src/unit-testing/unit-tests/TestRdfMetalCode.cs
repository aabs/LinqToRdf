using System;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Some.Namespace;

namespace UnitTests
{
    /// <summary>
    /// Summary description for TestRdfMetalCode
    /// </summary>
    [TestFixture]
    public class TestRdfMetalCode
    {
        [Test, Ignore("DBPedia is a disorderly P.O.S.")]
        public void TestGetPeopleFromDbPedia()
        {
            var ctx = new MyOntologyDataContext("http://DBpedia.org/sparql");
            var q = from p in ctx.Persons select p;
            foreach (Person person in q)
            {
                Debug.WriteLine(person.firstName + " " + person.family_name);
            }
        }
    }
}
