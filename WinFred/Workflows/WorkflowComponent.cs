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
        public string Description => GetDescription();
        public abstract string GetSummary();
        public string GetDescription() => "A description for each Workflow component will be placed here soon! Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.";

        [DataMember]
        public Workflow ParentWorkflow { get; set; }
    }
}