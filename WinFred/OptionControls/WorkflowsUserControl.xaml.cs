using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WinFred.OptionControls
{
    /// <summary>
    /// Interaction logic for WorkflowsUserControl.xaml
    /// </summary>
    public partial class WorkflowsUserControl : UserControl
    {
        public WorkflowsUserControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            WorkflowListBox.ItemsSource = Config.GetInstance().Workflows;
        }

        private void ChangeStatusMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ((Workflow) WorkflowListBox.SelectedItem).IsEnabled = !((Workflow) WorkflowListBox.SelectedItem).IsEnabled;
        }

        private void DeletePathMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Config.GetInstance().Workflows.Remove(((Workflow) WorkflowListBox.SelectedItem));
        }

        private void AddWorkflowButton_Click(object sender, RoutedEventArgs e)
        {
            Workflow wf = new Workflow();
            wf.IsEnabled = false;
            Config.GetInstance().Workflows.Add(wf);
            WorkflowListBox.SelectedIndex = Config.GetInstance().Workflows.Count - 1;
        }
    }
}
