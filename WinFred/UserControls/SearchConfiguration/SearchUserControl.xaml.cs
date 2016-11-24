using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using James.Search;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using UserControl = System.Windows.Controls.UserControl;

namespace James.UserControls.SearchConfiguration
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
            DataContext = Config.Instance;
        }

        /// <summary>
        /// Starts saving the current status of the index
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SaveIndexButton_Click(object sender, RoutedEventArgs e)
        {
            SearchEngine.Instance.SaveIndex();
        }

        /// <summary>
        /// Starts a recreation of the index
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void RebuildIndexButton_Click(object sender, RoutedEventArgs e)
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

        /// <summary>
        /// Asks the user if he is sure about that and starts to rebuild the index
        /// </summary>
        private async void BuildIndexInTheBg()
        {
            var parentWindow = (MetroWindow) Window.GetWindow(this);
            SearchEngine.Instance.ChangedBuildingIndexProgress += ChangedBuildingIndexProgress;
            _controller = await parentWindow.ShowProgressAsync("Building index...", "Browsing through your files...");
            _controller.SetIndeterminate();
            var tmp = new Task(() => SearchEngine.Instance.BuildIndex());
            tmp.Start();
        }

        /// <summary>
        /// Displays the progress of building the index to the gui
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangedBuildingIndexProgress(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 100)
            {
                try
                {
                    _controller.CloseAsync();
                }
                catch (Exception){}
            }
            _controller.SetProgress(e.ProgressPercentage/100.0);
            _controller.SetMessage(e.ProgressPercentage + "% of the files already added!");
        }

        /// <summary>
        /// Adds a folder to the user specific config
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            dialog.ShowDialog();
            if (dialog.SelectedPath != "" && Config.Instance.Paths.All(path => path.Location != dialog.SelectedPath))
            {
                var newPath = new Path {Location = dialog.SelectedPath};
                Config.Instance.Paths.Add(newPath);
                Config.Instance.Persist();
                MyFileWatcher.Instance.AddPath(newPath);
            }
        }

        /// <summary>
        /// Removes a folder of the user specifc config
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Toggles the Enabled status of the path
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeStatusMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ((Path) PathListBox.SelectedItem).IsEnabled = !((Path) PathListBox.SelectedItem).IsEnabled;
        }

        private void DeselectPath(object sender, RoutedEventArgs e) => PathListBox.UnselectAll();
    }
}