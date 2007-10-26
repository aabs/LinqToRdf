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
using ID3Lib;
using LinqToRdf;
using System.Data.Linq;

namespace RdfMusic
{
	[OwlResource(OntologyName="Music", RelativeUriReference="Track")]
	public class Track : OwlInstanceSupertype
	{

		[OwlResource(OntologyName = "Music", RelativeUriReference="title")]
		public string Title
		{
			get { return title; }
			set { title = value; }
		}

		[OwlResource(OntologyName = "Music", RelativeUriReference="artistName")]
		public string ArtistName
		{
			get { return artistName; }
			set { artistName = value; }
		}

		[OwlResource(OntologyName = "Music", RelativeUriReference="albumName")]
		public string AlbumName
		{
			get { return albumName; }
			set { albumName = value; }
		}

		[OwlResource(OntologyName = "Music", RelativeUriReference="year")]
		public string Year
		{
			get { return year; }
			set { year = value; }
		}

		[OwlResource(OntologyName = "Music", RelativeUriReference="genreName")]
		public string GenreName
		{
			get { return genreName; }
			set { genreName = value; }
		}

		[OwlResource(OntologyName = "Music", RelativeUriReference="comment")]
		public string Comment
		{
			get { return comment; }
			set { comment = value; }
		}

		[OwlResource(OntologyName = "Music", RelativeUriReference="fileLocation")]
		public string FileLocation
		{
			get { return fileLocation; }
			set { fileLocation = value; }
		}

		[OwlResource(OntologyName = "Music", RelativeUriReference="rating")]
		public int Rating
		{
			get { return rating; }
			set { rating = value; }
		}

		private string title;
		private string artistName;
		private string albumName;
		private string year;
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
			year = th.Year;
			genreName = th.Genere;
			comment = th.Comment;
		}

		public Track()
		{
		}
	}

    [OwlResource(OntologyName = "Music", RelativeUriReference = "Album")]
	public class Album : OwlInstanceSupertype
	{
		[OwlResource(OntologyName = "Music", RelativeUriReference="name")]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		private string name;

        public EntitySet<Track> Tracks { get; set; }
	}
}