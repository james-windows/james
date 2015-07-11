using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;

namespace WinFred.OptionControls
{
    /// <summary>
    /// Interaction logic for SearchUserControl.xaml
    /// </summary>
    public partial class SearchUserControl : UserControl
    {
        public SearchUserControl()
        {
            InitializeComponent();
            PathListBox.ItemsSource = Config.GetInstance().Paths;
            FileExtensionsDataGrid.ItemsSource = Config.GetInstance().DefaultFileExtensions;
        }

        private void RebuildIndexButton_Click(object sender, RoutedEventArgs e)
        {
            Task tmp = new Task(() => SearchEngine.GetInstance().BuildIndex());
            tmp.Start();
        }

        private void AddFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.ShowDialog();
            if (dialog.SelectedPath != "")
            {
                Config.GetInstance().Paths.Add(new Path() { Location = dialog.SelectedPath });
                Config.GetInstance().Persist();
            }
        }

        private async void RemoveFolderButton_Click(object sender, RoutedEventArgs e)
        {
            MetroWindow parentWindow = (MetroWindow)Window.GetWindow(this);
            MetroDialogSettings setting = new MetroDialogSettings(){NegativeButtonText = "Cancel", AffirmativeButtonText = "Yes, I'm sure!"};
            MessageDialogResult result = await parentWindow.ShowMessageAsync("Delete Path", "Are you sure?", MessageDialogStyle.AffirmativeAndNegative, setting);
            if (MessageDialogResult.Affirmative == result)
            {
                Config.GetInstance().Paths.Remove((Path)PathListBox.SelectedItem);
                Config.GetInstance().Persist();
            }
        }

        private void ChangeStatusMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ((Path)PathListBox.SelectedItem).IsEnabled = !((Path)PathListBox.SelectedItem).IsEnabled;
        }
    }
}
