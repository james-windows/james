using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
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

        [ComponentField("Run in background")]
        public bool Background { get; set; } = false;

        public virtual void Cancel()
        {
            if (!Background && proc != null && !proc.HasExited)
            {
                try
                {
                    proc?.Kill();
                }
                catch (Win32Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public override void CallNext(string[] arguments)
        {
            foreach (var id in ConnectedTo)
            {
                var nextComponent = this.GetNext(id);
                Task.Run(() => nextComponent.Run(arguments));
            }
        }

        public override void Run(string[] arguments)
        {
            if (Background == false && ParentWorkflow.IsCanceled)
            {
                return;
            }
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
        
        protected string[] StartProcess(string arguments)
        {
            proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = ExecutablePath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = ParentWorkflow.Path,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            proc.WaitForExit();
            return proc.StandardOutput.ReadToEnd().Split(new[] { SEPARATOR }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}