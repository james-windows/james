using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using WinFred.Annotations;

namespace WinFred.Search
{
    public class Path : INotifyPropertyChanged
    {
        private bool _isEnabled = true;

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
                OnPropertyChanged();
            }
        }

        public string Location { get; set; }
        public int Priority { get; set; }
        public List<FileExtension> FileExtensions { get; set; }
        public int FolderPriority { get; set; } = 0;
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString() => Location + " :" + Priority;

        public int GetFilePriority(string filePath)
        {
            var fileExtension = CalculatePriorityByFileExtensions(filePath, FileExtensions);
            var defaultFileExtension = CalculatePriorityByFileExtensions(filePath,
                Config.GetInstance().DefaultFileExtensions);

            if (fileExtension < -1 && defaultFileExtension < -1)
            {
                return -1;
            }
            return Math.Max(fileExtension, defaultFileExtension) + Priority;
        }

        private static int CalculatePriorityByFileExtensions(string filePath, List<FileExtension> fileExtensions)
        {
            var indexOfSearchedItem = fileExtensions.BinarySearch(new FileExtension(filePath.Split('.').Last(), 0));
            var priority = 0;
            if (indexOfSearchedItem < 0)
            {
                priority = -2;
            }
            else if (indexOfSearchedItem >= 0)
            {
                priority = fileExtensions[indexOfSearchedItem].Priority;
            }
            return priority;
        }
    }
}