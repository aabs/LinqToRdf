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

namespace LinqToRdf
{
    using Mapping = Dictionary<string, OntologyAttribute>;
    public class NamespaceManager
    {
        private char generatedNamespaceChar = 'a';

        Mapping store = new Mapping();
        public NamespaceManager()
        {
        }
        public NamespaceManager(Mapping store)
        {
            this.store = new Mapping(store);
        }
        public NamespaceManager(IEnumerable<OntologyAttribute> input)
        {
            foreach (OntologyAttribute attr in input)
            {
                this[attr.Name] = attr;
            }
        }
        public NamespaceManager(NamespaceManager nsm)
        {
            foreach (string name in nsm.Names)
            {
                this[name] = nsm[name];
            }
        }
        public int Count
        {
            get
            {
                return store.Count;
            }
        }
        public bool HasDefault
        {
            get
            {
                return store.ContainsKey("");
            }
        }
        public OntologyAttribute Default
        {
            get
            {
                return this[""];
            }
            set
            {
                this[""] = value;
            }
        }
        public OntologyAttribute this[string name]
        {
            get
            {
                if (name == null)
                    throw new ArgumentNullException("name cannot be null");
                return store.ContainsKey(name) ? store[name]:null;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value cannot be null");
                if (name == null)
                    throw new ArgumentNullException("name cannot be null");
                if (store.ContainsKey(name))
                    throw new ArgumentException("an ontology already exists by that name");
                store[name] = value;
            }
        }
        public IEnumerable<string> Names
        {
            get
            {
                return store.Keys;
            }
        }
        public IEnumerable<string> Namespaces
        {
            get
            {
                foreach (var item in store.Values)
                {
                    yield return item.BaseUri;
                }
            }
        }
        public IEnumerable<OntologyAttribute> Ontologies
        {
            get
            {
                foreach (var item in store.Values)
                {
                    yield return item;
                }
            }
        }
        public bool HasOntologyFor(Type t)
        {
            return store.ContainsValue(t.GetOntology());
        }
        public void Add(Type t)
        {
            OntologyAttribute attr = t.GetOntology();
            if(!store.ContainsKey(attr.Name))
                store[attr.Name] = attr;
        }
        public void Rename(string fromName, string toName)
        {
            OntologyAttribute ontology = store[fromName];
            store.Remove(fromName);
            store.Add(toName, ontology);
        }
        public void Remove(string name)
        {
            if (store.ContainsKey(name))
                store.Remove(name);
        }
        public void MakeDefaultNamespace(string name)
        {
            Rename(name, "");
        }
        public void AddNewDefaultNamespace(OntologyAttribute ontology, string name)
        {
            Rename("", name);
            Default = ontology;
        }

        public string CreateNewPrefixFor(OntologyAttribute ontology)
        {
            string result = generatedNamespaceChar.ToString();
            IncrementNamespace();
            return result;
        }
        private void IncrementNamespace()
        {
            int tmpInt = Convert.ToInt32(generatedNamespaceChar);
            tmpInt++;
            generatedNamespaceChar = Convert.ToChar(tmpInt);
        }

    }
}