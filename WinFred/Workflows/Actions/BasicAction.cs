using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using James.Workflows.Interfaces;
using James.Workflows.Outputs;
using James.Workflows.Triggers;

namespace James.Workflows.Actions
{
    [DataContract]
    public class BasicAction : RunnableWorkflowComponent
    {
        public BasicAction(string executablePath)
        {
            ExecutablePath = executablePath;
        }

        public BasicAction()
        {
        }

        [DataMember]
        public List<BasicOutput> Displayables { get; set; } = new List<BasicOutput>();

        [DataMember]
        [ComponentField("The program to execute", true)]
        public virtual string ExecutablePath { get; set; } = "";

        [DataMember]
        [ComponentField("Arguments for the program")]
        public virtual string ExecutableArguments { get; set; } = "";

        protected void Display(string output) => Displayables.ForEach(basicOutput => basicOutput.Display(output));

        public override string GetSummary() => $"Runs {ExecutablePath.Split('\\').Last()}";

        public override void Run(string arguments = "")
        {
            var path = ExecutablePath;
            if (path[1] != ':')
            {
                path = ParentWorkflow.Path + "\\" + path;
            }
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = path,
                    Arguments = ExecutableArguments + arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            Display(proc.StandardOutput.ReadToEnd());
        }

        public override bool IsAllowed(WorkflowComponent source) => source is BasicTrigger;
    }
}