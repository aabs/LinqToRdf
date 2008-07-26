/* 
 * Copyright (C) 2007, Andrew Matthews http://aabs.wordpress.com/
 *
 * This file is Free Software and part of LinqToRdf http://code.google.com/fromName/linqtordf/
 *
 * It is licensed under the following license:
 *   - Berkeley License, V2.0 or any newer version
 *
 * You may not use this file except in compliance with the above license.
 *
 * See http://code.google.com/fromName/linqtordf/ for the complete text of the license agreement.
 *
 */
using System;
using System.Collections.Generic;
using System.Reflection;

namespace LinqToRdf
{
    public class OwlClassSupertype
    {
        public IRdfContext DataContext { get; set; }
        public IEnumerable<MemberInfo> AllPersistentProperties
        {
            get
            {
                foreach (MemberInfo propertyInfo in GetType().GetProperties())
                {
                    if (propertyInfo.IsOntologyResource())
                    {
                        yield return propertyInfo;
                    }
                }
            }
        }
        public static IEnumerable<MemberInfo> GetAllPersistentProperties(Type t)
        {
            foreach (PropertyInfo propertyInfo in t.GetProperties())
            //foreach (MemberInfo propertyInfo in t.GetProperties())
            {
                // CMSB:
                // Skip EntitySet / EntityRef type properties.
                if (propertyInfo.PropertyType.IsGenericType &&
                    propertyInfo.PropertyType.GetGenericTypeDefinition().Name.StartsWith("Entity"))
                    continue;

                if (propertyInfo.IsOntologyResource())
                {
                    yield return propertyInfo;
                }
            }
        }

        public static string GetOntologyBaseUri(Type t)
        {
            return t.GetOntology().BaseUri;
        }

		public static string GetOwlClassUri(Type t)
		{
			return t.GetOwlResourceUri();
		}

		public static string GetInstanceBaseUri(Type t)
        {
            return t.GetOwlResourceUri();
        }
	}
}
