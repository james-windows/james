using System.Linq;
using System.Windows;
using James.WorkflowEditor;
using James.Workflows;
using MahApps.Metro.Controls.Dialogs;
using UserControl = System.Windows.Controls.UserControl;
using James.HelperClasses;

namespace James.UserControls
{
    /// <summary>
    ///     Interaction logic for WorkflowsUserControl.xaml
    /// </summary>
    public partial class WorkflowsUserControl : UserControl
    {
        public WorkflowsUserControl()
        {
            InitializeComponent();
            DataContext = WorkflowManager.Instance;
        }

        /// <summary>
        /// Deletes a workflow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DeleteWorkflowButton_Click(object sender, RoutedEventArgs e)
        {
            (await MetroDialogHelper.ShowDialog(this, "Delete Workflow", "Are you sure?"))
            .OnSuccess(() =>
            {
                WorkflowManager.Instance.Remove((Workflow)WorkflowListBox.SelectedItem);
            });
        }

        /// <summary>
        /// Creates a new workflow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AddWorkflowButton_Click(object sender, RoutedEventArgs e)
        {
            var name = await this.GetWindow().ShowInputAsync("Create new Workflow", "What should be the name of your new Workflow?");
            if (name != null && WorkflowManager.Instance.Workflows.All(workflow => workflow.Name != name))
            {
                var wf = new Workflow(name) {IsEnabled = true};
                WorkflowManager.Instance.Workflows.Add(wf);
                WorkflowListBox.SelectedIndex = WorkflowManager.Instance.Workflows.Count - 1;
            }
        }

        private void OpenWorkflowFolder(object sender, RoutedEventArgs e) => ((Workflow)WorkflowListBox.SelectedItem).OpenFolder();

        private void ExportWorkflowButton_Click(object sender, RoutedEventArgs e) => ((Workflow)WorkflowListBox.SelectedItem).Export();

        /// <summary>
        /// Open the settings window of the workflow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenWorkflowSettings(object sender, RoutedEventArgs e)
        {
            var dialog = new WorkflowSettingsDialog((Workflow)WorkflowListBox.SelectedItem);
            this.GetWindow()?.ShowMetroDialogAsync(dialog);
            dialog.Unloaded += (o, args) =>
            {
                int index = WorkflowListBox.SelectedIndex;
                DataContext = null;
                DataContext = WorkflowManager.Instance;
                WorkflowListBox.SelectedIndex = index;
            };
        }
    }
}