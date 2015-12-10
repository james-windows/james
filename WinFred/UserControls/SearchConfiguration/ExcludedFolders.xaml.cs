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
        private async void AddExcludedFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var parentWindow = (MetroWindow)Window.GetWindow(this);
            var result =
                await parentWindow.ShowInputAsync("Add excluded folder", "Which folders should be excluded?");
            Config.Instance.ExcludedFolders.Add(result.Trim());
            Config.Instance.Persist();
        }

        private void RemoveExcludedFolderButton_Click(object sender, RoutedEventArgs e)
        {
            Config.Instance.ExcludedFolders.Remove(IgnoredFolderListBox.SelectedItem as string);
            Config.Instance.Persist();
        }

        private void RemoveFolderButton_Click(object sender, RoutedEventArgs e)
        {
            Config.Instance.ExcludedFolders.Remove(IgnoredFolderListBox.SelectedItem as string);
            Config.Instance.Persist();
        }
    }
}
