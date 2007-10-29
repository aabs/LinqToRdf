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
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace LinqToRdf
{
    /// <summary>
    /// Attribute to specify at the assembly level what ontologies are in use, and where to find them.
    /// </summary>
    /// <example>
    /// <code>
    /// [assembly: Ontology(
    ///     Name="MusicOntology",
    ///     UrlOfOntology="file:///c:/etc/dev/ontologies/2007/07/music.n3",
    ///     Prefix="music",
    ///     Uri="http://aabs.tempuri.com/ontologies/2007/07/music#",
    ///     GraphName="AABS_MUSIC_07_07",
    /// )]
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = true)]
    public sealed class OntologyAttribute : Attribute
    {
        /// <summary>
        /// internal name for the ontology, to be used elesewhere in this system.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Where to find the ontology file.
        /// </summary>
        /// <remarks>
        /// Note that this is not necessarily the URL used internally in the ontology, although it may be.
        /// </remarks>
        public string UrlOfOntology { get; set; }
        /// <summary>
        /// this is the _preferred_ prefix to be used for the ontology URI
        /// </summary>
        /// <remarks>
        /// Note that while the prefix will be used whereever possible, there may be prefix clashes
        /// that will require ontology substitution
        /// </remarks>
        public string Prefix { get; set; }
        /// <summary>
        /// The base internal URI used in the ontology.
        /// </summary>
        /// <remarks>
        /// This is not the same as the URL needed to get the ontology file.
        /// </remarks>
        public string BaseUri { get; set; }
        /// <summary>
        /// The Named Graph used in ontology triple store for that ontology. Equivalent to ontology .NET namespace.
        /// </summary>
        public string GraphName { get; set; }
    }
    
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class OwlResourceAttribute : Attribute
    {
        /// <summary>
        /// internal name of the OntologyAttribute that defines the ontology this class is bound to
        /// </summary>
        /// <remarks>
        /// Must match the Name property of some assembly level instance of OntologyAttribute
        /// </remarks>
        public string OntologyName { get; set; }

        /// <summary>
        /// The _relative_ URI of the OWL resource this object corresponds to
        /// </summary>
        public string RelativeUriReference { get; set; }
    }

    public static class AttributeExtensions
    {
        public static bool IsOntologyResource(this MemberInfo pi)
        {
            return ((pi is PropertyInfo)&&(pi.GetCustomAttributes(typeof(OwlResourceAttribute), true).Count() > 0));
        }
        public static T[] GetAttributes<T>(this Assembly assembly)
        {
            return assembly.GetCustomAttributes(typeof(T), true) as T[];
        }
        public static OntologyAttribute[] GetAllOntologies(this Assembly assembly)
        {
            return assembly.GetAttributes<OntologyAttribute>();
        }
        public static OntologyAttribute GetOntology(this Assembly assembly, string ontologyName)
        {
            return (from a in assembly.GetAllOntologies() where a.Name == ontologyName select a).FirstOrDefault();
        }
        public static OntologyAttribute GetOntology(this Type t)
        {
            return t.Assembly.GetOntology(t.GetOwlResource().OntologyName);
        }
        public static OntologyAttribute GetOntology(this MemberInfo mi)
        {
            return mi.DeclaringType.Assembly.GetOntology(mi.GetOwlResource().OntologyName);
        }
        public static OwlResourceAttribute GetOwlResource(this Type t)
        {
            return t.GetCustomAttributes(typeof(OwlResourceAttribute), true).FirstOrDefault() as OwlResourceAttribute;
        }
        public static OwlResourceAttribute GetOwlResource(this MemberInfo mi)
        {
            return mi.GetCustomAttributes(typeof(OwlResourceAttribute), true).FirstOrDefault() as OwlResourceAttribute;
        }
        public static string GetOwlResourceUri(this Type t)
        {
            return t.GetOntology().BaseUri + t.GetOwlResource().RelativeUriReference;
        }
        public static string GetOwlResourceUri(this MemberInfo mi)
        {
            return mi.GetOntology().BaseUri + mi.GetOwlResource().RelativeUriReference;
        }
    }
}
