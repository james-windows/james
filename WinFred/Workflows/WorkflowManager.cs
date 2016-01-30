using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using James.HelperClasses;
using James.ResultItems;
using James.Workflows.Triggers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
                string configPath = path + "\\config.json";
                if (File.Exists(configPath))
                {
                    try
                    {
                        dynamic item = JsonConvert.DeserializeObject(File.ReadAllText(configPath));
                        Workflows.Add(new Workflow(item, path));
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Failed to load Workflow. Invalid config.js format!");
                    }
                }
            }

            LoadKeywordTriggers();
        }

        public List<WorkflowComponent> AllComponents => Workflows.SelectMany(workflow => workflow.Components).ToList();

        public void LoadKeywordTriggers()
        {
            KeywordTriggers.Clear();
            foreach (var workflow in Workflows)
            {
                KeywordTriggers.AddRange(workflow.Components.OfType<KeywordTrigger>().ToList());
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
            var keywordTriggers = KeywordTriggers.Where(trigger => trigger.Keyword.StartsWith(input.Split(' ')[0]));
            foreach (var trigger in keywordTriggers.Where(trigger => trigger.Autorun))
            {
                trigger.Run(input.Split(' '));
            }
            return keywordTriggers.Where(trigger => !trigger.Autorun).Select(
                        trigger =>
                            new MagicResultItem()
                            {
                                Icon = trigger.ParentWorkflow.Icon,
                                WorkflowComponent = trigger,
                                Subtitle = trigger.Subtitle,
                                Title = trigger.Title,
                                WorkflowArguments = input.Split(' ')
                            });
        }

        public void PersistWorkflows() => Workflows.ForEach(workflow => workflow.Persist());

        public void CancelWorkflows() => Workflows.ForEach(workflow => workflow.Cancel());

        public void Remove(Workflow item)
        {
            item.Remove();
            Workflows.Remove(item);
            LoadKeywordTriggers();
        }

        public void RunApiTrigger(string input, string[] args)
        {
            AllComponents.OfType<ApiTrigger>().Where(trigger => trigger.Action.Length > 0 && trigger.Action == input).ForEach(trigger => trigger.CallNext(args));
        }
    }
}