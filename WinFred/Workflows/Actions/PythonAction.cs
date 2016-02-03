using System;
using System.Diagnostics;
using James.Workflows.Interfaces;

namespace James.Workflows.Actions
{
    public class PythonAction : BasicAction, ICrossPlatform
    {
        [ComponentField("The Path of the script file")]
        public string Script { get; set; } = "";

        public PythonAction()
        {
            ExecutablePath = GetFullPathOfExe("python.exe");
        }

        public override string ExecutableArguments { get; set; } = "";

        public override string ExecutablePath { get; set; } = "";

        public override string GetSummary() => $"Runs {Script}";

        public override void Run(string[] arguments)
        {
            if (ExecutablePath == null)
            {
                CallNext(new string[] {"python couldn't be found in the path"});
                return;
            }
            proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = ExecutablePath,
                    Arguments = Script + " " + string.Join(" ", arguments),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = ParentWorkflow.Path,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            proc.WaitForExit();
            CallNext(proc.StandardOutput.ReadToEnd().Split(new[] { SEPARATOR }, StringSplitOptions.RemoveEmptyEntries));
        }
    }
}