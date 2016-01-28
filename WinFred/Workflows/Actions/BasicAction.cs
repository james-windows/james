using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using James.HelperClasses;
using James.Workflows.Outputs;
using James.Workflows.Triggers;

namespace James.Workflows.Actions
{
    public class BasicAction : WorkflowComponent
    {
        public BasicAction(string executablePath)
        {
            ExecutablePath = executablePath;
        }

        protected const string SEPARATOR = "\r\n";
        public BasicAction()
        {
        }

        [ComponentField("The program to execute", true)]
        public virtual string ExecutablePath { get; set; } = "";

        [ComponentField("Arguments for the program")]
        public virtual string ExecutableArguments { get; set; } = "";

        public override string GetSummary() => $"Runs {ExecutablePath.Split('\\').Last()}";

        protected Process proc;

        public bool Background { get; set; } = false;

        public virtual void Cancel()
        {
            if (!Background)
            {
                proc.Kill();
            }
        }

        public override void CallNext(string[] arguments)
        {
            if (Background && ParentWorkflow.canceld)
            {
                base.CallNext(arguments);
            }
            foreach (var id in ConnectedTo)
            {
                var nextComponent = this.GetNext(id);
                if (!(nextComponent is BasicAction && !(nextComponent as BasicAction).Background))
                {
                    Task.Run(() => nextComponent.Run(arguments));
                }
            }
        }

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
            CallNext(proc.StandardOutput.ReadToEnd().Split(new[] {SEPARATOR}, StringSplitOptions.RemoveEmptyEntries));
        }

        public override bool IsAllowed(WorkflowComponent source) => base.IsAllowed(source) && (source is BasicTrigger || source is MagicOutput || source is BasicAction);
    }
}