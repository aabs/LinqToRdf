using System;
using System.Collections.Generic;
using System.Text;
using LinqToRdf;
using SemWeb;
using System.IO;

namespace SystemScannerModel
{
    public static class Constants
    {
        public static readonly string OntologyUri = "C:/etc/dev/prototypes/linqtordf/SystemScanner/rdf/sys.artifacts.n3";

        // classes
        public static readonly Entity Artefact = "http://aabs.purl.org/ontologies/2007/10/sys#Artefact";
        public static readonly Entity Assembly = "http://aabs.purl.org/ontologies/2007/10/sys#Assembly";
        
        // properties
        public static readonly Entity filePath = "http://aabs.purl.org/ontologies/2007/10/sys#filePath";
        public static readonly Entity dateCreated = "http://aabs.purl.org/ontologies/2007/10/sys#dateCreated";
        public static readonly Entity dateLastModified = "http://aabs.purl.org/ontologies/2007/10/sys#dateLastModified";
        public static readonly Entity hasACL = "http://aabs.purl.org/ontologies/2007/10/sys#hasACL";
        public static readonly Entity isReadOnly = "http://aabs.purl.org/ontologies/2007/10/sys#isReadOnly";
        public static readonly Entity hasDependencyOn = "http://aabs.purl.org/ontologies/2007/10/sys#hasDependencyOn";
    }
    public partial class Artifact
    {
        public Artifact(string path)
        {
            FilePath = path;
            ArtifactExists = File.Exists(path);
            if (ArtifactExists)
            {
                DateCreated = File.GetCreationTime(path);
                DateLastModified = File.GetLastWriteTime(path);
                IsReadOnly = (File.GetAttributes(path) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
            }
        }
    }

    public partial class Assembly : Artifact
    {

        public Assembly(System.Reflection.Assembly asm)
            : base(asm.Location)
        {
            Version = asm.ImageRuntimeVersion.ToString();
            StrongKey = GetStrongKey(asm);
            IsSigned = StrongKey != null;
        }
        string GetStrongKey(System.Reflection.Assembly asm)
        {
            string key = asm.FullName.Split(',')[3].Split('=')[1];
            return key == "null" ? null : key;
        }
    }
}
