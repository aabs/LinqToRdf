using System;
using System.Data.Linq;
using System.Linq;
using LinqToRdf;

[assembly: Ontology(
    BaseUri = "http://tempuri.com/ontology/",
    Name = "MyModel",
    Prefix = "MyModel",
    UrlOfOntology = "http://tempuri.com/ontology/")]

namespace MyModel
{

    public class MyModelDataContext : RDF
    {
        public MyModelDataContext(string store) : base(new TripleStore(store))
        {}
        public IQueryable<ModelClass1> ModelClass1s
        {
            get { return ForType<ModelClass1>(); }
        }
        public IQueryable<ModelClass2> ModelClass2s
        {
            get { return ForType<ModelClass2>(); }
        }
  }

[OwlResource(OntologyName="MyModel", RelativeUriReference="Class1")]
public partial class ModelClass1 : OwlInstanceSupertype
{
[OwlResource(OntologyName="MyModel", RelativeUriReference="attr1")]
		public string ModelAttribute1{get;set;}
[OwlResource(OntologyName="MyModel", RelativeUriReference="someRelationship")]
	EntitySet<ModelClass2> child { get; set; }
}

[OwlResource(OntologyName="MyModel", RelativeUriReference="")]
public partial class ModelClass2 : OwlInstanceSupertype
{
[OwlResource(OntologyName="MyModel", RelativeUriReference="attr2")]
		public int ModelAttribute1{get;set;}
}

}