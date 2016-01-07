using System;
using System.Threading.Tasks;
using System.Windows.Input;
using James.Workflows.Triggers;

namespace James.ResultItems
{
    public class WorkflowResultItem: ResultItem
    {
        public KeywordTrigger WorkflowTrigger { get; set; }
        public string WorkflowArguments { get; set; }

        public override void Open(KeyEventArgs e)
        {
            try
            {
                Task.Run(() => WorkflowTrigger.TriggerRunables(WorkflowArguments));
            }
            catch (Exception)
            {
                Console.WriteLine("Workflow execution triggerd an exception");
            }
        }
    }
}
