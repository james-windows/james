using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            WorkflowTrigger.TriggerRunables(WorkflowArguments);
        }
    }
}
