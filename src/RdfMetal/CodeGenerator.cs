using System.Collections.Generic;
using Antlr.StringTemplate;

namespace RdfMetal
{
    public class CodeGenerator
    {
        private readonly StringTemplateGroup group = new StringTemplateGroup("myGroup", @"C:\shared.datastore\repository\personal\dev\projects\semantic-web\LinqToRdf.Prototypes\RdfMetal\template");

        public string Generate(IEnumerable<OntologyClass> classes, Options opts)
        {
            StringTemplate template = group.GetInstanceOf("classes");
            template.SetAttribute("classes", classes);
            template.SetAttribute("handle", opts.ontologyPrefix);
            template.SetAttribute("uri", opts.ontologyNamespace);
            template.SetAttribute("opts", opts);
            template.SetAttribute("refs", opts.namespaceReferences.Split(','));
            return template.ToString();
        }
    }
}