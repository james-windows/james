using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace James.HelperClasses
{
    public static class PathHelper
    {
        public static string GetFilename(string path)
        {
            return path.Split('\\').Last();
        }

        public static string GetFoldername(string path)
        {
            return path.Substring(0, path.LastIndexOf('\\'));
        }
    }
}
