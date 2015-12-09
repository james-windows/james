using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using James.Annotations;

namespace James.Search
{
    public class Path : INotifyPropertyChanged
    {
        private bool _isDefaultConfigurationEnabled = true;
        private bool _isEnabled = true;

        public Path()
        {
            FileExtensions = new List<FileExtension>();
        }

        public string Location { get; set; }
        public int Priority { get; set; }
        public bool IndexFolders { get; set; } = true;
        public List<FileExtension> FileExtensions { get; set; }

        public bool IsDefaultConfigurationEnabled
        {
            get { return _isDefaultConfigurationEnabled; }
            set
            {
                _isDefaultConfigurationEnabled = value;
                OnPropertyChanged(nameof(IsDefaultConfigurationEnabled));
            }
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString() => Location + " :" + Priority;

        /// <summary>
        ///     recursively goes through the folder and returns all items which sould be indexed.
        /// </summary>
        /// <param name="currentPath"></param>
        /// <returns></returns>
        public IEnumerable<SearchResult> GetItemsToBeIndexed(string currentPath = "")
        {
            var data = new List<SearchResult>();
            try
            {
                data.AddRange(GetItemsInCurrentScope(Location + currentPath));
                foreach (var directory in Directory.GetDirectories(Location + currentPath))
                {
                    if (Config.Instance.ExcludedFolders.Count(s => directory.Contains("\\" + s)) == 0)
                    {
                        data.AddRange(GetItemsToBeIndexed(directory.Replace(Location, "")));
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
            }
            if (IndexFolders && data.Count > 0)
            {
                data.Add(new SearchResult
                {
                    Path = Location + currentPath,
                    Priority = Config.Instance.DefaultFolderPriority + Priority
                });
            }
            return data;
        }

        private IEnumerable<SearchResult> GetItemsInCurrentScope(string currentPath)
        {
            return
                Directory.GetFiles(currentPath).Select(GetItemIfItShouldBeIndexed).Where(file => file != null).ToList();
        }

        private SearchResult GetItemIfItShouldBeIndexed(string filePath)
        {
            var priority = GetPathPriority(filePath);
            return GetPathPriority(filePath) > 0 ? new SearchResult {Path = filePath, Priority = priority} : null;
        }

        /// <summary>
        ///     Calculates the priority of a given path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public int GetPathPriority(string path)
        {
            if (Directory.Exists(path))
            {
                if (IndexFolders)
                {
                    return Priority + Config.Instance.DefaultFolderPriority;
                }
                return -1;
            }
            var priority = CalculatePriorityByFileExtensions(path);
            if (priority < 0)
            {
                return -1;
            }
            return priority + Priority;
        }

        /// <summary>
        ///     Calculates the priority for a given file. If priority smaller than 0 it shouldn't be indexed.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>priority</returns>
        private int CalculatePriorityByFileExtensions(string filePath)
        {
            var fileExtension = filePath.Split('.').Last();
            var priority = GetPriorityByGivenFileExtensions(fileExtension, FileExtensions);
            if (priority == int.MinValue && IsDefaultConfigurationEnabled)
            {
                priority = GetPriorityByGivenFileExtensions(fileExtension, Config.Instance.DefaultFileExtensions);
            }
            return priority;
        }

        /// <summary>
        ///     Returns the Priority of the file using the given fileExtensions list. If now suitable FileExtension was found
        ///     int.MinValue will be returned.
        /// </summary>
        /// <param name="fileExtension">FileExtension of the File</param>
        /// <param name="fileExtnesions">List of FileExtensions</param>
        /// <returns></returns>
        private static int GetPriorityByGivenFileExtensions(string fileExtension, List<FileExtension> fileExtnesions)
        {
            var indexOfSearchedItem = fileExtnesions.BinarySearch(new FileExtension(fileExtension, 0));
            return indexOfSearchedItem < 0 ? int.MinValue : fileExtnesions[indexOfSearchedItem].Priority;
        }
    }
}