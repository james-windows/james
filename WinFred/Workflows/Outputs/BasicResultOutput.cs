using System.Collections.Generic;
using System.Runtime.Serialization;
using James.ResultItems;

namespace James.Workflows.Outputs
{
    [DataContract]
    public class BasicResultOutput : BasicOutput
    {
        public override void Display(string output)
        {
            var outputResults = new List<ResultItem>
            {
                new BasicResultItem() {Subtitle = ParentWorkflow.Subtitle, Title = output}
            };
            MainWindow.GetInstance().searchResultControl.WorkflowOutput(outputResults);
        }

        public override string GetSummary() => $"Displays the response in SearchResultList";
    }
}