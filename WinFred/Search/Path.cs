using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using James.Annotations;
using James.ResultItems;

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
        public IEnumerable<ResultItem> GetItemsToBeIndexed(string currentPath = "")
        {
            var data = new List<ResultItem>();
            try
            {
                data.AddRange(GetItemsInCurrentScope(Location + currentPath));
                foreach (var directory in Directory.GetDirectories(Location + currentPath))
                {
                    if (Config.Instance.ExcludedFolders.All(s => !directory.Contains("\\" + s)) && Config.Instance.Paths.All(path => path.Location != directory))
                    {
                        data.AddRange(GetItemsToBeIndexed(directory.Replace(Location, "")));
                    }
                }
            }
            catch (UnauthorizedAccessException){}
            if (IndexFolders && data.Count > 0)
            {
                data.Add(GenerateFolder(Location + currentPath));
            }
            return data;
        }

        /// <summary>
        /// Generates an ResultItem for the folder
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private ResultItem GenerateFolder(string path)
        {
            return new SearchResultItem
            {
                Subtitle = path,
                Title = path.Split('\\').Last(),
                Priority = Config.Instance.DefaultFolderPriority + Priority
            };
        }

        /// <summary>
        /// Returns all ResultItems of files which should be indexed
        /// </summary>
        /// <param name="currentPath"></param>
        /// <returns></returns>
        private IEnumerable<ResultItem> GetItemsInCurrentScope(string currentPath)
        {
            return
                Directory.GetFiles(currentPath).Select(GetItemIfItShouldBeIndexed).Where(file => file != null).ToList();
        }

        private ResultItem GetItemIfItShouldBeIndexed(string filePath)
        {
            var priority = GetPathPriority(filePath);
            return priority > 0 ? new SearchResultItem(filePath, priority) : null;
        }

        /// <summary>
        ///     Calculates the priority of a given file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public int GetPathPriority(string path)
        {
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
        /// <param name="fileExtensions">List of FileExtensions</param>
        /// <returns></returns>
        private static int GetPriorityByGivenFileExtensions(string fileExtension, List<FileExtension> fileExtensions)
        {
            var indexOfSearchedItem = fileExtensions.FindIndex(x => x.Extension == fileExtension);
            return indexOfSearchedItem < 0 ? int.MinValue : fileExtensions[indexOfSearchedItem].Priority;
        }
    }
}