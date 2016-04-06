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

        /// <summary>
        /// Builds the dialog using reflection on the component
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Saves every attribute of the component
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveChanges(object sender, RoutedEventArgs e)
        {
            foreach (var result in propertyPanel.Children.Cast<ComponentPropertyUserControl>())
            {
                result.WriteToWorkflowComponent();
            }
            Close();
        }

        /// <summary>
        /// Persists the Workflow before closing
        /// </summary>
        private void Close()
        {
            _component.ParentWorkflow.Persist();
            var window = (MetroWindow) Window.GetWindow(this);
            window.HideMetroDialogAsync(this);
        }
    }
}