using System.Runtime.Serialization;
using System.Windows.Input;
using James.Workflows.Interfaces;

namespace James.Workflows.Outputs
{
    [DataContract]
    public class LargeTypeOutput : BasicOutput, ISurviveable
    {
        private bool firstRun { get; set; } = true;
        public override void Display(string output)
        {
            if (firstRun)
            {
                firstRun = false;
                MainWindow.GetInstance().DisplayLargeType(output);
            }
            else
            {
                LargeType.GetInstance().ChangeMessage(output);
            }
        }

        public void Cancel()
        {
            //LargeType.GetInstance().Window_KeyDown(this, null);
            firstRun = true;
        }
    }
}