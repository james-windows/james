using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace WinFred.UserControls
{
    /// <summary>
    ///     Interaction logic for WorkflowsUserControl.xaml
    /// </summary>
    public partial class WorkflowsUserControl : UserControl
    {
        public WorkflowsUserControl()
        {
            InitializeComponent();
            DataContext = Config.GetInstance();
        }

        private void ChangeStatusMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ((Workflow) WorkflowListBox.SelectedItem).IsEnabled = !((Workflow) WorkflowListBox.SelectedItem).IsEnabled;
        }

        private async void DeletePathMenuItem_Click(object sender, RoutedEventArgs e)
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
                Config.GetInstance().Workflows.Remove(((Workflow) WorkflowListBox.SelectedItem));
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
                var wf = new Workflow {Name = name, IsEnabled = false};
                Config.GetInstance().Workflows.Add(wf);
                WorkflowListBox.SelectedIndex = Config.GetInstance().Workflows.Count - 1;
            }
        }
    }
}