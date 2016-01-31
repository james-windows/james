using System.Collections.Generic;
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

        public WorkflowComponent Component => DataContext as WorkflowComponent;

        public WorkflowComponentUserControl()
        {
            InitializeComponent();
        }

        public WorkflowComponentUserControl(WorkflowComponent workflowComponent)
        {
            InitializeComponent();
            DataContext = workflowComponent;
            UpdatePosition();
            if (workflowComponent is BasicOutput && !(workflowComponent is MagicOutput))
            {
                rightAnchor.Visibility = Visibility.Hidden;
            }
        }

        public event ComponentUpdateHandler OnUpdate;

        private void RemoveComponent(object sender, RoutedEventArgs e)
        {
            Component.ParentWorkflow.RemoveComponent(Component);
            OnUpdate?.Invoke(this);
        }

        public void OpenComponent(object sender = null, RoutedEventArgs e = null)
        {
            var dialog = new ComponentMetroDialog(Component);
            var window = Window.GetWindow(this);
            ((MetroWindow) window)?.ShowMetroDialogAsync(dialog);

            dialog.Unloaded += Dialog_Unloaded;
        }

        private void Dialog_Unloaded(object sender, RoutedEventArgs e)
        {
            SummaryTextBlock.Text = Component.Summary;
        }

        private void OpenWorkflowFolder(object sender, RoutedEventArgs e) => Component.ParentWorkflow.OpenFolder();

        public void NewSource(WorkflowComponent component)
        {
            if (component == null || !(Component.IsAllowed(component) || component.ConnectedTo.Contains(Component.Id)))
            {
                leftAnchor.Visibility = Visibility.Hidden;
            }
            else
            {
                leftAnchor.Visibility = Visibility.Visible;
            }
        }

        public void UpdatePosition()
        {
            Canvas.SetLeft(this, Component.X);
            Canvas.SetTop(this, Component.Y);
        }
    }
}