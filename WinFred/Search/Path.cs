using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using James.Annotations;
using James.HelperClasses;
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
            //TODO if /Users/Moser is path and Moser is excluded even the files shouldn't be included
            var data = new List<ResultItem>();
            try
            {
                data.AddRange(GetItemsInCurrentScope(Location + currentPath));
                foreach (var directory in GetDirectories(Location + currentPath))
                {
                    bool folderExcluded = Config.Instance.ExcludedFolders.Any(s => Regex.IsMatch(directory, s));
                    bool folderConfigurationOverriden = Config.Instance.Paths.Any(path => path.Location == directory && currentPath != "");
                    if (!folderExcluded && !folderConfigurationOverriden)
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
        /// Returns all not hidden directories in path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private IEnumerable<string> GetDirectories(string path)
        {
            return new DirectoryInfo(path).GetDirectories()
                .Where(dir => !dir.Attributes.HasFlag(FileAttributes.Hidden))
                .Select(dir => dir.FullName).Where(dir => dir != "");
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
                Title = PathHelper.GetFilename(path),
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
            return new DirectoryInfo(currentPath).GetFiles()
                .Where(f => !f.Attributes.HasFlag(FileAttributes.Hidden))
                .Select(filePath => GetItemIfItShouldBeIndexed(filePath.FullName)).Where(f => f != null);
        }

        /// <summary>
        /// Returns item if priority > 0
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
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
            if (priority < 0 || Config.Instance.ExcludedFolders.Any(s => Regex.IsMatch(path, s)))
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
            return priority + Math.Max(0, 10 - (int)Math.Sqrt(PathHelper.GetFilename(filePath).Length));
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