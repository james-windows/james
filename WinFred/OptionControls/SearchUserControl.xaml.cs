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
            SearchEngine.GetInstance().BuildIndex();
        }

        private void AddFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            Config.GetInstance().Paths.Add(new Path() { Location = dialog.SelectedPath });
            Config.GetInstance().Persist();
        }

        private void RemoveFolderButton_Click(object sender, RoutedEventArgs e)
        {
            Config.GetInstance().Paths.Remove((Path)PathListBox.SelectedItem);
            Config.GetInstance().Persist();
        }
    }
}
