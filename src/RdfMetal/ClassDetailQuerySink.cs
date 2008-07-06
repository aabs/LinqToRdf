using System;
using System.Collections.Generic;
using System.Diagnostics;
using SemWeb;

namespace RdfMetal
{
    public class ClassDetailQuerySink : StatementSink
    {
        public List<Uri> ls = new List<Uri>();

        #region StatementSink Members

        public bool Add(Statement s)
        {
            string output = string.Format("{0} {1} {2}", s.Subject, s.Predicate, s.Object);
            Debug.WriteLine(output);
            return true;
        }

        #endregion
    }
}