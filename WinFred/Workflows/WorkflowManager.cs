using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using James.HelperClasses;
using James.ResultItems;
using James.Workflows.Triggers;

namespace James.Workflows
{
    internal class WorkflowManager
    {
        private static WorkflowManager _workflowManager;
        private static readonly object SingeltonLock = new object();

        private WorkflowManager()
        {
            if (!Directory.Exists(Config.Instance.ConfigFolderLocation + "\\workflows"))
            {
                Directory.CreateDirectory(Config.Instance.ConfigFolderLocation + "\\workflows");
            }
            foreach (var path in Directory.GetDirectories(Config.Instance.ConfigFolderLocation + "\\workflows"))
            {
                Workflows.Add(GeneralHelper.DeserializeWorkflow(path + "\\config.xml"));
                Workflows.Last().Title = path.Split('\\').Last();
            }

            foreach (var workflow in Workflows)
            {
                KeywordTriggers.AddRange(workflow.Triggers.OfType<KeywordTrigger>());
            }
        }

        public ObservableCollection<Workflow> Workflows { get; set; } = new ObservableCollection<Workflow>();
        public List<KeywordTrigger> KeywordTriggers { get; set; } = new List<KeywordTrigger>();

        public static WorkflowManager Instance
        {
            get
            {
                lock (SingeltonLock)
                {
                    return _workflowManager ?? (_workflowManager = new WorkflowManager());
                }
            }
        }

        public IEnumerable<ResultItem> GetKeywordTriggers(string input)
        {
            return
                KeywordTriggers.Where(trigger => trigger.Keyword.StartsWith(input.Split(' ')[0]))
                    .Select(
                        trigger =>
                            new WorkflowResultItem
                            {
                                WorkflowTrigger = trigger,
                                Subtitle = trigger.ParentWorkflow.Subtitle,
                                Title = trigger.ParentWorkflow.Title,
                                WorkflowArguments = input.Replace(input.Split(' ')[0], "").Trim()
                            });
        }

        public void PersistWorkflows() => Workflows.ToList().ForEach(workflow => workflow.Persist());

        public void CancelWorkflows() => Workflows.ToList().ForEach(workflow => workflow.Cancel());

        public void Remove(Workflow item)
        {
            item.Remove();
            Workflows.Remove(item);
        }
    }
}