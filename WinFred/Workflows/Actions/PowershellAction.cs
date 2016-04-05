using James.HelperClasses;

namespace James.Workflows.Actions
{
    public class PowershellAction: BasicAction
    {
        [ComponentField("The Path of the script file")]
        public string Script { get; set; } = "";

        public PowershellAction()
        {
            ExecutablePath = PathHelper.GetFullPathOfExe("powershell.exe");
        }

        public override string ExecutableArguments { get; set; } = "";

        public override string ExecutablePath { get; set; } = "";

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
                CallNext(new string[] { "powershell.exe couldn't be found in the path" });
                return;
            }
            CallNext(StartProcess(".\\" + Script + " " + string.Join(" ", arguments)));
        }
    }
}
