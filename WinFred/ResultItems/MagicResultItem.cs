using System.Windows.Input;
using James.Workflows;

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
        public override void Open(KeyEventArgs e)
        {
            WorkflowComponent.CallNext(new [] {Subtitle});
        }

        public override string AutoComplete()
        {
            return Title;
        }
    }
}
