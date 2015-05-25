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

        private void RemoveFolderButton_Click(object sender, RoutedEventArgs e)
        {
            Config.GetInstance().Paths.Remove((Path)PathListBox.SelectedItem);
            Config.GetInstance().Persist();
        }

        /// <summary>
        /// reverts the isEnabled Propertie
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeStatusMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (PathListBox.SelectedItem != null)
            {
                int index = Config.GetInstance().Paths.IndexOf((Path) PathListBox.SelectedItem);
                Config.GetInstance().Paths[index].IsEnabled = !((Path)PathListBox.SelectedItem).IsEnabled;
                Config.GetInstance().Persist();
            }
        }

        /// <summary>
        /// Deletes the current selected item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeletePathMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (PathListBox.SelectedItem != null)
            {
                Config.GetInstance().Paths.Remove((Path) PathListBox.SelectedItem);
                Config.GetInstance().Persist();
            }
        }
    }
}
