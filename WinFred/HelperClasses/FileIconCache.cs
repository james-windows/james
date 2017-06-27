using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;

namespace James.HelperClasses
{
    public class FileIconCache
    {
        private FileIconCache()
        {
            //FolderIcon = IconHelper.GetIcon("folder.ico");
            FolderIcon = null;
        }

        [JsonIgnore]
        public BitmapImage FolderIcon { get; }
        #region singleton

        private static FileIconCache _instance;
        private static readonly object SingeltonLock = new object();

        public static FileIconCache Instance
        {
            get
            {
                lock (SingeltonLock)
                {
                    return _instance ?? (_instance = new FileIconCache());
                }
            }
        }

        #endregion

        readonly Dictionary<string, ImageSource> _cache = new Dictionary<string, ImageSource>();
        private static readonly string[] CacheByPath = {"exe", "lnk"};

        public int CachedIconsCount => _cache.Count;

        public ImageSource GetFileIcon(string path)
        {
            string fileExtension = PathHelper.GetFileExtension(path);
            if (CacheByPath.Count(s => s == fileExtension) == 1)
            {
                if (_cache.ContainsKey(path)) return _cache[path];
                return File.Exists(path)? _cache[path] = GetFileIconByPath(path): null;
            }
            if (fileExtension != null && _cache.ContainsKey(fileExtension))
            {
                return _cache[fileExtension];
            }
            if (Directory.Exists(path))
            {
                return FolderIcon;
            }
            if (!File.Exists(path))
            {
                return null;
            }

            return _cache[fileExtension] = GetFileIconByPath(path);
        }

        public void LoadAllIcons(string[] paths)
        {
            foreach (var path in paths)
            {
                GetFileIcon(path);
            }
        }

        private ImageSource GetFileIconByPath(string path)
        {
            var tmp = System.Drawing.Icon.ExtractAssociatedIcon(path).ToImageSource();
            tmp.Freeze();
            return tmp;
        }
    }
}
