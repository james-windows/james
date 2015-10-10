using System.Runtime.Serialization;

namespace James.Workflows.Outputs
{
    [DataContract]
    public class LargeTypeOutput : BasicOutput
    {
        public override void Display(string output)
        {
            //WorkflowManager.GetInstance(null).ParentSearchResultUserControl.GetParentWindow().DisplayLargeTypeOutput(output);
        }
    }
}