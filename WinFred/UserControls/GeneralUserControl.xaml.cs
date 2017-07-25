using System;
using System.Windows;
using System.Windows.Controls;
using James.HelperClasses;

namespace James.UserControls
{
    /// <summary>
    ///     Interaction logic for GeneralUserControl.xaml
    /// </summary>
    public partial class GeneralUserControl : UserControl
    {
        private bool _startup = false;

        public GeneralUserControl()
        {
            InitializeComponent();
            SetStartup();
        }

        public bool Startup
        {
            get { return _startup; }
            set
            {
                SetStartupStatus(value);
                _startup = value;
            }
        }

        private async void SetStartupStatus(bool status)
        {
            var startupTask = await Windows.ApplicationModel.StartupTask.GetAsync("JamesStartupTask");
            if(status)
            {
                startupTask.RequestEnableAsync();
            }
            else
            {
                startupTask.Disable();
            }
        }

        private async void SetStartup()
        {
            try
            {
                var startupTask = await Windows.ApplicationModel.StartupTask.GetAsync("JamesStartupTask");
                _startup = startupTask.State == Windows.ApplicationModel.StartupTaskState.Enabled;
            }
            catch(Exception e)
            {
                //ignore buggy startuptask
            }
            
        }

        /// <summary>
        /// Uninstalls the application if it was deliverd by using squirrel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UninstallProgram(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("must be fixed!");
            Environment.Exit(0);
        }

        private void CloseProgram(object sender, RoutedEventArgs e) => Environment.Exit(0);

        /// <summary>
        /// Resets the config
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ResetConfig(object sender, RoutedEventArgs e)
        {
            (await MetroDialogHelper.ShowDialog(this, "Reset config", "Are you sure that you want to reset your config? All edits in the config will be permanently removed!"))
            .OnSuccess(() =>
            {
                Config.Instance.ResetConfig();
                DataContext = Config.Instance;
            });
        }

        private void LaunchWelcomeWindow(object sender, RoutedEventArgs e) => new MyWindows.WelcomeWindow(false).Show();

        private void UserControl_Loaded(object sender, RoutedEventArgs e) => DataContext = Config.Instance;
    }
}