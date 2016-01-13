using System.Diagnostics;
using James.Workflows.Interfaces;

namespace James.Workflows.Actions
{
    public class PythonAction : BasicAction, ICrossPlatform
    {
        [ComponentField("The Path of the script file")]
        public string Script { get; set; } = "";

        [ComponentField("Additional Arguments for the program")]
        public override string ExecutableArguments { get; set; } = "";

        public override string ExecutablePath { get; set; } = @"C:\Portable\WinPython-64bit-3.4.3.7\python-3.4.3.amd64\python.exe";

        public override string GetSummary() => $"Runs {Script}";

        public override void Run(string[] arguments)
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = ExecutablePath,
                    Arguments = Script + " " + ExecutableArguments + string.Join(" ", arguments),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = ParentWorkflow.Path,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            proc.WaitForExit();
            CallNext(proc.StandardOutput.ReadToEnd().Split(SEPARATOR));
        }
    }
}