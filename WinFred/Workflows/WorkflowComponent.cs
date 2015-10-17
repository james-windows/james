using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Windows;
using James.Workflows.Outputs;

namespace James.Workflows
{
    [DataContract(IsReference = true), KnownType(typeof (RunnableWorkflowComponent)), KnownType(typeof (BasicOutput))]
    public abstract class WorkflowComponent
    {
        public FrameworkElement UiElement { get; set; }
        public string Name => Regex.Replace(GetType().Name, "(?<=[a-z])([A-Z])", " $1", RegexOptions.Compiled);
        public string Summary => GetSummary();

        public abstract string GetSummary();

        public WorkflowComponent()
        {
        }

        public WorkflowComponent(Workflow parent)
        {
            ParentWorkflow = parent;
        }

        [DataMember]
        public Workflow ParentWorkflow { get; set; }
    }
}