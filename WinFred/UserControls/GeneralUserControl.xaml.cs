using System;
using System.Windows;
using System.Windows.Controls;
using James.Enumerations;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Squirrel;

namespace James.UserControls
{
    /// <summary>
    ///     Interaction logic for GeneralUserControl.xaml
    /// </summary>
    public partial class GeneralUserControl : UserControl
    {
        public GeneralUserControl()
        {
            InitializeComponent();
            DataContext = Config.GetInstance();
            AccentColorComboBox.ItemsSource = Enum.GetNames(typeof (AccentColorTypes));
        }

        private async void UninstallProgram(object sender, RoutedEventArgs e)
        {
            var manager = new UpdateManager(Config.GetInstance().ReleaseUrl);
            await manager.FullUninstall();
            Environment.Exit(0);
        }

        private void CloseProgram(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private async void ResetConfig(object sender, RoutedEventArgs e)
        {
            var parentWindow = (MetroWindow) Window.GetWindow(this);
            var setting = new MetroDialogSettings
            {
                NegativeButtonText = "Cancel",
                AffirmativeButtonText = "Yes, I'm sure!"
            };
            var result =
                await
                    parentWindow.ShowMessageAsync("Reset config",
                        "Are you sure that you want to reset your config? All edits in the config will be permanently removed!",
                        MessageDialogStyle.AffirmativeAndNegative, setting);
            if (MessageDialogResult.Affirmative == result)
            {
                Config.GetInstance().ResetConfig();
                DataContext = Config.GetInstance();
            }
        }

        private void LaunchWelcomeWindow(object sender, RoutedEventArgs e)
        {
            new WelcomeWindow(false).Show();
        }
    }
}