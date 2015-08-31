using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using WinFred.Annotations;

namespace WinFred
{
    public class Workflow : INotifyPropertyChanged
    {
        private bool _isEnabled = true;
        private string _name;

        public Workflow()
        {
        }

        public Workflow(string name, string keyword, bool showProcessWindow = false, bool isEnabled = true)
        {
            Name = name;
            Keyword = keyword;
            ShowProcessWindow = showProcessWindow;
            IsEnabled = isEnabled;
        }

        public bool ShowProcessWindow { get; set; }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Keyword { get; set; }
        public string ProgramName { get; set; }
        public string Arguments { get; set; }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Generates an Process and returns it.
        /// </summary>
        /// <returns>The Process of the Workflow</returns>
        public Process Execute(string parameter)
        {
            if (ProgramName.Trim() == "")
            {
                throw new ArgumentException("The Command property must not be null!");
            }
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = ProgramName,
                    Arguments = Arguments + " " + parameter.Trim(),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = !ShowProcessWindow
                }
            };
            return proc;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}