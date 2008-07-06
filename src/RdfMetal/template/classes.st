using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LinqToRdf;
using System.Data.Linq;
$refs:namespaceref()$

namespace $opts.dotnetNamespace$
{
[assembly: Ontology(
    BaseUri = "$uri$",
    Name = "$handle$",
    Prefix = "$handle$",
    UrlOfOntology = "$uri$")]


    public partial class $handle$DataContext : RdfDataContext
    {
        public $handle$DataContext(TripleStore store) : base(store)
        {
        }
        public $handle$DataContext(string store) : base(new TripleStore(store))
        {
        }

		$classes:query()$
    }

$classes:class()$

}