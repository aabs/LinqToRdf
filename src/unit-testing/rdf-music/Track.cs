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
		public string Year
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

	[OntologyBaseUri("http://aabs.purl.org/ontologies/2007/04/music#")]
	[OwlClass("Album", true)]
	public class Album : OwlInstanceSupertype
	{
		[OwlProperty("name", true)]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		private string name;
	}
}