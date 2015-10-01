using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using James.Annotations;

namespace James.Search
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

        public int GetFilePriority(string filePath)
        {
            var priority = CalculatePriorityByFileExtensions(filePath, Config.GetInstance().DefaultFileExtensions);
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
            indexOfSearchedItem = defaultFileExtensions.BinarySearch(new FileExtension(filePath.Split('.').Last(), 0));
            if (indexOfSearchedItem >= 0)
            {
                return defaultFileExtensions[indexOfSearchedItem].Priority;
            }
            return -1;
        }
    }
}