using System;
using System.Collections.Generic;

namespace LinqToRdf
{
	public class NamespaceManager
	{
		public Dictionary<Type, string> typeMappings = new Dictionary<Type, string>();
		public Dictionary<string, string> namespaceUris = new Dictionary<string, string>();
		
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