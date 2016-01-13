using System.Collections.Generic;
using James.ResultItems;

namespace James.Workflows.Outputs
{
    public class MagicOutput : BasicOutput
    {
        public override void Run(string[] output)
        {
            var outputResults = new List<ResultItem>
            {
                new MagicResultItem() {Subtitle = output[0], Title = ParentWorkflow.Name, WorkflowComponent = this}
            };
            MainWindow.GetInstance().searchResultControl.WorkflowOutput(outputResults);
        }

        public override string GetSummary() => $"Displays the response in SearchResultList";
    }
}