using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFred
{
    public class SearchConfig 
    {
        public List<Path> Paths { get; set; }

        public SearchConfig()
        {
            Paths = new List<Path>();
        }
    }

    public class Path
    {
        public string Location { get; set; }
        public List<string> IncludedExtensions { get; set; }
        public List<string> ExcludedExtensions { get; set; }
        public List<string> ExcludedFolders { get; set; }

        public Path()
        {
            IncludedExtensions = new List<string>();
            ExcludedExtensions = new List<string>();
            ExcludedFolders = new List<string>();
        }
    }

}
