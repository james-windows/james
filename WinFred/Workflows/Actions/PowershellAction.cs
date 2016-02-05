using System;
using System.Diagnostics;

namespace James.Workflows.Actions
{
    public class PowershellAction: BasicAction
    {
        public PowershellAction()
        {
            ExecutablePath = GetFullPathOfExe("powershell.exe");
        }

        [ComponentField("The Path of the script file")]
        public string Script { get; set; } = "";

        [ComponentField("Additional Arguments for the program")]
        public override string ExecutableArguments { get; set; } = "";

        public override string ExecutablePath { get; set; } = "";

        public override string GetSummary() => $"Runs {Script}";

        public override void Run(string[] arguments)
        {
            if (ExecutablePath == null)
            {
                CallNext(new string[] { "powershell.exe couldn't be found in the path" });
                return;
            }
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = ExecutablePath,
                    Arguments = ".\\" + Script + " " + ExecutableArguments + string.Join(" ", arguments),
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
