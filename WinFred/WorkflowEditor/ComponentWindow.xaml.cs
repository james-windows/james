using System.Linq;
using System.Windows;
using James.Workflows;
using MahApps.Metro.Controls;

namespace James.WorkflowEditor
{
    /// <summary>
    ///     Interaction logic for ComponentWindow.xaml
    /// </summary>
    public partial class ComponentWindow : MetroWindow
    {
        private readonly WorkflowComponent _component;

        public ComponentWindow(WorkflowComponent component)
        {
            InitializeComponent();
            this._component = component;
            DataContext = component;
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var prop in _component.GetType().GetProperties())
            {
                var attrs = prop.GetCustomAttributes(true);
                foreach (var componentFieldAttribute in attrs.OfType<ComponentFieldAttribute>())
                {
                    propertyPanel.Children.Add(componentFieldAttribute.GetElement(prop, _component));
                }
            }
        }

        private void DiscardChanges(object sender, RoutedEventArgs e) => Close();

        private void OpenWorkflowDirectory(object sender, RoutedEventArgs e) => _component.ParentWorkflow.OpenFolder();

        private void SaveChanges(object sender, RoutedEventArgs e)
        {
            foreach (var result in propertyPanel.Children.Cast<ComponentPropertyUserControl>())
            {
                result.WriteToWorkflowComponent();
            }
            Close();
        }
    }
}