using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
        public virtual string ExecutableArguments { get; set; } = "{...}";

        public override string GetSummary() => $"Runs {ExecutablePath.Split('\\').Last()}";

        protected Process proc;

        [ComponentField("Run in background")]
        public bool Background { get; set; } = false;

        /// <summary>
        /// Cancels the current Action
        /// </summary>
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

        /// <summary>
        /// Starts the Action
        /// </summary>
        /// <param name="arguments"></param>
        public override void Run(string[] arguments)
        {
            if (!Background && ParentWorkflow.IsCanceled)
            {
                return;
            }
            var path = ExecutablePath;
            if (path[1] != ':')
            {
                path = ParentWorkflow.Path + "\\" + path;
            }
            CallNext(StartProcess(ExecutableArguments.InsertArguments(arguments), path));
        }

        public override bool IsAllowed(WorkflowComponent source) => base.IsAllowed(source) && (source is BasicTrigger || source is MagicOutput || source is BasicAction);

        /// <summary>
        /// Starts the process for the action
        /// </summary>
        /// <param name="arguments"></param>
        /// <param name="executablePath"></param>
        /// <returns></returns>
        protected string[] StartProcess(string arguments, string executablePath = null)
        {
            ProcessStartInfo info = new ProcessStartInfo
            {
                FileName = executablePath ?? ExecutablePath,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = ParentWorkflow.Path,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8
        };
            
            proc = new Process
            {
                StartInfo = info
            };
            proc.Start();
            proc.WaitForExit();
            
            string output = proc.StandardOutput.ReadToEnd();
            return output.Split(new[] { SEPARATOR }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}