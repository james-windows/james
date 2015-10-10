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
            WorkflowManager.GetInstance(null).ParentSearchResultUserControl.WorkflowOutput(searchResults);
        }
    }
}