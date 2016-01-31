using System.Linq;
using System.Windows;
using James.WorkflowEditor;
using James.Workflows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using UserControl = System.Windows.Controls.UserControl;

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

        private async void DeleteWorkflowButton_Click(object sender, RoutedEventArgs e)
        {
            var parentWindow = (MetroWindow) Window.GetWindow(this);
            var setting = new MetroDialogSettings
            {
                NegativeButtonText = "Cancel",
                AffirmativeButtonText = "Yes, I'm sure!"
            };
            var result = await parentWindow.ShowMessageAsync("Delete Workflow", "Are you sure?", MessageDialogStyle.AffirmativeAndNegative, setting);
            if (MessageDialogResult.Affirmative == result)
            {
                WorkflowManager.Instance.Remove((Workflow) WorkflowListBox.SelectedItem);
            }
        }

        private async void AddWorkflowButton_Click(object sender, RoutedEventArgs e)
        {
            var parentWindow = (MetroWindow) Window.GetWindow(this);
            var name = await parentWindow.ShowInputAsync("Create new Workflow", "What should be the name of your new Workflow?");
            if (name != null && WorkflowManager.Instance.Workflows.All(workflow => workflow.Name != name))
            {
                var wf = new Workflow(name) {IsEnabled = true};
                WorkflowManager.Instance.Workflows.Add(wf);
                WorkflowListBox.SelectedIndex = WorkflowManager.Instance.Workflows.Count - 1;
            }
        }

        private void OpenWorkflowFolder(object sender, RoutedEventArgs e) => ((Workflow)WorkflowListBox.SelectedItem).OpenFolder();

        private void ExportWorkflowButton_Click(object sender, RoutedEventArgs e) => ((Workflow)WorkflowListBox.SelectedItem).Export();

        private void OpenWorkflowSettings(object sender, RoutedEventArgs e)
        {
            var dialog = new WorkflowSettingsDialog((Workflow)WorkflowListBox.SelectedItem);
            var window = Window.GetWindow(this);
            ((MetroWindow)window)?.ShowMetroDialogAsync(dialog);
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