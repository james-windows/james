using System.Collections.Generic;
using System.Runtime.Serialization;
using James.ResultItems;

namespace James.Workflows.Outputs
{
    [DataContract]
    public class SearchResultOutput : BasicOutput
    {
        public override void Display(string output)
        {
            var searchResults = new List<ResultItem>
            {
                new WorkflowResultItem {Subtitle = ParentWorkflow.Subtitle, Title = output}
            };
            MainWindow.GetInstance().searchResultControl.WorkflowOutput(searchResults);
        }

        public override string GetSummary() => $"Displays the response in SearchResultList";
    }
}