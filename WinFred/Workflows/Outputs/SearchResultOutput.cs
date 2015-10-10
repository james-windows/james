using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using James.Workflows.Interfaces;

namespace James.Workflows.Outputs
{
    [DataContract]
    public class SearchResultOutput: BasicOutput
    {
        public override void Display(string output)
        {
            List<SearchResult> searchResults = new List<SearchResult>
            {
                new SearchResult() {Path = ParentWorkflow.Subtitle, Filename = output}
            };
            WorkflowManager.GetInstance(null).ParentSearchResultUserControl.WorkflowOutput(searchResults);
        }
    }
}
