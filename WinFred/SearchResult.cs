using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;

namespace WinFred
{
    public class SearchResult
    {
        public string Id { get; set; }
        public int Priority { get; set; }
        public string Path { get; set; }

        public string Filename
        {
            get
            {
                return Path.Substring(Path.LastIndexOf('\\') + 1);
            }
        }

        public ImageSource Icon
        {
            get
            {
                if (File.Exists(Path))
                {
                    Icon ico = System.Drawing.Icon.ExtractAssociatedIcon(Path);
                    if (ico != null) return HelperClass.ToImageSource(ico);
                }
                else
                {
                    
                }
                
                return null;
            }
        }

        public void Open()
        {
            System.Diagnostics.Process.Start(Path);
            SearchEngine.GetInstance().IncrementPriority(this);
        }
    }
}