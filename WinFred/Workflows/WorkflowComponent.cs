using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Windows;
using James.Workflows.Outputs;

namespace James.Workflows
{
    [DataContract(IsReference = true), KnownType(typeof (RunnableWorkflowComponent)), KnownType(typeof (BasicOutput))]
    public abstract class WorkflowComponent
    {
        protected WorkflowComponent()
        {
        }

        protected WorkflowComponent(Workflow parent)
        {
            ParentWorkflow = parent;
        }

        public FrameworkElement UiElement { get; set; }
        public string Name => Regex.Replace(GetType().Name, "(?<=[a-z])([A-Z])", " $1", RegexOptions.Compiled);
        public string Summary => GetSummary();
        public abstract string GetSummary();

        [DataMember]
        public Workflow ParentWorkflow { get; set; }
    }
}