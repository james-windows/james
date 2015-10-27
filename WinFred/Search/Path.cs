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
        private bool _isEnabled = true;
        private bool _isDefaultConfigurationEnabled = true;

        public bool IsDefaultConfigurationEnabled
        {
            get { return _isDefaultConfigurationEnabled; }
            set
            {
                _isDefaultConfigurationEnabled = value;
                OnPropertyChanged(nameof(IsDefaultConfigurationEnabled));
            }
        }

        public Path()
        {
            FileExtensions = new List<FileExtension>();
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

        public string Location { get; set; }
        public int Priority { get; set; }
        public bool IndexFolders { get; set; } = true;
        public List<FileExtension> FileExtensions { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString() => Location + " :" + Priority;

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
            var data = new List<SearchResult>();
            foreach (var filePath in Directory.GetFiles(currentPath))
            {
                data.AddRange(GetFileIfItShouldBeIndexed(filePath));
            }
            return data;
        }

        private IEnumerable<SearchResult> GetFileIfItShouldBeIndexed(string filePath)
        {
            var data = new List<SearchResult>();
            var priority = GetFilePriority(filePath);
            if (priority > 0)
            {
                data.Add(new SearchResult {Path = filePath, Priority = priority});
            }
            return data;
        }

        public int GetFilePriority(string filePath)
        {
            if (Directory.Exists(filePath))
            {
                if (this.IndexFolders)
                {
                    return Priority + Config.Instance.DefaultFolderPriority;
                }
                return -1;
            }
            var priority = CalculatePriorityByFileExtensions(filePath, Config.Instance.DefaultFileExtensions);
            if (priority == -1)
            {
                return -1;
            }
            return priority + Priority;
        }

        private int CalculatePriorityByFileExtensions(string filePath, List<FileExtension> defaultFileExtensions)
        {
            var indexOfSearchedItem = FileExtensions.BinarySearch(new FileExtension(filePath.Split('.').Last(), 0));
            if (indexOfSearchedItem >= 0)
            {
                return FileExtensions[indexOfSearchedItem].Priority;
            }
            if (IsDefaultConfigurationEnabled)
            {
                indexOfSearchedItem = defaultFileExtensions.BinarySearch(new FileExtension(filePath.Split('.').Last(), 0));
                if (indexOfSearchedItem >= 0)
                {
                    return defaultFileExtensions[indexOfSearchedItem].Priority;
                }
            }
            return -1;
        }
    }
}