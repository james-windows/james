using System.Windows.Input;
using James.Workflows;
using James.Workflows.Triggers;

namespace James.ResultItems
{
    public class MagicResultItem:ResultItem
    {
        public WorkflowComponent WorkflowComponent { get; set; }

        public string[] WorkflowArguments { get; set; }

        /// <summary>
        /// Does basically nothing because it's only in use to display information
        /// </summary>
        /// <param name="e"></param>
        /// <param name="search">The current text in the SearchBox</param>
        public override void Open(KeyEventArgs e, string search)
        {
            WorkflowComponent.CallNext(search.Split(' '));
        }

        public override string AutoComplete()
        {
            if (WorkflowComponent is KeywordTrigger)
            {
                return (WorkflowComponent as KeywordTrigger).Keyword + " ";
            }
            return null;
        }
    }
}
