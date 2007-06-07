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
using System.Collections.Generic;

namespace LinqToRdf
{
	public class NamespaceManager
	{
		private char ns = 'a';
		public Dictionary<Type, string> typeMappings = new Dictionary<Type, string>();
		public Dictionary<string, string> namespaceUris = new Dictionary<string, string>();

		public void RegisterType(Type t)
		{
			if(!typeMappings.ContainsKey(t))
			{
				string newNs = OwlClassSupertype.GetOntologyBaseUri(t);
				RegisterType(t, ns.ToString());
				RegisterNamespace(ns.ToString(), newNs);
				IncrementNamespace();
			}
		}

		private void IncrementNamespace()
		{
			int x = Convert.ToInt32(ns);
			x++;
			ns = Convert.ToChar(x);
		}

		public void RegisterType(Type t, string prefix)
		{
			if (!typeMappings.ContainsKey(t) && !typeMappings.ContainsValue(prefix))
			{
				typeMappings.Add(t, prefix);
				return;
			}
			throw new ApplicationException("type or prefix is already registered");
		}

		public void RegisterNamespace(string prefix, string namespaceUri)
		{
			if(!namespaceUris.ContainsKey(prefix))
			{
				namespaceUris.Add(prefix, namespaceUri);
			}
		}

	}
}