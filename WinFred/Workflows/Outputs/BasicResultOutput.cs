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
                new BasicResultItem() {Subtitle = output, Title = ParentWorkflow.Name}
            };
            MainWindow.GetInstance().searchResultControl.WorkflowOutput(outputResults);
        }

        public override string GetSummary() => $"Displays the response in SearchResultList";
    }
}