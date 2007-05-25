using System;
using System.Collections.Generic;
using System.IO;
using System.Query;
using System.Text;
using System.Expressions;

namespace RdfMusic.IO
{
    public class FsIter
    {
        public static IEnumerable<DirectoryInfo> SubDirectories(DirectoryInfo di)
        {
            yield return di;
            foreach (DirectoryInfo directory in di.GetDirectories())
            {
                foreach (DirectoryInfo subDirectory in SubDirectories(directory))
                {
                    yield return subDirectory;
                }
            }
        }

        public static IEnumerable<FileInfo> AllFilesUnder(DirectoryInfo d)
        {
            foreach (DirectoryInfo dir in SubDirectories(d))
            {
                foreach (FileInfo fileInfo in dir.GetFiles())
                {
                    yield return fileInfo;
                }
            }
        }
        public static IList<FileInfo> MatchingFilesUnder(string extension, DirectoryInfo d)
        {
          var q = from f in AllFilesUnder(d)
                  where f.Extension == extension
                  select f;
          return q.ToList();
        }

        public static IEnumerable<FileInfo> MatchingFilesUnder(Func<FileInfo, bool> x, DirectoryInfo d)
        {
            foreach (FileInfo fileInfo in AllFilesUnder(d))
            {
                if (x(fileInfo))
                    yield return fileInfo;
            }
        }

        public static IEnumerable<FileInfo> FilesByExtension(string ext, DirectoryInfo d)
        {
            Func<FileInfo, bool> test = x=>x.Extension==ext;
            return Match(test, AllFilesUnder(d));
        }
        
        public static IEnumerable<IT> Match<IT>(Func<IT, bool> test, IEnumerable<IT> col)
        {
            foreach (IT it in col)
            {
                if(test(it))
                    yield return (it);
            }
        }
    }
}