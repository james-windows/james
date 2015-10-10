using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace James.Search
{
    internal class SearchEngine
    {
        public delegate void ChangedBuildingIndexProgressEventHandler(object sender, ProgressChangedEventArgs e);
        
        private static SearchEngine _searchEngine;
        private static readonly object SingeltonLock = new object();
        private readonly SearchEngineWrapper.SearchEngineWrapper _searchEngineWrapper;

        private readonly Timer timer;
        private SearchEngine()
        {
            _searchEngineWrapper = new SearchEngineWrapper.SearchEngineWrapper(Config.GetInstance().ConfigFolderLocation + "\\files.txt");
            timer = new Timer(1000 * 60 * 5)
            {
                AutoReset = true,
                Enabled = true
            };
            timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {

#if DEBUG
            Console.WriteLine("Triggerd Event for backup SearchEngine");
#endif
            _searchEngineWrapper.Save();
        }

        public event ChangedBuildingIndexProgressEventHandler ChangedBuildingIndexProgress;

        public static SearchEngine GetInstance()
        {
            lock (SingeltonLock)
            {
                return _searchEngine ?? (_searchEngine = new SearchEngine());
            }
        }

        public void BuildIndex()
        {
            var data = GetFilesToBeIndexed();
            WriteFilesToIndex(data);
        }

        private static List<SearchResult> GetFilesToBeIndexed()
        {
            var data = new List<SearchResult>();
            Parallel.ForEach(Config.GetInstance().Paths.Where(path => path.IsEnabled), currentPath =>
            {
                data.AddRange(currentPath.GetItemsToBeIndexed());
            });
            return data;
        }

        private void WriteFilesToIndex(IReadOnlyList<SearchResult> data)
        {
            var lastProgress = -1;
            for (var i = 0; i < data.Count; i++)
            {
                ChangeProgress(i, data.Count, ref lastProgress);
                AddFile(data[i]);
            }
            ChangedBuildingIndexProgress?.Invoke(this, new ProgressChangedEventArgs(100, null));
            _searchEngineWrapper.Save();
        }

        private void ChangeProgress(int position, int total, ref int lastProgress)
        {
            if (lastProgress != CalcProgress(position, total))
            {
                lastProgress = CalcProgress(position, total);
                ChangedBuildingIndexProgress?.Invoke(this, new ProgressChangedEventArgs(lastProgress, null));
            }
        }

        private static int CalcProgress(int position, int total) => (int) (((double) position) / total * 100);

        public void AddFile(SearchResult file)
        {
            _searchEngineWrapper.Insert(file.Path, file.Priority);
        }

        internal void RenameFile(string oldPath, string newPath)
        {
            _searchEngineWrapper.Rename(oldPath, newPath);
        }

        public List<SearchResult> Query(string search)
        {
            if (search.Length < Config.GetInstance().StartSearchMinTextLength)
            {
                return new List<SearchResult>();
            }
#if DEBUG
            DateTime tmp = DateTime.Now;
            _searchEngineWrapper.Find(search);
            Console.WriteLine((DateTime.Now - tmp).TotalMilliseconds);
#else
            _searchEngineWrapper.Find(search);
#endif
            return _searchEngineWrapper.searchResults.Select(item => new SearchResult() {Path = item.path + item.name}).ToList();
        }

        public void DeleteFile(string path)
        {
            _searchEngineWrapper.Remove(path);
        }

        public void IncrementPriority(SearchResult result)
        {
           IncrementPriority(result.Path);
        }
        
        public void IncrementPriority(string path)
        {
            _searchEngineWrapper.AddPriority(path, 5);
        }
    }
}