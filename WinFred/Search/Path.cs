﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using WinFred.Annotations;

namespace WinFred
{
    public class Path : INotifyPropertyChanged
    {
        private bool _isEnabled = true;

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

        public Path()
        {
            FileExtensions = new List<FileExtension>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// For debugging
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Location + " :" + Priority;
        }

        public int GetFilePriority(String filePath)
        {
            int FileExtension = CalculatePriorityByFileExtensions(filePath, this.FileExtensions);
            int DefaultFileExtension = CalculatePriorityByFileExtensions(filePath, Config.GetInstance().DefaultFileExtensions);

            return Math.Max(FileExtension, DefaultFileExtension) + this.Priority;
        }

        private int CalculatePriorityByFileExtensions(string filePath, List<FileExtension> fileExtensions)
        {
            int wasFound = fileExtensions.BinarySearch(new FileExtension(filePath.Split('.').Last(), 0));
            int priority = wasFound;
            if (priority >= 0)
            {
                priority = fileExtensions[priority].Priority;
            }
            return priority;
        }
    }
}
