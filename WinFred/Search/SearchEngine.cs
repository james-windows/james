using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using James.Properties;
using James.ResultItems;
using System.IO;
using James.HelperClasses;

namespace James.Search
{
    public class SearchEngine
    {
        public delegate void ChangedBuildingIndexProgressEventHandler(object sender, ProgressChangedEventArgs e);

        private static SearchEngine _searchEngine;
        private static readonly object SingeltonLock = new object();
        private readonly SearchEngineWrapper.SearchEngineWrapper _searchEngineWrapper;
        private readonly Timer _timer;

        private SearchEngine()
        {
            string filePath = Config.Instance.ConfigFolderLocation + "\\files.txt";
            if (!File.Exists(filePath))
            {
                File.Create(filePath);
            }
            _searchEngineWrapper =
                new SearchEngineWrapper.SearchEngineWrapper(filePath, Config.Instance.MaxSearchResults);

            //Triggers index backup every 5 minutes
            _timer = new Timer(1000*60*5)
            {
                AutoReset = true,
                Enabled = true
            };
            _timer.Elapsed += Timer_Elapsed;
        }

        public static SearchEngine Instance
        {
            get
            {
                lock (SingeltonLock)
                {
                    return _searchEngine ?? (_searchEngine = new SearchEngine());
                }
            }
        }

        /// <summary>
        /// It's time for a new backup of the index to be saved
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
#if DEBUG
            Console.WriteLine(Resources.SearchEngine_SearchEngineBackup_Notification);
#endif
            _searchEngineWrapper.Save();
        }

        public event ChangedBuildingIndexProgressEventHandler ChangedBuildingIndexProgress;

        /// <summary>
        /// Starts to rebuild the index
        /// </summary>
        public void BuildIndex()
        {
            Config.Instance.Paths.Where(path => path.IsEnabled).ForEach(path => DeletePath(path.Location));
            var data = GetFilesToBeIndexed();
            WriteFilesToIndex(data);
        }

        /// <summary>
        /// Collects all items from every path parallel
        /// </summary>
        /// <returns></returns>
        private static List<ResultItem> GetFilesToBeIndexed()
        {
            var data = new List<ResultItem>();
            Parallel.ForEach(Config.Instance.Paths.Where(path => path.IsEnabled),
                currentPath => { data.AddRange(currentPath.GetItemsToBeIndexed()); });
            return data;
        }

        /// <summary>
        /// Adds providen ResultItems to the index
        /// </summary>
        /// <param name="data"></param>
        public void WriteFilesToIndex(IReadOnlyList<ResultItem> data)
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

        /// <summary>
        /// Notifies the GUI about the progress
        /// </summary>
        /// <param name="position"></param>
        /// <param name="total"></param>
        /// <param name="lastProgress"></param>
        private void ChangeProgress(int position, int total, ref int lastProgress)
        {
            if (lastProgress != CalcProgress(position, total))
            {
                lastProgress = CalcProgress(position, total);
                ChangedBuildingIndexProgress?.Invoke(this, new ProgressChangedEventArgs(lastProgress, null));
            }
        }

        private static int CalcProgress(int position, int total) => (int) (((double) position)/total*100);

        /// <summary>
        /// Wraps the Insert method of the SearchEngineWrapper
        /// </summary>
        /// <param name="file"></param>
        public void AddFile(ResultItem file) => _searchEngineWrapper.Insert(file.Subtitle, file.Priority);

        /// <summary>
        /// Wraps the Rename method of the SearchEngineWrapper
        /// </summary>
        /// <param name="oldPath"></param>
        /// <param name="newPath"></param>
        public void RenameFile(string oldPath, string newPath) => _searchEngineWrapper.Rename(oldPath, newPath);

        /// <summary>
        /// Wraps the Delete Path method of the SearchEngineWrapper
        /// </summary>
        /// <param name="path"></param>
        public void DeletePath(string path) => _searchEngineWrapper.Remove(path);

        /// <summary>
        /// Wraps the IncrementePriority method of the SearchEngineWrapper
        /// </summary>
        /// <param name="path"></param>
        /// <param name="priority"></param>
        public void IncrementPriority(string path, int priority = 5) => _searchEngineWrapper.AddPriority(path, priority);

        /// <summary>
        /// Other overload of the IncrementPriority method
        /// </summary>
        /// <param name="resultItem"></param>
        public void IncrementPriority(ResultItem resultItem) => IncrementPriority(resultItem.Subtitle);

        /// <summary>
        /// Wraps the Query method of the SearchEngineWrapper
        /// </summary>
        /// <param name="search"></param>
        public List<ResultItem> Query(string search)
        {
            if (search.Length < Config.Instance.StartSearchMinTextLength)
            {
                return new List<ResultItem>();
            }
#if DEBUG
            var tmp = DateTime.UtcNow;
            _searchEngineWrapper.Find(search);
            Console.WriteLine((DateTime.UtcNow - tmp).TotalMilliseconds);
#else
            _searchEngineWrapper.Find(search);
#endif
            return
                _searchEngineWrapper.searchResults.Select(
                    item => new SearchResultItem(item.path, item.priority))
                    .ToList()
                    .ConvertAll(input => (ResultItem) input);
        }
    }
}