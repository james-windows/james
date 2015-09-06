using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using WinFred.Enumerations;

namespace WinFred.UserControls
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

        private void UninstallProgram(object sender, RoutedEventArgs e)
        {
            Process.Start(Environment.SystemDirectory + @"\appwiz.cpl");
        }

        private void CloseProgram(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private async void ResetConfig(object sender, RoutedEventArgs e)
        {
            var parentWindow = (MetroWindow)Window.GetWindow(this);
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
    }
}