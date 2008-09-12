using System;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using RdfMetal;
using Some.Namespace;
using Option = RdfMetal.Options;

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

		[Test]
		public void TestCodeGeneratorAccessesTemplates()
		{
			var cg = new CodeGenerator();
			var actualCode = cg.Generate(GetTestData().ToArray(), GetTestOpts());
			Assert.IsTrue(!string.IsNullOrEmpty(actualCode));
			Assert.IsTrue(actualCode.Contains("MyOntologyDataContext "));
			Assert.IsTrue(actualCode.Contains("RelativeUriReference=\"Class1\")]"));
			Assert.IsTrue(actualCode.Contains("\"Prop0\")]"));
			Assert.IsTrue(actualCode.Contains("_Prop0.HasLoadedOrAssignedValue"));
			Assert.IsTrue(actualCode.Contains("_Prop1.HasLoadedOrAssignedValue"));
		}

    	private IEnumerable<OntologyClass> GetTestData()
    	{
    		var ontologyClass = new OntologyClass
    		           	{
    		           		Name = "Class1",
    		           		IncomingRelationships = null,
    		           		Uri = "http://purl.aabs.org/ont/2008/09/ont#class1"
    		           	};
    		var ontologyProperties = GetProperties(ontologyClass).ToArray();
    		ontologyClass.Properties = ontologyProperties;
    		yield return ontologyClass;
    	}

    	private IEnumerable<OntologyProperty> GetProperties(OntologyClass ontologyClass)
		{
    		for (int i = 0; i < 5; i++)
    		{
			yield return new OntologyProperty
			             	{
			             		HostClass = ontologyClass,
								IsObjectProp = true,
								Name = "Prop" + i,
								Range = "Range" + i,
								RangeUri = "range" + i,
								Uri = "prop" + i
			             	};
    		}
		}
    	private Option GetTestOpts()
    	{
    		return new Option
    		       	{
					ontologyNamespace = "MyOntology",
					sourceLocation = "DomainModel.cs",
					metadataLocation = "",
					ignoreBlankNodes = false,
					ontologyPrefix = "MyOntology",
					dotnetNamespace = "MyOntology",
					namespaceReferences = "System"
    		       	};
    	}
    }
}
