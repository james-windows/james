using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Media;
using WinFred.Search;

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
                    Icon ico = System.Drawing.Icon.ExtractAssociatedIcon(Path);
                    if (ico == null)
                        return null;
                    ImageSource imageSource = null;
                    if (ico != null)
                        imageSource = HelperClass.ToImageSource(ico);
                    return imageSource;
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