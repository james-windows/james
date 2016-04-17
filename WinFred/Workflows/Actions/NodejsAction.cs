using James.HelperClasses;
using James.Workflows.Interfaces;

namespace James.Workflows.Actions
{
    public class NodejsAction: BasicAction, ICrossPlatform
    {
        public NodejsAction()
        {
            ExecutablePath = PathHelper.GetFullPathOfExe("node.exe");
        }

        [ComponentField("The Path of the script file")]
        public string Script { get; set; } = "";

        public override string ExecutablePath { get; set; } = "";

        public override string ExecutableArguments { get; set; } = "{...}";

        public override string GetSummary() => $"Runs {Script}";

        /// <summary>
        /// Starts the action with the providen arguments
        /// </summary>
        /// <param name="arguments"></param>
        public override void Run(string[] arguments)
        {
            if (Background == false && ParentWorkflow.IsCanceled)
            {
                return;
            }
            if (ExecutablePath == null)
            {
                CallNext(new string[] { "node.exe couldn't be found in the path" });
                return;
            }
            CallNext(StartProcess(Script + " " + ExecutableArguments.InsertArguments(arguments)));
        }
    }
}
