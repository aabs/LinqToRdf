using System;
using System.Data.Linq;
using System.Linq;
using LinqToRdf;

[assembly: Ontology(
    BaseUri = "http://tempuri.com/ontology/",
    Name = "TestModel",
    Prefix = "TestModel",
    UrlOfOntology = "http://tempuri.com/ontology/")]

namespace TestModel
{

    public class TestModelDataContext : RDF
    {
        public TestModelDataContext(string store) : base(new TripleStore(store))
        {}
        public IQueryable<Parent> Parents
        {
            get { return ForType<Parent>(); }
        }
        public IQueryable<Child> Childs
        {
            get { return ForType<Child>(); }
        }
  }


[OwlResource(OntologyName="TestModel", RelativeUriReference="parent")]
public partial class Parent : OwlInstanceSupertype
{
[OwlResource(OntologyName="TestModel", RelativeUriReference="prop1")]
		public string Property1{get;set;}
[OwlResource(OntologyName="TestModel", RelativeUriReference="hasChild")]
	EntitySet<Child> children { get; set; }
}

[OwlResource(OntologyName="TestModel", RelativeUriReference="child")]
public partial class Child : OwlInstanceSupertype
{
[OwlResource(OntologyName="TestModel", RelativeUriReference="prop2")]
		public int Property2{get;set;}
}
}