using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using James.HelperClasses;
using James.UserControls;
using James.Workflows.Actions;
using James.Workflows.Outputs;
using James.Workflows.Triggers;

namespace James.Workflows
{
    class WorkflowManager
    {
        private static WorkflowManager _workflowManager;
        private static readonly object SingeltonLock = new object();
        public SearchResultUserControl ParentSearchResultUserControl { get; set; }
        public static WorkflowManager GetInstance(SearchResultUserControl searchResultUserControl)
        {
            lock (SingeltonLock)
            {
                return _workflowManager ?? (_workflowManager = new WorkflowManager(searchResultUserControl));
            }
        }

        public List<Workflow> Workflows { get; set; } = new List<Workflow>();
        public List<KeywordTrigger> KeywordTriggers { get; set; } = new List<KeywordTrigger>();

        private WorkflowManager(SearchResultUserControl searchResultUserControl)
        {
            ParentSearchResultUserControl = searchResultUserControl;
            //Workflow tmp = new Workflow("Timer") {Author = "Michael Moser", Subtitle = "Zeigt die aktuelle Uhrzeit an..."};
            //tmp.Outputs.Add(new LargeTypeOutput());
            //tmp.Actions.Add(new BasicAction($@"{Config.GetInstance().ConfigFolderLocation}\workflows\{tmp.Title}\timer.exe") {ParentWorkflow = tmp});
            //tmp.Actions.First().Displayables.Add(tmp.Outputs.First());
            //KeywordTrigger keywordTriggers = new KeywordTrigger(tmp) {Keyword = "time"};
            //IntervalTrigger intervalTrigger = new IntervalTrigger(tmp) {Interval = 1000};
            //tmp.Triggers.Add(keywordTriggers);
            //keywordTriggers.Runnables.Add(intervalTrigger);
            //tmp.Triggers.Add(intervalTrigger);
            //intervalTrigger.Runnables.Add(tmp.Actions.First());
            //Workflows.Add(tmp);

            Workflows.Add(GeneralHelper.DeserializeWorkflow($@"{Config.GetInstance().ConfigFolderLocation}\workflows\Timer\config.xml"));

            foreach (var workflow in Workflows)
            {
                KeywordTriggers.AddRange(workflow.Triggers.OfType<KeywordTrigger>());
            }
            //PersistWorkflows();
        }

        public IEnumerable<SearchResult> GetKeywordTriggers(string input)
        {
            return KeywordTriggers.Where(trigger => trigger.Keyword.StartsWith(input)).Select(trigger => new SearchResult() { Path = trigger.ParentWorkflow.Subtitle, Filename = trigger.ParentWorkflow.Title, WorkflowTrigger = trigger });
        }

        public void PersistWorkflows() => Workflows.ForEach(workflow => workflow.Persist());

        public void CancelWorkflows() => Workflows.ForEach(workflow => workflow.Cancel());
    }
}
