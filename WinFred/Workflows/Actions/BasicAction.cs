using System.Diagnostics;
using System.Linq;
using James.Workflows.Outputs;
using James.Workflows.Triggers;

namespace James.Workflows.Actions
{
    public class BasicAction : RunnableWorkflowComponent
    {
        public BasicAction(string executablePath)
        {
            ExecutablePath = executablePath;
        }

        protected const char SEPARATOR = '\n';
        public BasicAction()
        {
        }

        [ComponentField("The program to execute", true)]
        public virtual string ExecutablePath { get; set; } = "";

        [ComponentField("Arguments for the program")]
        public virtual string ExecutableArguments { get; set; } = "";

        public override string GetSummary() => $"Runs {ExecutablePath.Split('\\').Last()}";

        public override void Run(string[] arguments)
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
            CallNext(proc.StandardOutput.ReadToEnd().Split(SEPARATOR));
        }

        public override bool IsAllowed(WorkflowComponent source) => base.IsAllowed(source) && (source is BasicTrigger || source is MagicOutput || source is BasicAction);
    }
}