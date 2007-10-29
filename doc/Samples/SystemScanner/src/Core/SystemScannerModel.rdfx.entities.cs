using System;
using LinqToRdf;
using System.Data.Linq;

namespace SystemScannerModel
{

    [OwlResource(OntologyName = "$fileinputname$", RelativeUriReference = "Artifact")]
    public partial class Artifact : OwlInstanceSupertype
    {
        [OwlResource(OntologyName = "SystemScannerModel ", RelativeUriReference = "artifactExists")]
        public bool ArtifactExists { get; set; }
        [OwlResource(OntologyName = "SystemScannerModel ", RelativeUriReference = "dateCreated")]
        public DateTime DateCreated { get; set; }
        [OwlResource(OntologyName = "SystemScannerModel ", RelativeUriReference = "dateLastModified")]
        public DateTime DateLastModified { get; set; }
        [OwlResource(OntologyName = "SystemScannerModel ", RelativeUriReference = "filePath")]
        public string FilePath { get; set; }
        [OwlResource(OntologyName = "SystemScannerModel ", RelativeUriReference = "isReadOnly")]
        public bool IsReadOnly { get; set; }
    }

    [OwlResource(OntologyName = "$fileinputname$", RelativeUriReference = "assembly")]
    public partial class Assembly : Artifact
    {
        [OwlResource(OntologyName = "SystemScannerModel ", RelativeUriReference = "isSigned")]
        public bool IsSigned { get; set; }
        [OwlResource(OntologyName = "SystemScannerModel ", RelativeUriReference = "strongKey")]
        public string StrongKey { get; set; }
        [OwlResource(OntologyName = "SystemScannerModel ", RelativeUriReference = "version")]
        public string Version { get; set; }
    }
}