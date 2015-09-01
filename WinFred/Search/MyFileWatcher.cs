﻿using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WinFred.Search
{
    internal class MyFileWatcher
    {
        private static readonly object SingeltonLock = new object();
        //private List<object> _list;

        private static MyFileWatcher _watcher;
        private readonly Path[] _paths;

        private MyFileWatcher()
        {
            _paths = Config.GetInstance().Paths.ToArray();
            var fileSystemWatchers = new LinkedList<FileSystemWatcher>();

            foreach (var path in _paths)
            {
                var watcher = new FileSystemWatcher(path.Location);
                watcher.IncludeSubdirectories = true;
                watcher.Created += File_Created;
                watcher.Deleted += File_Deleted;
                watcher.Renamed += File_Renamed;
                watcher.Changed += File_Changed;
                fileSystemWatchers.AddLast(watcher);
                watcher.EnableRaisingEvents = true;
                //_list.Add(watcher);
            }
        }

        public static MyFileWatcher GetInstance()
        {
            lock (SingeltonLock)
            {
                return _watcher ?? (_watcher = new MyFileWatcher());
            }
        }

        private void File_Created(object sender, FileSystemEventArgs e)
        {
            var watcher = sender as FileSystemWatcher;
            var currentPath = _paths.First(path => path.Location == watcher?.Path);
            var priority = currentPath.GetFilePriority(e.FullPath);
            if (priority >= 0)
            {
                SearchEngine.GetInstance().AddFile(new Data(e.FullPath, priority));
            }
        }

        private void File_Deleted(object sender, FileSystemEventArgs e)
        {
            SearchEngine.GetInstance().DeleteFile(e.FullPath);
        }

        private void File_Renamed(object sender, RenamedEventArgs e)
        {
            var watcher = sender as FileSystemWatcher;
            var currentPath = _paths.First(path => path.Location == watcher?.Path);
            var priority = currentPath.GetFilePriority(e.FullPath);
            if (priority >= 0)
            {
                SearchEngine.GetInstance().RenameFile(e.OldFullPath, e.FullPath);
            }
        }

        private void File_Changed(object sender, FileSystemEventArgs e)
        {
            SearchEngine.GetInstance().IncrementPriority(e.FullPath);
        }
    }
}