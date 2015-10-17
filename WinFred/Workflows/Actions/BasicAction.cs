using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        public BasicAction()
        {
        }

        [DataMember]
        public List<BasicOutput> Displayables { get; set; } = new List<BasicOutput>();

        [DataMember]
        [ComponentField("The program to execute", true)]
        public string ExecutablePath { get; set; } = "";

        [DataMember]
        [ComponentField("Arguments for the program")]
        public string ExecutableArguments { get; set; } = "";

        private void Display(string output) => Displayables.ForEach(basicOutput => basicOutput.Display(output));

        public override string GetSummary() => $"Runs {ExecutablePath.Split('\\').Last()}";

        public override void Run(string output = "")
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = ExecutablePath,
                    Arguments = ExecutableArguments + output,
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