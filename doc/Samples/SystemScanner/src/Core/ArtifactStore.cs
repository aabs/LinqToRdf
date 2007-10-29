using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SemWeb;
using System.IO;
using LinqToRdf;
using SemWeb.Inference;
using System.Web;
using SystemScannerModel;

namespace SystemScanner.Core
{
    public class ArtifactStore
    {
        private enum StoreFormat
        {
            N3,
            RDF,
            OWL,
            Unknown
        } ;

        private static int nscount = 1;
        Dictionary<string, string> namespaces = new Dictionary<string, string>();
        private MemoryStore store;
        public MemoryStore TripleStore
        {
            get
            {
                return store;
            }
        }

        string Read(string path)
        {
            return File.ReadAllText(path, Encoding.UTF8);
        }

        public ArtifactStore(string storeLocation)
        {
            string ontology = "";
            string data = "";
            store = new MemoryStore();
            ontology = Read(Constants.OntologyUri);
            store.AddReasoner(new Euler(new N3Reader(new StringReader(ontology))));
            if (File.Exists(storeLocation))
            {
                switch (GetStoreFormat(storeLocation))
                {
                    case StoreFormat.N3:
                        data = File.ReadAllText(storeLocation);
                        N3Reader reader = new N3Reader(new StringReader(data));
                        store.Import(reader);
                        break;
                    case StoreFormat.RDF:
                        data = File.ReadAllText(storeLocation);
                        store.Import(new RdfXmlReader(new StringReader(data)));
                        break;
                }
            }
        }

        public void Save(string storeLocation)
        {
            using (RdfWriter rxw = GetRdfWriter(storeLocation))
            {
                foreach (string key in namespaces.Keys)
                {
                    rxw.Namespaces.AddNamespace(namespaces[key], key);
                }
                rxw.Write(store);
            }
        }

        private static RdfWriter GetRdfWriter(string storeLocation)
        {
            switch (GetStoreFormat(storeLocation))
            {
                case StoreFormat.N3:
                    return new N3Writer(storeLocation);
                case StoreFormat.RDF:
                    return new RdfXmlWriter(storeLocation);
            }
            throw new ApplicationException("invalid file type");
        }

        private static StoreFormat GetStoreFormat(string location)
        {
            switch (Path.GetExtension(location).ToLowerInvariant())
            {
                case ".n3":
                    return StoreFormat.N3;
                case ".rdf":
                    return StoreFormat.RDF;
                case ".owl":
                    return StoreFormat.OWL;
                default:
                    return StoreFormat.Unknown;
            }
        }

        public void Add(Artifact t)
        {
            string uri = OwlClassSupertype.GetOntologyBaseUri(typeof(Artifact));
            if (!namespaces.ContainsValue(uri))
            {
                namespaces.Add("ns" + nscount++, uri);
            }
            if (t.InstanceUri == null) t.InstanceUri = CreateInstanceName(t);
            store.Add(t);
        }
        private string CreateInstanceName(Artifact atfct)
        {
            return OwlClassSupertype.GetInstanceBaseUri(typeof(Artifact)) + "_" + HttpUtility.UrlEncode(atfct.FilePath.GetHashCode().ToString());
        }

    }
}
