using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Media;
using James.HelperClasses;
using James.Search;
using James.Workflows.Interfaces;
using James.Workflows.Triggers;

namespace James
{
    public class SearchResult : IComparable<SearchResult>
    {
        public string Id { get; set; }
        public int Priority { get; set; }
        private string _path;
        public string Filename { get; set; }
        public ImageSource Icon => (Config.GetInstance().DisplayFileIcons) ? GeneralHelper.GetIcon(Path) : null;
        public int CompareTo(SearchResult other) => Priority - other.Priority;
        public BasicTrigger WorkflowTrigger { get; set; }

        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                _path = value;
                Filename = value.Substring(Path.LastIndexOf('\\') + 1);
            }
        }

        public void Open()
        {
            if (WorkflowTrigger != null)
            {
                WorkflowTrigger.TriggerRunables();
            }
            else
            {
                Process.Start(Path);
                SearchEngine.GetInstance().IncrementPriority(this);
            }
        }

        public void OpenFolder()
        {
            if (WorkflowTrigger != null)
            {
                WorkflowTrigger.TriggerRunables();
            }
            else if (Directory.Exists(Path))
            {
                Open();
            }
            else
            {
                Process.Start(Path.Substring(0, Path.LastIndexOf('\\')));
                SearchEngine.GetInstance().IncrementPriority(this);
            }
        }
    }
}