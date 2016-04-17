using James.HelperClasses;
using James.Workflows.Interfaces;

namespace James.Workflows.Actions
{
    public class PythonAction : BasicAction, ICrossPlatform
    {
        [ComponentField("The Path of the script file")]
        public string Script { get; set; } = "";

        public PythonAction()
        {
            ExecutablePath = PathHelper.GetFullPathOfExe("python.exe");
        }

        public override string ExecutablePath { get; set; } = "";

        public override string GetSummary() => $"Runs {Script}";

        public override string ExecutableArguments { get; set; } = "{...}";

        /// <summary>
        /// Starts the action with the providen arguments
        /// </summary>
        /// <param name="arguments"></param>
        public override void Run(string[] arguments)
        {
            if (!Background && ParentWorkflow.IsCanceled)
            {
                return;
            }
            if (ExecutablePath == null)
            {
                CallNext(new[] {"python couldn't be found in the path"});
                return;
            }
            CallNext(StartProcess(Script + " " + ExecutableArguments.InsertArguments(arguments)));
        }
    }
}