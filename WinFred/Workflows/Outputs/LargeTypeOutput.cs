using System.Runtime.Serialization;
using System.Windows.Input;
using James.Workflows.Interfaces;

namespace James.Workflows.Outputs
{
    [DataContract]
    public class LargeTypeOutput : BasicOutput, ISurviveable
    {
        private bool FirstRun { get; set; } = true;
        public override void Display(string output)
        {
            if (FirstRun)
            {
                FirstRun = false;
                MainWindow.GetInstance().DisplayLargeType(output);
            }
            else
            {
                LargeType.Instance.ChangeMessage(output);
            }
        }

        public override string GetSummary() => $"Displays the response in LargeType";

        public void Cancel()
        {
            FirstRun = true;
        }
    }
}