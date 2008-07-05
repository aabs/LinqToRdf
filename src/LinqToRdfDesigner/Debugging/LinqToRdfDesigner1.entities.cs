using System;
using System.Data.Linq;
using System.Linq;
using LinqToRdf;

[assembly: Ontology(
    BaseUri = "http://aabs.purl.org/ontologies/2007/04/music#",
    Name = "Music",
    Prefix = "Music",
    UrlOfOntology = "http://aabs.purl.org/ontologies/2007/04/music#")]

namespace Music
{

    public partial class MusicDataContext : RdfDataContext
    {
        public MusicDataContext(string store) : base(new TripleStore(store))
        {}
        public IQueryable<Album> Albums
        {
            get { return ForType<Album>(); }
        }
        public IQueryable<Track> Tracks
        {
            get { return ForType<Track>(); }
        }
  }


[OwlResource(OntologyName="Music", RelativeUriReference="Album")]
public partial class Album : OwlInstanceSupertype
{
[OwlResource(OntologyName="Music", RelativeUriReference="name")]
		public string Name{get;set;}
		
        private EntitySet<Track> _Tracks = new EntitySet<Track>();
        [OwlResource(OntologyName = "Music", RelativeUriReference = "isTrackOn")]
        public EntitySet<Track> Tracks
        {
            get
            {
                if (_Tracks.HasLoadedOrAssignedValues)
                    return _Tracks;
                if (DataContext != null)
                {
                    var ctx = (MusicDataContext)DataContext;
                    _Tracks.SetSource(from t in ctx.Tracks
													where t.Album.HavingSubjectUri(this.InstanceUri)
													select t);
                }
                return _Tracks;
            }
        }
}

[OwlResource(OntologyName="Music", RelativeUriReference="Track")]
public partial class Track : OwlInstanceSupertype
{
[OwlResource(OntologyName="Music", RelativeUriReference="title")]
		public string Title{get;set;}
[OwlResource(OntologyName="Music", RelativeUriReference="fileLocation")]
		public string FileLocation{get;set;}
        [OwlResource(OntologyName = "Music", RelativeUriReference = "isTrackOn")]
        public string AlbumUri { get; set; }

        private EntityRef<Album> _Album { get; set; }
        [OwlResource(OntologyName = "Music", RelativeUriReference = "isTrackOn")]
        public Album Album
        {
            get
            {
                if (_Album.HasLoadedOrAssignedValue)
                    return _Album.Entity;
                if (DataContext != null)
                {
                    var ctx = (MusicDataContext)DataContext;
                    _Album = new EntityRef<Album>(from x in ctx.Albums where x.HasInstanceUri(AlbumUri) select x);
                    return _Album.Entity;
                }
                return null;
            }
        }
		
		}
}