using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace James.UserControls.SearchConfiguration
{
    /// <summary>
    /// Interaction logic for ExcludedFolders.xaml
    /// </summary>
    public partial class ExcludedFolders : UserControl
    {
        public ExcludedFolders()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Provides the option to add an excluded folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AddExcludedFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var parentWindow = (MetroWindow)Window.GetWindow(this);
            var result =
                await parentWindow.ShowInputAsync("Add excluded folder", "Which folders should be excluded?");
            if (result != null)
            {
                Config.Instance.ExcludedFolders.Add(result.Trim());
                Config.Instance.Persist();
            }
        }

        /// <summary>
        /// Provides the option to add an excluded folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveExcludedFolderButton_Click(object sender, RoutedEventArgs e)
        {
            Config.Instance.ExcludedFolders.Remove(IgnoredFolderListBox.SelectedItem as string);
            Config.Instance.Persist();
        }

        /// <summary>
        /// Provides the option to to remove a folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveFolderButton_Click(object sender, RoutedEventArgs e)
        {
            Config.Instance.ExcludedFolders.Remove(IgnoredFolderListBox.SelectedItem as string);
            Config.Instance.Persist();
        }
    }
}
