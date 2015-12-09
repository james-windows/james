using System.Linq;
using System.Windows;
using James.Workflows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace James.WorkflowEditor
{
    /// <summary>
    ///     Interaction logic for ComponentMetroDialog.xaml
    /// </summary>
    public partial class ComponentMetroDialog : BaseMetroDialog
    {
        private readonly WorkflowComponent _component;

        public ComponentMetroDialog(WorkflowComponent component)
        {
            InitializeComponent();
            _component = component;
        }

        private void BaseMetroDialog_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = _component;
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

        private void Close()
        {
            _component.ParentWorkflow.Persist();
            var window = (MetroWindow) Window.GetWindow(this);
            window.HideMetroDialogAsync(this);
        }
    }
}