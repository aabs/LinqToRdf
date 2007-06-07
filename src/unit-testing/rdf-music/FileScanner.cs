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
using System.IO;
using System.Text;
using System.Web;
using ID3Lib;
using LinqToRdf;
using SemWeb;
using SemWeb.Inference;

namespace RdfMusic
{
    public class MusicStore
    {
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

        private enum StoreFormat
        {
            N3,
            RDF,
            Unknown
        } ;
        public void InitialiseStore(string storeLocation)
        {
            store = new MemoryStore();
            store.AddReasoner(new Euler(new N3Reader(MusicConstants.OntologyURL)));
            if (File.Exists(storeLocation))
            {
                switch(GetStoreFormat(storeLocation))
                {
                case StoreFormat.N3:
                    store.Import(new N3Reader(storeLocation));
                    break;
                case StoreFormat.RDF:
                    store.Import(new RdfXmlReader(storeLocation));
                    break;
                }
            }
        }

        public void WriteStoreToFile(string storeLocation)
        {
            using (RdfWriter rxw = GetRrdfWriter(storeLocation))
            {
                foreach (string key in namespaces.Keys)
                {
                    rxw.Namespaces.AddNamespace(namespaces[key], key);
                }
                rxw.Write(store);
            }
        }

        private static RdfWriter GetRrdfWriter(string storeLocation)
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
            string[] sa = location.Split(new char[] { '.' });
            if (sa[sa.Length - 1] == "n3")
            {
                return StoreFormat.N3;
            }
            else
            {
                return StoreFormat.RDF;
            }
        }

        public void Add(Track t)
        {
            string uri = OwlClassSupertype.GetOntologyBaseUri(typeof(Track));
            if (!namespaces.ContainsValue(uri))
            {
                namespaces.Add("ns" + nscount++, uri);
            }
            store.Add(t);
        }
    }

    public class FileScanner
    {
        public class NewFileScannedEventArgs : EventArgs
        {
            public string FileLocation
            {
                get { return fileLocation; }
            }

            private readonly string fileLocation;

            public NewFileScannedEventArgs(string fileLocation):base()
            {
                this.fileLocation = fileLocation;
            }
        }
        public event EventHandler<NewFileScannedEventArgs> newFileScanned;

        public void ScanFiles(string directoryLocation, MusicStore store)
        {

            foreach (Track t in GetAllTracks(directoryLocation))
            {
                t.InstanceUri = GenTrackName(t);
                store.Add(t);
                if (newFileScanned != null) newFileScanned(this, new NewFileScannedEventArgs(t.InstanceUri));
            }
        }

        private IEnumerable<Track> GetAllTracks(string location)
        {
            MP3File mp3f = new MP3File();
            foreach (FileInfo info in RdfMusic.IO.FsIter.FilesByExtension(".mp3", new DirectoryInfo(location)))
            {
                TagHandler th = null;
                try
                {
                    th = new TagHandler(mp3f.Read(info.FullName));
                }
                catch { }
                if (th != null)
                    yield return new Track(th, info.FullName);
            }
        }

        private string GenTrackName(Track track)
        {
            return OwlClassSupertype.GetInstanceBaseUri(typeof(Track)) + "_" + HttpUtility.UrlEncode(track.FileLocation.GetHashCode().ToString());
        }

    }
}
