using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LinqToRdf;
using System.Data.Linq;
using System;

namespace RdfMetal.Music
{
/*
[assembly: Ontology(
    BaseUri = "http://aabs.purl.org/ontologies/2007/04/music#",
    Name = "music",
    Prefix = "music",
    UrlOfOntology = "http://aabs.purl.org/ontologies/2007/04/music#")]
*/

    public partial class musicDataContext : RdfDataContext
    {
        public musicDataContext(TripleStore store) : base(store)
        {
        }
        public musicDataContext(string store) : base(new TripleStore(store))
        {
        }

		        public IQueryable<ProducerOfMusic> ProducerOfMusics
		        {
		            get
		            {
		                return ForType<ProducerOfMusic>();
		            }
		        }
		
		        public IQueryable<SellerOfMusic> SellerOfMusics
		        {
		            get
		            {
		                return ForType<SellerOfMusic>();
		            }
		        }
		
		        public IQueryable<NamedThing> NamedThings
		        {
		            get
		            {
		                return ForType<NamedThing>();
		            }
		        }
		
		        public IQueryable<TemporalThing> TemporalThings
		        {
		            get
		            {
		                return ForType<TemporalThing>();
		            }
		        }
		
		        public IQueryable<Person> Persons
		        {
		            get
		            {
		                return ForType<Person>();
		            }
		        }
		
		        public IQueryable<Band> Bands
		        {
		            get
		            {
		                return ForType<Band>();
		            }
		        }
		
		        public IQueryable<Studio> Studios
		        {
		            get
		            {
		                return ForType<Studio>();
		            }
		        }
		
		        public IQueryable<Music> Musics
		        {
		            get
		            {
		                return ForType<Music>();
		            }
		        }
		
		        public IQueryable<Album> Albums
		        {
		            get
		            {
		                return ForType<Album>();
		            }
		        }
		
		        public IQueryable<Track> Tracks
		        {
		            get
		            {
		                return ForType<Track>();
		            }
		        }
		
		        public IQueryable<Song> Songs
		        {
		            get
		            {
		                return ForType<Song>();
		            }
		        }
		
		        public IQueryable<Mp3File> Mp3Files
		        {
		            get
		            {
		                return ForType<Mp3File>();
		            }
		        }
		
		        public IQueryable<Genre> Genres
		        {
		            get
		            {
		                return ForType<Genre>();
		            }
		        }
		

    }

[OwlResource(OntologyName="music", RelativeUriReference="ProducerOfMusic")]
public partial class ProducerOfMusic : OwlInstanceSupertype
{
#region Datatype properties
#endregion

#region Incoming relationships properties
#endregion

#region Object properties
#endregion
}

[OwlResource(OntologyName="music", RelativeUriReference="SellerOfMusic")]
public partial class SellerOfMusic : OwlInstanceSupertype
{
#region Datatype properties
#endregion

#region Incoming relationships properties
#endregion

#region Object properties
#endregion
}

[OwlResource(OntologyName="music", RelativeUriReference="NamedThing")]
public partial class NamedThing : OwlInstanceSupertype
{
#region Datatype properties
  [OwlResource(OntologyName = "music", RelativeUriReference = "name")]
  public string name {get;set;} // NamedThing

#endregion

#region Incoming relationships properties
#endregion

#region Object properties
#endregion
}

[OwlResource(OntologyName="music", RelativeUriReference="TemporalThing")]
public partial class TemporalThing : OwlInstanceSupertype
{
#region Datatype properties
#endregion

#region Incoming relationships properties
#endregion

#region Object properties
#endregion
}

[OwlResource(OntologyName="music", RelativeUriReference="Person")]
public partial class Person : OwlInstanceSupertype
{
#region Datatype properties
#endregion

#region Incoming relationships properties
#endregion

#region Object properties
#endregion
}

[OwlResource(OntologyName="music", RelativeUriReference="Band")]
public partial class Band : OwlInstanceSupertype
{
#region Datatype properties
#endregion

#region Incoming relationships properties
#endregion

#region Object properties
#endregion
}

[OwlResource(OntologyName="music", RelativeUriReference="Studio")]
public partial class Studio : OwlInstanceSupertype
{
#region Datatype properties
#endregion

#region Incoming relationships properties
#endregion

#region Object properties
#endregion
}

[OwlResource(OntologyName="music", RelativeUriReference="Music")]
public partial class Music : OwlInstanceSupertype
{
#region Datatype properties
#endregion

#region Incoming relationships properties
#endregion

#region Object properties
#endregion
}

[OwlResource(OntologyName="music", RelativeUriReference="Album")]
public partial class Album : OwlInstanceSupertype
{
#region Datatype properties
  [OwlResource(OntologyName = "music", RelativeUriReference = "year")]
  public int year {get;set;} // Album

#endregion

#region Incoming relationships properties
#endregion

#region Object properties
#endregion
}

[OwlResource(OntologyName="music", RelativeUriReference="Track")]
public partial class Track : OwlInstanceSupertype
{
#region Datatype properties
  [OwlResource(OntologyName = "music", RelativeUriReference = "title")]
  public string title {get;set;} // Track
  [OwlResource(OntologyName = "music", RelativeUriReference = "artistName")]
  public string artistName {get;set;} // Track
  [OwlResource(OntologyName = "music", RelativeUriReference = "albumName")]
  public string albumName {get;set;} // Track
  [OwlResource(OntologyName = "music", RelativeUriReference = "genreName")]
  public string genreName {get;set;} // Track
  [OwlResource(OntologyName = "music", RelativeUriReference = "comment")]
  public string comment {get;set;} // Track
  [OwlResource(OntologyName = "music", RelativeUriReference = "fileLocation")]
  public string fileLocation {get;set;} // Track

#endregion

#region Incoming relationships properties
#endregion

#region Object properties
[OwlResource(OntologyName = "music", RelativeUriReference = "isTrackOn")]
public string isTrackOnUri { get; set; }

private EntityRef<Album> _isTrackOn { get; set; }

[OwlResource(OntologyName = "music", RelativeUriReference = "isTrackOn")]
public Album isTrackOn
{
    get
    {
        if (_isTrackOn.HasLoadedOrAssignedValue)
            return _isTrackOn.Entity;
        if (DataContext != null)
        {
            var ctx = (musicDataContext)DataContext;
            _isTrackOn = new EntityRef<Album>(from x in ctx.Albums where x.HasInstanceUri(isTrackOnUri) select x);
            return _isTrackOn.Entity;
        }
        return null;
    }
}

#endregion
}

[OwlResource(OntologyName="music", RelativeUriReference="Song")]
public partial class Song : OwlInstanceSupertype
{
#region Datatype properties
#endregion

#region Incoming relationships properties
#endregion

#region Object properties
#endregion
}

[OwlResource(OntologyName="music", RelativeUriReference="Mp3File")]
public partial class Mp3File : OwlInstanceSupertype
{
#region Datatype properties
#endregion

#region Incoming relationships properties
#endregion

#region Object properties
#endregion
}

[OwlResource(OntologyName="music", RelativeUriReference="Genre")]
public partial class Genre : OwlInstanceSupertype
{
#region Datatype properties
#endregion

#region Incoming relationships properties
#endregion

#region Object properties
#endregion
}



}