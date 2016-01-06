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
        public List<BasicTrigger> Triggers { get; set; } = new List<BasicTrigger>();

        [DataMember(Order = 8)]
        public List<BasicAction> Actions { get; set; } = new List<BasicAction>();

        [DataMember(Order = 9)]
        public List<BasicOutput> Outputs { get; set; } = new List<BasicOutput>();


        public string Path => Config.Instance.ConfigFolderLocation + "\\workflows\\" + Name;

        public void Cancel()
        {
            Triggers.OfType<ISurviveable>().ForEach(surviveable => surviveable.Cancel());
            Actions.OfType<ISurviveable>().ForEach(surviveable => surviveable.Cancel());
            Outputs.OfType<ISurviveable>().ForEach(surviveable => surviveable.Cancel());
        }

        public void Persist() => File.WriteAllText(Path + "\\config.json", this.Serialize());

        public void AddComponent(WorkflowComponent instance)
        {
            instance.ParentWorkflow = this;
            var trigger = instance as BasicTrigger;
            if (trigger != null)
            {
                Triggers.Add(trigger);
                WorkflowManager.Instance.LoadKeywordTriggers();
            }
            var action = instance as BasicAction;
            if (action != null)
            {
                Actions.Add(action);
            }
            var output = instance as BasicOutput;
            if (output != null)
            {
                Outputs.Add(output);
            }
        }

        public void RemoveComponent(WorkflowComponent component)
        {
            var basicTrigger = component as BasicTrigger;
            if (basicTrigger != null)
            {
                foreach (var trigger in Triggers)
                {
                    trigger.Runnables.Remove(basicTrigger);
                }
                Triggers.Remove(basicTrigger);
                WorkflowManager.Instance.LoadKeywordTriggers();
            }

            var basicAction = component as BasicAction;
            if (basicAction != null)
            {
                foreach (var trigger in Triggers)
                {
                    trigger.Runnables.Remove(basicAction);
                }
                Actions.Remove(basicAction);
            }

            var basicOutput = component as BasicOutput;
            if (basicOutput != null)
            {
                foreach (var action in Actions)
                {
                    action.Displayables.Remove(basicOutput);
                }
                Outputs.Remove(basicOutput);
            }
        }

        public void OpenFolder() => Process.Start(Path);

        public void Remove() => Directory.Delete(Path, true);
    }
}