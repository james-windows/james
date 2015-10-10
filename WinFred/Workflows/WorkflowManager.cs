using System.Collections.Generic;
using System.IO;
using System.Linq;
using James.HelperClasses;
using James.UserControls;
using James.Workflows.Outputs;
using James.Workflows.Triggers;

namespace James.Workflows
{
    internal class WorkflowManager
    {
        private static WorkflowManager _workflowManager;
        private static readonly object SingeltonLock = new object();

        private WorkflowManager()
        {
            foreach (string path in Directory.GetDirectories(Config.GetInstance().ConfigFolderLocation + "\\workflows"))
            {
                Workflows.Add(GeneralHelper.DeserializeWorkflow(path + "\\config.xml"));
                Workflows.Last().Title = path.Split('\\').Last();
            }

            foreach (var workflow in Workflows)
            {
                KeywordTriggers.AddRange(workflow.Triggers.OfType<KeywordTrigger>());
            }
            //PersistWorkflows();
        }

        public List<Workflow> Workflows { get; set; } = new List<Workflow>();
        public List<KeywordTrigger> KeywordTriggers { get; set; } = new List<KeywordTrigger>();

        public static WorkflowManager GetInstance()
        {
            lock (SingeltonLock)
            {
                return _workflowManager ?? (_workflowManager = new WorkflowManager());
            }
        }

        public IEnumerable<SearchResult> GetKeywordTriggers(string input)
        {
            return
                KeywordTriggers.Where(trigger => trigger.Keyword.StartsWith(input.Split(' ')[0]))
                    .Select(
                        trigger =>
                            new SearchResult
                            {
                                Path = trigger.ParentWorkflow.Subtitle,
                                Filename = trigger.ParentWorkflow.Title,
                                WorkflowTrigger = trigger,
                                WorkflowArguments = input.Replace(input.Split(' ')[0], "").Trim()
                            });
        }

        public void PersistWorkflows() => Workflows.ForEach(workflow => workflow.Persist());

        public void CancelWorkflows() => Workflows.ForEach(workflow => workflow.Cancel());
    }
}