using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace WinFred
{
    public class SearchResult
    {
        public string Path { get; set; }

        public string Filename
        {
            get
            {
                return Path.Substring(Path.LastIndexOf('\\') + 1);
            }
        }
    }
}