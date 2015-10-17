using System.Windows;
using System.Windows.Controls;
using James.Workflows;
using James.Workflows.Outputs;

namespace James.WorkflowEditor
{
    /// <summary>
    ///     Interaction logic for WorkflowComponentUserControl.xaml
    /// </summary>
    public partial class WorkflowComponentUserControl : UserControl
    {
        public delegate void ComponentUpdateHandler(object sender);

        public WorkflowComponentUserControl(WorkflowComponent workflowComponent)
        {
            InitializeComponent();
            DataContext = workflowComponent;
            if (workflowComponent is BasicOutput)
            {
                rightAnchor.Visibility = Visibility.Hidden;
            }
        }

        public event ComponentUpdateHandler OnUpdate;

        private void RemoveComponent(object sender, RoutedEventArgs e)
        {
            var component = (WorkflowComponent) ((MenuItem) sender).DataContext;
            component.ParentWorkflow.RemoveComponent(component);
            OnUpdate?.Invoke(this);
        }

        public void OpenComponent()
        {
            var window = new ComponentWindow((WorkflowComponent) DataContext) {Owner = Window.GetWindow(this)};
            window.ShowDialog();
        }

        private void EditComponent(object sender, RoutedEventArgs e) => OpenComponent();

        private void OpenWorkflowFolder(object sender, RoutedEventArgs e)
            => ((WorkflowComponent)DataContext).ParentWorkflow.OpenFolder();
    }
}