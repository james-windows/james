using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using James.Workflows.Outputs;

namespace James.Workflows.Actions
{
    [DataContract]
    public class BasicAction : RunnableWorkflowComponent
    {
        public BasicAction(string executablePath)
        {
            ExecutablePath = executablePath;
        }

        private BasicAction()
        {
        }

        [DataMember]
        public List<BasicOutput> Displayables { get; set; } = new List<BasicOutput>();

        [DataMember]
        public string ExecutablePath { get; set; }

        public void Display(string output)
        {
            foreach (var item in Displayables)
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