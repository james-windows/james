using James.HelperClasses;
using James.Workflows.Interfaces;

namespace James.Workflows.Outputs
{
    public class LargeTypeOutput : BasicOutput, ISurviveable
    {
        private bool FirstRun { get; set; } = true;

        public void Cancel() => FirstRun = true;
       
        public override void Run(string[] output)
        {
            string text = FormatString.InsertArguments(output);
            if (FirstRun)
            {
                FirstRun = false;
                Windows.MainWindow.GetInstance().DisplayLargeType(text);
            }
            else
            {
                Windows.LargeType.Instance.ChangeMessage(text);
            }
        }

        public override string GetSummary() => $"Displays the response in LargeType";
    }
}