using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using James.Workflows.Interfaces;
using James.Workflows.Outputs;

namespace James.Workflows.Actions
{
    [DataContract]
    public class BasicAction: RunnableWorkflowComponent
    {
        [DataMember]
        public List<BasicOutput> Displayables { get; set; } = new List<BasicOutput>();

        [DataMember]
        public string ExecutablePath { get; set; } 

        public BasicAction(string executablePath)
        {
            ExecutablePath = executablePath;
        }

        private BasicAction()
        {
            
        }

        public void Display(string output)
        {
            foreach (BasicOutput item in Displayables)
            {
                item.Display(output);
            }
        }

        public override void Run()
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = ExecutablePath,
                    Arguments = "",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            Display(proc.StandardOutput.ReadToEnd());
        }
    }
}
