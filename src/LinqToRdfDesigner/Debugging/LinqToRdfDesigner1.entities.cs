using System;
using System.Data.Linq;
using System.Linq;
using LinqToRdf;

[assembly: Ontology(
    BaseUri = "http://tempuri.com/ontology/",
    Name = "",
    Prefix = "",
    UrlOfOntology = "http://tempuri.com/ontology/")]

namespace 
{

    public class DataContext : RDF
    {
        public DataContext(string store) : base(new TripleStore(store))
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


[OwlResource(OntologyName="", RelativeUriReference="class1")]
public partial class ModelClass1 : OwlInstanceSupertype
{
Aggregation target: ModelClass2
		
        private EntitySet<ModelClass2> _ModelClass2s = new EntitySet<ModelClass2>();
        [OwlResource(OntologyName = "", RelativeUriReference = "hasParent")]
        public EntitySet<ModelClass2> ModelClass2s
        {
            get
            {
                if (_ModelClass2s.HasLoadedOrAssignedValues)
                    return _ModelClass2s;
                if (DataContext != null)
                {
                    _ModelClass2s.SetSource(from t in DataContext.ModelClass2s
													where t.ModelClass1.HavingSubjectUri(this.InstanceUri)
													select t);
                }
                return _ModelClass2s;
            }
        }
}

[OwlResource(OntologyName="", RelativeUriReference="class2")]
public partial class ModelClass2 : OwlInstanceSupertype
{
Aggregation Source: ModelClass1
}
}