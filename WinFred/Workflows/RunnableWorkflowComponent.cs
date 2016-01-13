namespace James.Workflows
{
    public abstract class RunnableWorkflowComponent : WorkflowComponent
    {
        protected RunnableWorkflowComponent(Workflow parent) : base(parent)
        {
        }

        protected RunnableWorkflowComponent()
        {
        }
    }
}