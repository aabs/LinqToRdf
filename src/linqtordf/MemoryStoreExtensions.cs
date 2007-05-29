using System;
using System.Diagnostics;
using System.Reflection;
using SemWeb;

namespace LinqToRdf
{
    public static class OntologyHelper
    {
        public static readonly Entity rdfType = "http://www.w3.org/1999/02/22-rdf-syntax-ns#type";
    }


    public static class MemoryStoreExtensions
    {
        public static void Add(this MemoryStore ms, OwlInstanceSupertype oc)
        {
            Type t = oc.GetType();
            Debug.WriteLine(oc.InstanceUri);
            PropertyInfo[] pia = t.GetProperties();
            ms.Add(new Statement((Entity)oc.InstanceUri, OntologyHelper.rdfType, (Entity)OwlClassSupertype.GetOwlClassUri(t)));
            foreach (PropertyInfo pi in pia)
            {
                if(IsPersistentProperty(pi))
                {
                    AddPropertyToStore(oc, pi, ms);
                }
            }
        }
        private static bool IsPersistentProperty(PropertyInfo pi)
        {
            return pi.GetCustomAttributes(typeof (OwlPropertyAttribute), true).Length > 0;
        }
        private static void AddPropertyToStore(OwlInstanceSupertype track, PropertyInfo pi, MemoryStore ms)
        {
            Add(track.InstanceUri, OwlClassSupertype.GetPropertyUri(pi.ReflectedType, pi.Name), pi.GetValue(track, null).ToString(), ms);
        }
        public static void Add(string s, string p, string o, MemoryStore ms)
        {
            if(!Empty(s) && !Empty(p) && !Empty(o))
                ms.Add(new Statement(new Entity(s), new Entity(p), new Literal(o), Statement.DefaultMeta));
        }
        private static bool Empty(string s)
        {
            return (s == null || s.Length == 0);
        }
    }

}