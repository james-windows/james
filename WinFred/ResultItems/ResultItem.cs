using System;
using System.Windows.Input;
using System.Windows.Media;

namespace James.ResultItems
{
    public abstract class ResultItem : IComparable<ResultItem>
    {
        public int Priority { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public ImageSource Icon { get; set; }

        public int CompareTo(ResultItem other) => Priority - other.Priority;

        public abstract void Open(KeyEventArgs e);
        //{
        //    if (WorkflowTrigger != null)
        //    {
        //        WorkflowTrigger.TriggerRunables(WorkflowArguments);
        //    }
        //    else
        //    {
        //        Process.Start(Path);
        //        SearchEngine.Instance.IncrementPriority(this);
        //    }
        //}

        //public void OpenFolder()
        //{
        //    if (WorkflowTrigger != null)
        //    {
        //        WorkflowTrigger.TriggerRunables(WorkflowArguments);
        //    }
        //    else if (Directory.Exists(Path))
        //    {
        //        Open();
        //    }
        //    else
        //    {
        //        Process.Start(Path.Substring(0, Path.LastIndexOf('\\')));
        //        SearchEngine.Instance.IncrementPriority(this);
        //    }
        //}

        public override string ToString() => Title + ", " + Priority;
    }
}