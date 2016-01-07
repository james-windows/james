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

namespace James.Workflows
{
    [DataContract]
    public class Workflow
    {
        private Workflow()
        {
        }

        public Workflow(string name)
        {
            Name = name;
            Directory.CreateDirectory(Path);
            Persist();
        }

        [DataMember(Order = 4)]
        public string IconPath { get; set; } = "";

        public string Name { get; set; }

        [DataMember(Order = 3)]
        public string Author { get; set; } = System.Security.Principal.WindowsIdentity.GetCurrent()?.Name;

        [DataMember(Order = 5)]
        public string Version { get; set; } = "0.1";

        [DataMember(Order = 6)]
        public bool IsEnabled { get; set; } = true;

        [DataMember(Order = 7)]
        public List<WorkflowComponent> Components { get; set; } = new List<WorkflowComponent>();

        public List<BasicTrigger> Triggers => Components.OfType<BasicTrigger>().ToList();
        public List<BasicAction> Actions => Components.OfType<BasicAction>().ToList();
        public List<BasicOutput> Outputs => Components.OfType<BasicOutput>().ToList();

        public string Path => Config.Instance.ConfigFolderLocation + "\\workflows\\" + Name;

        public void Cancel()
        {
            Components.OfType<ISurviveable>().ForEach(surviveable => surviveable.Cancel());
        }

        public void Persist() => File.WriteAllText(Path + "\\config.json", this.Serialize());

        public void AddComponent(WorkflowComponent instance)
        {
            instance.ParentWorkflow = this;
            Components.Add(instance);
            WorkflowManager.Instance.LoadKeywordTriggers();
        }

        public void RemoveComponent(WorkflowComponent component)
        {
            Triggers.ForEach(trigger => trigger.Runnables.Remove(component as RunnableWorkflowComponent));
            Actions.ForEach(action => action.Displayables.Remove(component as BasicOutput));

            Components.Remove(component);
        }

        public void OpenFolder() => Process.Start(Path);

        public void Remove() => Directory.Delete(Path, true);
    }
}