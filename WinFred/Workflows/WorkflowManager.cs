using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using James.HelperClasses;
using James.ResultItems;
using James.Workflows.Triggers;
using Newtonsoft.Json;

namespace James.Workflows
{
    internal class WorkflowManager
    {
        private static WorkflowManager _workflowManager;
        private static readonly object SingeltonLock = new object();

        private WorkflowManager()
        {
            if (!Directory.Exists(Config.WorkflowFolderLocation))
            {
                Directory.CreateDirectory(Config.WorkflowFolderLocation);
            }

            Directory.GetDirectories(Config.WorkflowFolderLocation).ForEach(dir => LoadWorkflow(dir));
            LoadKeywordTriggers();
        }

        /// <summary>
        /// Loads a workflow from an providen filepath
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool LoadWorkflow(string path)
        {
            string configPath = path + "\\config.json";
            if (File.Exists(configPath))
            {
                try
                {
                    dynamic item = JsonConvert.DeserializeObject(File.ReadAllText(configPath));
                    Workflow workflow = new Workflow(item, path);
                    Application.Current.Dispatcher.Invoke(() => Workflows.Add(workflow));
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to load Workflow. Invalid config.js format!");
                    return false;
                }
                return true;
            }
            return false;
        }

        public List<WorkflowComponent> AllComponents => Workflows.SelectMany(workflow => workflow.Components).ToList();

        /// <summary>
        /// Loads all Keywordtriggers over all workflows
        /// </summary>
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

        /// <summary>
        /// Returns all matched keywords
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public IEnumerable<ResultItem> GetKeywordTriggers(string input)
        {
            var keywordTriggers = KeywordTriggers.Where(trigger => trigger.Keyword.StartsWith(input.Split(' ')[0])).ToList();
            foreach (var trigger in keywordTriggers.Where(trigger => trigger.Autorun && input.StartsWith(trigger.Keyword)))
            {
                trigger.Run(input.Split(' '));
            }
            return keywordTriggers.Where(trigger => !trigger.Autorun || trigger.Keyword.StartsWith(input)).Select(
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

        /// <summary>
        /// Removes a workflow
        /// </summary>
        /// <param name="item"></param>
        public void Remove(Workflow item)
        {
            item.Remove();
            Workflows.Remove(item);
            LoadKeywordTriggers();
        }

        /// <summary>
        /// Runs all matching api triggers, which matches the action window
        /// </summary>
        /// <param name="input"></param>
        /// <param name="args"></param>
        public void RunApiTrigger(string input, string[] args)
        {
            AllComponents.OfType<ApiTrigger>().Where(trigger => trigger.Action.Length > 0 && trigger.Action == input).ForEach(trigger => trigger.CallNext(args));
        }
    }
}