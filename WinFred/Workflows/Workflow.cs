using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using James.HelperClasses;
using James.Workflows.Actions;
using James.Workflows.Interfaces;
using James.Workflows.Outputs;
using James.Workflows.Triggers;
using Newtonsoft.Json.Linq;

namespace James.Workflows
{
    public class Workflow
    {
        private Workflow()
        {
        }

        public Workflow(dynamic item, string path)
        {
            Name = path.Split('\\').Last();
            Author = item.author;
            IsEnabled = item.enabled;

            foreach (var component in item.components)
            {
                Components.Add(WorkflowComponent.LoadComponent(component));
                Components.Last().ParentWorkflow = this;
            }
            _lastId = Components.Count > 0 ? Components.Last().Id + 1: 0;
        }

        public Workflow(string name)
        {
            Name = name;
            Directory.CreateDirectory(Path);
            Persist();
        }

        public string Name { get; set; }

        [DataMember(Order = 3)]
        public string Author { get; set; } = System.Security.Principal.WindowsIdentity.GetCurrent()?.Name.Split('\\').Last();

        [DataMember(Order = 6)]
        public bool IsEnabled { get; set; } = true;

        [DataMember(Order = 7)]
        public List<WorkflowComponent> Components { get; set; } = new List<WorkflowComponent>();

        public List<BasicTrigger> Triggers => Components.OfType<BasicTrigger>().ToList();
        public List<BasicAction> Actions => Components.OfType<BasicAction>().ToList();
        public List<BasicOutput> Outputs => Components.OfType<BasicOutput>().ToList();

        private int _lastId = 0;

        public string Path => Config.Instance.ConfigFolderLocation + "\\workflows\\" + Name;

        public bool canceld = false;

        public void Cancel()
        {
            canceld = true;
            Components.OfType<BasicAction>().ForEach(surviveable => surviveable.Cancel());
        }

        public void Persist()
        {
            dynamic workflow = new JObject();
            workflow.author = Author;
            workflow.enabled = IsEnabled;
            workflow.components = new JArray(Components.Select(component => component.Persist()).ToArray());
            File.WriteAllText(Path + "\\config.json", workflow.ToString());
        }

        public void AddComponent(WorkflowComponent instance)
        {
            instance.Id = _lastId;
            _lastId++;
            instance.ParentWorkflow = this;
            Components.Add(instance);
            WorkflowManager.Instance.LoadKeywordTriggers();
        }

        public void RemoveComponent(WorkflowComponent component)
        {
            Triggers.ForEach(trigger => trigger.ConnectedTo.Remove(component.Id));
            Actions.ForEach(action => action.ConnectedTo.Remove(component.Id));

            Components.Remove(component);
        }

        public void OpenFolder() => Process.Start(Path);

        public void Remove() => Directory.Delete(Path, true);
    }
}