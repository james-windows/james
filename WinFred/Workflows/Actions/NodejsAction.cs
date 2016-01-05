using System.Diagnostics;
using System.Runtime.Serialization;
using James.Workflows.Interfaces;

namespace James.Workflows.Actions
{
    [DataContract]
    public class NodejsAction: BasicAction, ICrossPlatform
    {
        [DataMember]
        [ComponentField("The Path of the script file")]
        public string ScriptFile { get; set; } = "";

        [DataMember]
        [ComponentField("Additional Arguments for the program")]
        public override string ExecutableArguments { get; set; } = "";


        public override string ExecutablePath { get; set; } = @"C:\Users\moser\cmd\node.exe";

        public override void Run(string arguments = "")
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = ExecutablePath,
                    Arguments = ScriptFile + " " + ExecutableArguments + arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = ParentWorkflow.Path,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            proc.WaitForExit();
            Display(proc.StandardOutput.ReadToEnd());
            
        }
    }
}
