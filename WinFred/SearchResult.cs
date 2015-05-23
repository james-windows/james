using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                return null;
                if (File.Exists(Path))
                {
                    DateTime tmp = DateTime.Now;
                    Icon ico = System.Drawing.Icon.ExtractAssociatedIcon(Path);
                    ImageSource imageSource;
                    if (ico != null)
                        imageSource = HelperClass.ToImageSource(ico);
                    Debug.WriteLine("Get Image: " + (DateTime.Now - tmp).TotalMilliseconds);
                    return Icon;
                }
                else
                {
                    
                }
                
                return null;
            }
        }

        public void Open()
        {
            Process.Start(Path);
            SearchEngine.GetInstance().IncrementPriority(this);
        }

        public void OpenFolder()
        {
            if (Directory.Exists(Path))
            {
                Open();
            }
            else
            {
                Process.Start(Path.Substring(0, Path.LastIndexOf('\\')));
                SearchEngine.GetInstance().IncrementPriority(this);
            }
        }
    }
}