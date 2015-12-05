using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using James.Search;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using UserControl = System.Windows.Controls.UserControl;

namespace James.UserControls
{
    /// <summary>
    ///     Interaction logic for SearchUserControl.xaml
    /// </summary>
    public partial class SearchUserControl : UserControl
    {
        private ProgressDialogController _controller;

        public SearchUserControl()
        {
            InitializeComponent();
        }

        private async void RebuildIndexButton_Click(object sender, RoutedEventArgs e)
        {
            var parentWindow = (MetroWindow) Window.GetWindow(this);
            var setting = new MetroDialogSettings
            {
                NegativeButtonText = "Cancel",
                AffirmativeButtonText = "Yes, I'm sure!"
            };
            var result =
                await
                    parentWindow.ShowMessageAsync("Rebuilding file index",
                        "Do you really want to rebuild your index, this may take a few minutes?",
                        MessageDialogStyle.AffirmativeAndNegative, setting);
            if (MessageDialogResult.Affirmative == result)
            {
                BuildIndexInTheBg();
            }
        }

        private async void BuildIndexInTheBg()
        {
            var parentWindow = (MetroWindow) Window.GetWindow(this);
            SearchEngine.Instance.ChangedBuildingIndexProgress += ChangedBuildingIndexProgress;
            _controller = await parentWindow.ShowProgressAsync("Building index...", "Browsing through your files...");
            _controller.SetIndeterminate();
            var tmp = new Task(() => SearchEngine.Instance.BuildIndex());
            tmp.Start();
        }

        private void ChangedBuildingIndexProgress(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 100)
            {
                _controller.CloseAsync();
            }
            _controller.SetProgress(e.ProgressPercentage/100.0);
            _controller.SetMessage(e.ProgressPercentage + "% of the files already added!");
        }

        private void AddFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            dialog.ShowDialog();
            if (dialog.SelectedPath != "")
            {
                Path newPath = new Path {Location = dialog.SelectedPath};
                Config.Instance.Paths.Add(newPath);
                Config.Instance.Persist();
                MyFileWatcher.Instance.AddPath(newPath);
            }
        }

        private async void RemoveFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var parentWindow = (MetroWindow) Window.GetWindow(this);
            var setting = new MetroDialogSettings
            {
                NegativeButtonText = "Cancel",
                AffirmativeButtonText = "Yes, I'm sure!"
            };
            var result =
                await
                    parentWindow.ShowMessageAsync("Delete Path", "Are you sure?",
                        MessageDialogStyle.AffirmativeAndNegative, setting);
            if (MessageDialogResult.Affirmative == result)
            {
                var oldPath = (Path) PathListBox.SelectedItem;
                Config.Instance.Paths.Remove(oldPath);
                Config.Instance.Persist();
                MyFileWatcher.Instance.RemovePath(oldPath);
            }
        }

        private void ChangeStatusMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ((Path) PathListBox.SelectedItem).IsEnabled = !((Path) PathListBox.SelectedItem).IsEnabled;
        }

        private void SearchUserControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = Config.Instance;
        }

        private void DeselectPath(object sender, RoutedEventArgs e) => PathListBox.UnselectAll();
    }
}