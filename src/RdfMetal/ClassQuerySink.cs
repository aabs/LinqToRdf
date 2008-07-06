using System.Collections.Generic;
using System.Collections.Specialized;
using SemWeb;
using SemWeb.Query;

namespace RdfMetal
{
    public class ClassQuerySink : QueryResultSink
    {
        private readonly bool ignoreBnodes;
        private readonly string @namespace;
        private readonly string[] varNames;

        public List<NameValueCollection> bindings = new List<NameValueCollection>();

        public ClassQuerySink(bool ignoreBnodes, string @namespace, string[] varNames)
        {
            this.ignoreBnodes = ignoreBnodes;
            this.@namespace = @namespace;
            this.varNames = varNames;
        }

        public override bool Add(VariableBindings result)
        {
            var nvc = new NameValueCollection();

            foreach (string varName in varNames)
            {
                Resource resource = result[varName];
                if (!string.IsNullOrEmpty(resource.Uri))
                {
                    if (string.IsNullOrEmpty(@namespace) || resource.Uri.StartsWith(@namespace))
                    {
                        nvc[varName] = resource.Uri;
                    }
                }
                else if (resource is BNode && !ignoreBnodes)
                {
                    var bn = resource as BNode;
                    if (string.IsNullOrEmpty(@namespace) || bn.LocalName.StartsWith(@namespace))
                    {
                        nvc[varName] = bn.LocalName;
                    }
                }
            }
            bindings.Add(nvc);
            return true;
        }
    }
}