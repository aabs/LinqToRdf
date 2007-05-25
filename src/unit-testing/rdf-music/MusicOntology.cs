using SemWeb;

namespace RdfMusic
{
    public static class MusicConstants
    {
        public static readonly string OntologyURL = "C:/dev/prototypes/semantic-web/ontologies/2007/04/music/music.n3";

        public static readonly Entity Title = "http://aabs.purl.org/ontologies/2007/04/music#title";
        public static readonly Entity ArtistName = "http://aabs.purl.org/ontologies/2007/04/music#artistName";
        public static readonly Entity AlbumName = "http://aabs.purl.org/ontologies/2007/04/music#albumName";
        public static readonly Entity Year = "http://aabs.purl.org/ontologies/2007/04/music#year";
        public static readonly Entity GenreName = "http://aabs.purl.org/ontologies/2007/04/music#genreName";
        public static readonly Entity Comment = "http://aabs.purl.org/ontologies/2007/04/music#comment";
        public static readonly Entity FileLocation = "http://aabs.purl.org/ontologies/2007/04/music#fileLocation";
    }
}