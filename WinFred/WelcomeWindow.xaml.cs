using System.Windows;
using MahApps.Metro.Controls;

namespace James
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

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (_launchMainWindowAtEnd)
            {
                var window = new MainWindow(true);
                window.Show();
            }
            Close();
        }
    }
}