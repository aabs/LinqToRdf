using System;
using ID3Lib;
using LinqToRdf;

namespace RdfMusic
{
    [OntologyBaseUri("http://aabs.purl.org/ontologies/2007/04/music#")]
    [OwlClass("Track", true)]
    public class Track : OwlInstanceSupertype
    {

        [OwlProperty("title", true)]
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        [OwlProperty("artistName", true)]
        public string ArtistName
        {
            get { return artistName; }
            set { artistName = value; }
        }

        [OwlProperty("albumName", true)]
        public string AlbumName
        {
            get { return albumName; }
            set { albumName = value; }
        }

        [OwlProperty("year", true)]
        public int Year
        {
            get { return year; }
            set { year = value; }
        }

        [OwlProperty("genreName", true)]
        public string GenreName
        {
            get { return genreName; }
            set { genreName = value; }
        }

        [OwlProperty("comment", true)]
        public string Comment
        {
            get { return comment; }
            set { comment = value; }
        }

        [OwlProperty("fileLocation", true)]
        public string FileLocation
        {
            get { return fileLocation; }
            set { fileLocation = value; }
        }

        [OwlProperty("rating", true)]
        public int Rating
        {
            get { return rating; }
            set { rating = value; }
        }

        private string title;
        private string artistName;
        private string albumName;
        private int year;
        private string genreName;
        private string comment;
        private string fileLocation;
        private int rating;

        public Track(TagHandler th, string fileLocation)
        {
            this.fileLocation = fileLocation;
            title = th.Track;
            artistName = th.Artist;
            albumName = th.Album;
            year = Convert.ToInt32(th.Year);
            genreName = th.Genere;
            comment = th.Comment;
        }

        public Track()
        {
        }
    }
}