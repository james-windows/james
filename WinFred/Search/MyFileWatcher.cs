using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace James.Search
{
    internal class MyFileWatcher
    {
        private static readonly object SingeltonLock = new object();

        private static MyFileWatcher _instance;
        private readonly Path[] _paths;

        private MyFileWatcher()
        {
            _paths = Config.Instance.Paths.ToArray();
            var fileSystemWatchers = new LinkedList<FileSystemWatcher>();
            foreach (var path in _paths)
            {
                var watcher = new FileSystemWatcher(path.Location) {IncludeSubdirectories = true};
                watcher.Created += File_Created;
                watcher.Deleted += File_Deleted;
                watcher.Renamed += File_Renamed;
                watcher.Changed += File_Changed;
                fileSystemWatchers.AddLast(watcher);
                watcher.EnableRaisingEvents = true;
            }
        }

        public static MyFileWatcher Instance
        {
            get
            { 
                lock (SingeltonLock)
                {
                    return _instance ?? (_instance = new MyFileWatcher());
                }
            }
        }

        private void File_Created(object sender, FileSystemEventArgs e)
        {
            var watcher = sender as FileSystemWatcher;
            var currentPath = _paths.First(path => path.Location == watcher?.Path);
            var priority = currentPath.GetPathPriority(e.FullPath);
            if (priority >= 0)
            {
                SearchEngine.GetInstance().AddFile(new SearchResult {Path = e.FullPath, Priority = priority});
            }
        }

        private static void File_Deleted(object sender, FileSystemEventArgs e)
        {
            SearchEngine.GetInstance().DeleteFile(e.FullPath);
        }

        private void File_Renamed(object sender, RenamedEventArgs e)
        {
            var watcher = sender as FileSystemWatcher;
            var currentPath = _paths.First(path => path.Location == watcher?.Path);
            var priority = currentPath.GetPathPriority(e.FullPath);
            if (priority >= 0)
            {
                SearchEngine.GetInstance().RenameFile(e.OldFullPath, e.FullPath);
            }
        }

        private static void File_Changed(object sender, FileSystemEventArgs e)
        {
            SearchEngine.GetInstance().IncrementPriority(e.FullPath);
        }
    }
}