using System.IO.Compression;
using System.Windows;
using System.Windows.Forms;
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

        private void ChangeStatusMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ((Workflow) WorkflowListBox.SelectedItem).IsEnabled = !((Workflow) WorkflowListBox.SelectedItem).IsEnabled;
        }

        private async void DeleteWorkflowButton_Click(object sender, RoutedEventArgs e)
        {
            var parentWindow = (MetroWindow) Window.GetWindow(this);
            var setting = new MetroDialogSettings
            {
                NegativeButtonText = "Cancel",
                AffirmativeButtonText = "Yes, I'm sure!"
            };
            var result =
                await
                    parentWindow.ShowMessageAsync("Delete Workflow", "Are you sure?",
                        MessageDialogStyle.AffirmativeAndNegative, setting);
            if (MessageDialogResult.Affirmative == result)
            {
                WorkflowManager.Instance.Remove((Workflow) WorkflowListBox.SelectedItem);
            }
        }

        private async void AddWorkflowButton_Click(object sender, RoutedEventArgs e)
        {
            var parentWindow = (MetroWindow) Window.GetWindow(this);
            var name =
                await
                    parentWindow.ShowInputAsync("Create new Workflow", "What should be the name of your new Workflow?");
            if (name != null)
            {
                var wf = new Workflow(name) {IsEnabled = true};
                WorkflowManager.Instance.Workflows.Add(wf);
                WorkflowListBox.SelectedIndex = WorkflowManager.Instance.Workflows.Count - 1;
            }
        }

        private void OpenWorkflowFolder(object sender, RoutedEventArgs e)
        {
            ((Workflow)WorkflowListBox.SelectedItem).OpenFolder();
        }

        private void ExportWorkflowButton_Click(object sender, RoutedEventArgs e)
        {
            var item = ((Workflow) WorkflowListBox.SelectedItem);
            FileDialog dialog = new SaveFileDialog();
            dialog.FileName = item.Name + ".james";
            dialog.Filter = "james workflows|*.james";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                ZipFile.CreateFromDirectory(Config.Instance.ConfigFolderLocation + "\\workflows\\" + item.Name, dialog.FileName);
            }
        }
    }
}