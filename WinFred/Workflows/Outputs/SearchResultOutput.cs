using System.Collections.Generic;
using System.Runtime.Serialization;

namespace James.Workflows.Outputs
{
    [DataContract]
    public class SearchResultOutput : BasicOutput
    {
        public override void Display(string output)
        {
            var searchResults = new List<SearchResult>
            {
                new SearchResult {Path = ParentWorkflow.Subtitle, Filename = output}
            };
            MainWindow.GetInstance().searchResultControl.WorkflowOutput(searchResults);
        }

        public override string GetSummary() => $"Displays the response in SearchResultList";
    }
}