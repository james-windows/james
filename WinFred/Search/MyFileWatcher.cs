using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WinFred.Search
{
    class MyFileWatcher
    {
        Path[] paths;
        LinkedList<FileSystemWatcher> fileSystemWatchers;
        public MyFileWatcher()
        {
            paths = Config.GetInstance().Paths.ToArray();
            fileSystemWatchers = new LinkedList<FileSystemWatcher>();

            for (int i = 0; i < paths.Length; i++)
            {
                FileSystemWatcher tmpWatcher = new FileSystemWatcher(paths[i].Location);
                tmpWatcher.IncludeSubdirectories = true;
                tmpWatcher.Created += File_Created;
                tmpWatcher.Deleted += File_Deleted;
                tmpWatcher.Renamed += File_Renamed;
                tmpWatcher.Changed += File_Changed;
                fileSystemWatchers.AddLast(tmpWatcher);
                tmpWatcher.EnableRaisingEvents = true;
            }
        }

        private void File_Created(object sender, FileSystemEventArgs e)
        {
            FileSystemWatcher watcher = sender as FileSystemWatcher;//TODO find better solution than duplication
            Path currentPath = paths.First(path => path.Location == watcher.Path);
            int priority = currentPath.GetFilePriority(e.FullPath);
            if (priority >= 0)
            {
                SearchEngine.GetInstance().AddFile(new Data(e.FullPath, priority));
            }
        }

        private void File_Deleted(object sender, FileSystemEventArgs e)
        {
            FileSystemWatcher watcher = sender as FileSystemWatcher;
            Path currentPath = paths.First(path => path.Location == watcher.Path);
            int priority = currentPath.GetFilePriority(e.FullPath);
            if (priority >= 0)
            {
                SearchEngine.GetInstance().DeleteFile(e.FullPath);
            }
        }

        private void File_Renamed(object sender, RenamedEventArgs e)
        {
            FileSystemWatcher watcher = sender as FileSystemWatcher;
            Path currentPath = paths.First(path => path.Location == watcher.Path);
            int priority = currentPath.GetFilePriority(e.FullPath);
            if (priority >= 0)
            {
                SearchEngine.GetInstance().RenameFile(e.FullPath);
            }
        }

        private void File_Changed(object sender, FileSystemEventArgs e)
        {
            FileSystemWatcher watcher = sender as FileSystemWatcher;
            Path currentPath = paths.First(path => path.Location == watcher.Path);
            int priority = currentPath.GetFilePriority(e.FullPath);
            if (priority >= 0)
            {
                SearchEngine.GetInstance().IncrementPriority(e.FullPath);
            }
        }
    }
}