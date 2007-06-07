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
using System.Query;

namespace LinqToRdf
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property)]
	public class OwlResourceSupertypeAttribute : Attribute
	{
		public string Uri
		{
			get { return uri; }
		}

		private readonly string uri;

		public bool IsRelativeUri
		{
			get { return isRelativeUri; }
		}

		private readonly bool isRelativeUri;

		public OwlResourceSupertypeAttribute(string uri)
			: this(uri, false)
		{
		}

		public OwlResourceSupertypeAttribute(string uri, bool isRelativeUri)
		{
			this.uri = uri;
			this.isRelativeUri = isRelativeUri;
		}
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property)]
	public class OwlClassAttribute : OwlResourceSupertypeAttribute
	{
		public OwlClassAttribute(string uri)
			: base(uri, false)
		{
		}

		public OwlClassAttribute(string uri, bool isRelativeUri)
			: base(uri, isRelativeUri)
		{
		}
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class OwlPropertyAttribute : OwlResourceSupertypeAttribute
	{
		public OwlPropertyAttribute(string uri)
			: base(uri, false)
		{
		}

		public OwlPropertyAttribute(string uri, bool isRelativeUri)
			: base(uri, isRelativeUri)
		{
		}
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public class OntologyBaseUriAttribute : Attribute
	{
		public string BaseUri
		{
			get { return baseUri; }
		}

		private string baseUri;

		public OntologyBaseUriAttribute(string baseUri)
		{
			this.baseUri = baseUri;
		}
	}
}