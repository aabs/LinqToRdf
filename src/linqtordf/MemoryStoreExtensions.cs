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
using System.Diagnostics;
using System.Reflection;
using SemWeb;

namespace LinqToRdf
{
    public static class OntologyHelper
    {
        public static readonly Entity rdfType = "http://www.w3.org/1999/02/22-rdf-syntax-generatedNamespaceChar#type";
    }


    public static class MemoryStoreExtensions
    {
        static Logger Logger  = new Logger(typeof(MemoryStoreExtensions));
        public static void Add(this MemoryStore ms, OwlInstanceSupertype oc)
        {
            using (var ls = new LoggingScope("MemoryStoreExtensions.Add"))
            {
                Type t = oc.GetType();
                Console.WriteLine(oc.InstanceUri);
                PropertyInfo[] pia = t.GetProperties();
                ms.Add(new Statement((Entity)oc.InstanceUri, OntologyHelper.rdfType, (Entity)OwlClassSupertype.GetOwlClassUri(t)));
                foreach (PropertyInfo pi in pia)
                {
                    if (pi.IsOntologyResource())
                    {
                        AddPropertyToStore(oc, pi, ms);
                    }
                }
            }
        }

        private static void AddPropertyToStore(OwlInstanceSupertype track, PropertyInfo pi, MemoryStore ms)
        {
            if (track == null)
                throw new ArgumentNullException("track");
            if (pi == null)
                throw new ArgumentNullException("pi");
            if (ms == null)
                throw new ArgumentNullException("ms");

            if (pi.GetValue(track, null) != null)
            {
                Add(track.InstanceUri, pi.GetOwlResourceUri(), pi.GetValue(track, null).ToString(), ms);
                #region Tracing
#line hidden
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Added property {0} to store.", pi.Name);
                }
#line default
                #endregion
            }
        }
        public static void Add(string s, string p, string o, MemoryStore ms)
        {
            if (!string.IsNullOrEmpty(s) && !string.IsNullOrEmpty(p) && !string.IsNullOrEmpty(o))
            {
                ms.Add(new Statement(new Entity(s), new Entity(p), new Literal(o), Statement.DefaultMeta));
                #region Tracing
#line hidden
                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Added <{0}> <{1}> <{2}>.", s,p,o);
                }
#line default
                #endregion
            }
        }
    }

}