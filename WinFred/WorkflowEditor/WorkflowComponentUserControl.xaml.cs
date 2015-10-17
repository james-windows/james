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
using James.Workflows;
using James.Workflows.Outputs;
using James.Workflows.Triggers;

namespace James.WorkflowEditor
{
    /// <summary>
    /// Interaction logic for WorkflowComponentUserControl.xaml
    /// </summary>
    public partial class WorkflowComponentUserControl : UserControl
    {
        public WorkflowComponentUserControl(WorkflowComponent workflowComponent)
        {
            InitializeComponent();
            DataContext = workflowComponent;
            if (workflowComponent is BasicOutput)
            {
                rightAnchor.Visibility = Visibility.Hidden;
            }
        }

        public delegate void ComponentUpdateHandler(object sender);
        public event ComponentUpdateHandler OnUpdate;

        private void RemoveComponent(object sender, RoutedEventArgs e)
        {
            var component = (WorkflowComponent)((MenuItem)sender).DataContext;
            component.ParentWorkflow.RemoveComponent(component);
            OnUpdate?.Invoke(this);
        }

        public void OpenComponent()
        {
            var window = new ComponentWindow((WorkflowComponent) DataContext) {Owner = Window.GetWindow(this)};
            window.ShowDialog();
        }

        private void EditComponent(object sender, RoutedEventArgs e) => OpenComponent();

        private void OpenWorkflowFolder(object sender, RoutedEventArgs e) => ((WorkflowComponent) DataContext).ParentWorkflow.OpenFolder();
    }
}
