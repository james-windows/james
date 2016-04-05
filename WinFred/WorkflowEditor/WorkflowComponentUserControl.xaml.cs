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

        /// <summary>
        /// Delets the component
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveComponent(object sender, RoutedEventArgs e)
        {
            Component.ParentWorkflow.RemoveComponent(Component);
            OnUpdate?.Invoke(this);
        }

        /// <summary>
        /// Opens the details of an component
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OpenComponent(object sender = null, RoutedEventArgs e = null)
        {
            var dialog = new ComponentMetroDialog(Component);
            var window = Window.GetWindow(this);
            ((MetroWindow) window)?.ShowMetroDialogAsync(dialog);

            dialog.Unloaded += Dialog_Unloaded;
        }

        /// <summary>
        /// Reloads the summary text if the form for the component got closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Dialog_Unloaded(object sender, RoutedEventArgs e)
        {
            SummaryTextBlock.Text = Component.Summary;
        }

        private void OpenWorkflowFolder(object sender, RoutedEventArgs e) => Component.ParentWorkflow.OpenFolder();

        /// <summary>
        /// Adds a connection to the component
        /// </summary>
        /// <param name="component"></param>
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

        /// <summary>
        /// Updates the postion of the user control to the providen WorkflowComponent
        /// </summary>
        public void UpdatePosition()
        {
            Canvas.SetLeft(this, Component.X);
            Canvas.SetTop(this, Component.Y);
        }
    }
}