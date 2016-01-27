using James.Workflows.Interfaces;

namespace James.Workflows.Outputs
{
    public class LargeTypeOutput : BasicOutput, ISurviveable
    {
        private bool FirstRun { get; set; } = true;

        public void Cancel() => FirstRun = true;

        public override void Run(string[] output)
        {
            if (FirstRun)
            {
                FirstRun = false;
                Windows.MainWindow.GetInstance().DisplayLargeType(string.Join(" ", output));
            }
            else
            {
                Windows.LargeType.Instance.ChangeMessage(string.Join(" ", output));
            }
        }

        public override string GetSummary() => $"Displays the response in LargeType";
    }
}