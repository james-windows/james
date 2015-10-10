using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
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
        [DataMember(Order = 4)]
        public string IconPath { get; set; }
        [DataMember(Order = 1)]
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

        private Workflow()
        {
            
        }

        public Workflow(string title)
        {
            Title = title;
            Directory.CreateDirectory(Config.GetInstance().ConfigFolderLocation + "\\workflows\\" + title);
        }

        public void Cancel()
        {
            Triggers.OfType<ISurviveable>().ToList().ForEach(surviveable => surviveable.Cancel());
            Actions.OfType<ISurviveable>().ToList().ForEach(surviveable => surviveable.Cancel());
            Outputs.OfType<ISurviveable>().ToList().ForEach(surviveable => surviveable.Cancel());
        }

        public void Persist()
        {
            string path = Config.GetInstance().ConfigFolderLocation + "\\workflows\\" + Title + "\\config.xml";
            File.WriteAllText(path, GeneralHelper.SerializeWorkflow(this));
        }
    }
}
