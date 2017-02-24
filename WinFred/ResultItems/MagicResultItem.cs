using System.Windows.Input;
using James.Windows;
using James.Workflows;
using James.Workflows.Outputs;
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
        public override void Open(KeyEventArgs e, string search, bool showFileProperties)
        {
            WorkflowComponent.ParentWorkflow.IsCanceled = false;
            WorkflowComponent.CallNext(WorkflowArguments);
            var magic = WorkflowComponent as MagicOutput;
            if (magic != null && magic.Hide)
            {
                MainWindow.GetInstance().HideWindow();
            }
        }

        /// <summary>
        /// Provides the string to use for the auto completion
        /// </summary>
        /// <returns></returns>
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
