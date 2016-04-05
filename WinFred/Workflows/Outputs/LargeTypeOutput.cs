using James.HelperClasses;
using James.Workflows.Interfaces;

namespace James.Workflows.Outputs
{
    public class LargeTypeOutput : BasicOutput, ISurviveable
    {
        private bool FirstRun { get; set; } = true;

        public void Cancel() => FirstRun = true;
       
        /// <summary>
        /// Displays the generated string (output arguments + FormatString) and shows it using
        /// the LargeType window
        /// </summary>
        /// <param name="output"></param>
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