using System.Windows;
using MahApps.Metro.Controls;

namespace James.Windows
{
    /// <summary>
    ///     Interaction logic for WelcomeWindow.xaml
    /// </summary>
    public partial class WelcomeWindow : MetroWindow
    {
        private readonly bool _launchMainWindowAtEnd;

        public WelcomeWindow(bool launchMainWindowAtEnd = true)
        {
            _launchMainWindowAtEnd = launchMainWindowAtEnd;
            InitializeComponent();
        }

        /// <summary>
        /// Launches the search windows if the tutorial is completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (_launchMainWindowAtEnd)
            {
                MainWindow.GetInstance(true).Show();
            }
            Close();
        }
    }
}