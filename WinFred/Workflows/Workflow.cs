using System.Collections.Generic;
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
    [DataContract(IsReference = true)]
    public class Workflow
    {
        private Workflow()
        {
        }

        public Workflow(string title)
        {
            Title = title;
            Directory.CreateDirectory(Config.GetInstance().ConfigFolderLocation + "\\workflows\\" + title);
        }

        [DataMember(Order = 4)]
        public string IconPath { get; set; }

        public string Title { get; set; }

        [DataMember(Order = 2)]
        public string Subtitle { get; set; }

        [DataMember(Order = 3)]
        public string Author { get; set; }

        [DataMember(Order = 5)]
        public string Version { get; set; } = "0.1";

        [DataMember(Order = 6)]
        public bool IsEnabled { get; set; }

        [DataMember(Order = 7)]
        public List<BasicTrigger> Triggers { get; set; } = new List<BasicTrigger>();

        [DataMember(Order = 8)]
        public List<BasicAction> Actions { get; set; } = new List<BasicAction>();

        [DataMember(Order = 9)]
        public List<BasicOutput> Outputs { get; set; } = new List<BasicOutput>();

        public void Cancel()
        {
            Triggers.OfType<ISurviveable>().ToList().ForEach(surviveable => surviveable.Cancel());
            Actions.OfType<ISurviveable>().ToList().ForEach(surviveable => surviveable.Cancel());
            Outputs.OfType<ISurviveable>().ToList().ForEach(surviveable => surviveable.Cancel());
        }

        public void Persist()
        {
            var path = Config.GetInstance().ConfigFolderLocation + "\\workflows\\" + Title + "\\config.xml";
            File.WriteAllText(path, GeneralHelper.SerializeWorkflow(this));
        }
    }
}