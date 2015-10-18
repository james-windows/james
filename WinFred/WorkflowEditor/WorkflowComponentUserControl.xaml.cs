using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using James.Workflows;
using James.Workflows.Outputs;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace James.WorkflowEditor
{
    /// <summary>
    ///     Interaction logic for WorkflowComponentUserControl.xaml
    /// </summary>
    public partial class WorkflowComponentUserControl : UserControl
    {
        public delegate void ComponentUpdateHandler(object sender);

        public WorkflowComponentUserControl()
        {
            InitializeComponent();
        }

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

        public void OpenComponent(object sender = null, RoutedEventArgs e = null)
        {
            ComponentMetroDialog dialog = new ComponentMetroDialog((WorkflowComponent)DataContext);
            var window = Window.GetWindow(this);
            ((MetroWindow) window)?.ShowMetroDialogAsync(dialog);

            dialog.Unloaded += Dialog_Unloaded;
        }

        private void Dialog_Unloaded(object sender, RoutedEventArgs e)
        {
            SummaryTextBlock.Text = ((WorkflowComponent)DataContext).Summary;
        }

        private void OpenWorkflowFolder(object sender, RoutedEventArgs e)
            => ((WorkflowComponent)DataContext).ParentWorkflow.OpenFolder();
    }
}