using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using James.HelperClasses;
using James.ResultItems;

namespace James.Search
{
    internal class MyFileWatcher
    {
        private const int MaxMoveDelay = 50;
        private static readonly object SingeltonLock = new object();
        private static readonly object DeleteEventsLock = new object();

        private static MyFileWatcher _instance;
        private static readonly Queue<DeleteEvent> DeleteEvents = new Queue<DeleteEvent>();
        private readonly List<FileSystemWatcher> _fileSystemWatchers = new List<FileSystemWatcher>();
        private readonly List<Path> _paths = new List<Path>();

        private MyFileWatcher()
        {
            var paths = Config.Instance.Paths.ToList();
            paths.Where(path => path.IsEnabled).ForEach(AddPath);
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

        /// <summary>
        /// Handels if an event for file create got triggered
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void File_Created(object sender, FileSystemEventArgs e)
        {
            var newName = PathHelper.GetFilename(e.Name);
            var watcher = sender as FileSystemWatcher;
            var currentPath = _paths.First(path => path.Location == watcher?.Path);
            var oldPath = GetOldPathIfExists(newName);
            if (oldPath != null)
            {
                Console.WriteLine($"moved {e.FullPath} | old: {oldPath}");
                SearchEngine.Instance.RenameFile(oldPath, e.FullPath);
            }
            else
            {
                Console.WriteLine($"created {e.FullPath}");
                int priority = GetPathPriority(currentPath, e.FullPath);
                if (priority >= 0)
                {
                    SearchEngine.Instance.AddFile(new SearchResultItem {Subtitle = e.FullPath, Title = e.Name,Priority = priority});
                }
            }
            if (Directory.Exists(e.FullPath))
            {
                SearchEngine.Instance.WriteFilesToIndex(
                    currentPath.GetItemsToBeIndexed(e.FullPath.Replace(currentPath.Location, "")).ToList());
            }
        }

        /// <summary>
        /// Searches if a folder with the same name got moved
        /// </summary>
        /// <param name="newName"></param>
        /// <returns></returns>
        private static string GetOldPathIfExists(string newName)
        {
            string oldPath = null;
            lock (DeleteEventsLock)
            {
                if (DeleteEvents.Count > 0 && DeleteEvents.Peek().Name == newName)
                {
                    oldPath = DeleteEvents.Dequeue().Path;
                }
            }
            return oldPath;
        }

        /// <summary>
        /// Handels if an event for file delete got triggered
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void File_Deleted(object sender, FileSystemEventArgs e)
        {
            lock (DeleteEventsLock)
            {
                DeleteEvents.Enqueue(new DeleteEvent
                {
                    Date = DateTime.UtcNow,
                    Path = e.FullPath,
                    Name = e.FullPath.Split('\\').Last()
                });
            }
            Task.Run(() =>
            {
                Thread.Sleep(MaxMoveDelay);
                lock (DeleteEventsLock)
                {
                    DeleteFileLazy();
                }
            });
            Console.WriteLine($"marking for deletion: {e.FullPath}");
        }

        /// <summary>
        /// Deletes all folders which got marked as lazy delete
        /// </summary>
        private static void DeleteFileLazy()
        {
            while (DeleteEvents.Count > 0 && (DateTime.UtcNow - DeleteEvents.Peek().Date).TotalMilliseconds > MaxMoveDelay)
            {
                var path = DeleteEvents.Dequeue().Path;
                Console.WriteLine($"lazy deleted {path}");
                SearchEngine.Instance.DeletePath(path);
            }
        }

        /// <summary>
        /// Handels if an event for file rename got triggered
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void File_Renamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"renamed {e.OldFullPath} to {e.FullPath}");
            var watcher = sender as FileSystemWatcher;
            var currentPath = _paths.First(path => path.Location == watcher?.Path);
            int priority = GetPathPriority(currentPath, e.FullPath);
            if (priority >= 0)
            {
                SearchEngine.Instance.RenameFile(e.OldFullPath, e.FullPath);
            }
        }

        /// <summary>
        ///     Destroys the filewatcher listening to that path as well removes it from the path list.
        /// </summary>
        /// <param name="path"></param>
        public void RemovePath(Path path)
        {
            _paths.Remove(path);
            var watcher = _fileSystemWatchers.First(item => item.Path == path.Location);
            watcher.Dispose();
            _fileSystemWatchers.Remove(watcher);
        }

        /// <summary>
        ///     Adds the given path to the list as well as created a new filewatcher for this path.
        /// </summary>
        /// <param name="path"></param>
        public void AddPath(Path path)
        {
            _paths.Add(path);
            _fileSystemWatchers.Add(CreateFileSystemWatcher(path));
        }

        /// <summary>
        ///     Creates a FileSystemWatcher for the given path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private FileSystemWatcher CreateFileSystemWatcher(Path path)
        {
            var watcher = new FileSystemWatcher(path.Location)
            {
                IncludeSubdirectories = true,
                EnableRaisingEvents = true
            };
            watcher.Created += File_Created;
            watcher.Deleted += File_Deleted;
            watcher.Renamed += File_Renamed;
            return watcher;
        }

        /// <summary>
        ///     Calculates the priority of a given path
        /// </summary>
        /// <param name="searchScope"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private int GetPathPriority(Path searchScope, string path)
        {
            if (Directory.Exists(path) && searchScope.IndexFolders)
            {
                return searchScope.Priority + Config.Instance.DefaultFolderPriority;
            }
            return searchScope.GetPathPriority(path);
        }
    }
}