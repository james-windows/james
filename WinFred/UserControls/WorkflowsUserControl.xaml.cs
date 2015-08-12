using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;
using System.Windows.Controls;

namespace WinFred.UserControls
{
    /// <summary>
    /// Interaction logic for WorkflowsUserControl.xaml
    /// </summary>
    public partial class WorkflowsUserControl : UserControl
    {
        public WorkflowsUserControl()
        {
            InitializeComponent();
            this.DataContext = Config.GetInstance();
        }

        private void ChangeStatusMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ((Workflow) WorkflowListBox.SelectedItem).IsEnabled = !((Workflow) WorkflowListBox.SelectedItem).IsEnabled;
        }

        private async void DeletePathMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MetroWindow parentWindow = (MetroWindow)Window.GetWindow(this);
            MetroDialogSettings setting = new MetroDialogSettings(){NegativeButtonText = "Cancel", AffirmativeButtonText = "Yes, I'm sure!"};
            MessageDialogResult result = await parentWindow.ShowMessageAsync("Delete Workflow", "Are you sure?", MessageDialogStyle.AffirmativeAndNegative, setting);
            if (MessageDialogResult.Affirmative == result)
            {
                Config.GetInstance().Workflows.Remove(((Workflow)WorkflowListBox.SelectedItem));
            }
        }

        private async void AddWorkflowButton_Click(object sender, RoutedEventArgs e)
        {
            MetroWindow parentWindow = (MetroWindow)Window.GetWindow(this);
            string name = await parentWindow.ShowInputAsync("Create new Workflow", "What should be the name of your new Workflow?");
            if (name != null) 
            {
                Workflow wf = new Workflow() { Name = name, IsEnabled = false };
                Config.GetInstance().Workflows.Add(wf);
                WorkflowListBox.SelectedIndex = Config.GetInstance().Workflows.Count - 1;
            }
        }
    }
}
