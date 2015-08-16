using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shell;
using WinFred.Search;

namespace WinFred.UserControls
{
    /// <summary>
    /// Interaction logic for SearchUserControl.xaml
    /// </summary>
    public partial class SearchUserControl : UserControl
    {
        public SearchUserControl()
        {
            InitializeComponent();
            this.DataContext = Config.GetInstance();
        }
        private ProgressDialogController _controller;

        private async void RebuildIndexButton_Click(object sender, RoutedEventArgs e)
        {
            MetroWindow parentWindow = (MetroWindow)Window.GetWindow(this);
            MetroDialogSettings setting = new MetroDialogSettings() { NegativeButtonText = "Cancel", AffirmativeButtonText = "Yes, I'm sure!" };
            MessageDialogResult result = await parentWindow.ShowMessageAsync("Rebuilding file index", "Do you really want to rebuild your index, this may take a few minutes?", MessageDialogStyle.AffirmativeAndNegative, setting);
            if (MessageDialogResult.Affirmative == result)
            {
                BuildIndexInTheBg();
            }
        }

        private async void BuildIndexInTheBg()
        {
            MetroWindow parentWindow = (MetroWindow)Window.GetWindow(this);
            SearchEngine.GetInstance().ChangedBuildingIndexProgress += ChangedBuildingIndexProgress;
            _controller = await parentWindow.ShowProgressAsync("Building index...", "Browsing through your files...");
            _controller.SetIndeterminate();
            Task tmp = new Task(() => SearchEngine.GetInstance().BuildIndex());
            tmp.Start();
        }

        private void ChangedBuildingIndexProgress(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 100)
            {
                _controller.CloseAsync();
            }
            _controller.SetProgress(e.ProgressPercentage / 100.0);
            _controller.SetMessage(e.ProgressPercentage + "% of the files already added!");
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
