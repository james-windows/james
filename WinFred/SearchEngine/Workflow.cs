using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WinFred.Annotations;

namespace WinFred
{
    public class Workflow : INotifyPropertyChanged
    {
        public Workflow()
        {

        }

        public bool ShowProcessWindow { get; set; }
        private string _name;
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

        public Workflow(String name, String keyword, bool showProcessWindow = false, bool isEnabled = true)
        {
            Name = name;
            Keyword = keyword;
            this.ShowProcessWindow = showProcessWindow;
            this.IsEnabled = isEnabled;
        }

        /// <summary>
        /// Generates an Process and returns it.
        /// </summary>
        /// <returns>The Process of the Workflow</returns>
        public Process Execute(string parameter)
        {
            if (ProgramName.Trim() == "")
            {
                throw new ArgumentException("The Command property must not be null!");
            }
            Process proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = this.ProgramName,
                    Arguments = this.Arguments + " " + parameter.Trim(),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = !ShowProcessWindow
                }
            };
            return proc;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
