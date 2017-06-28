using System.Windows;
using System.Windows.Controls;

namespace James.UserControls
{
    /// <summary>
    ///     Interaction logic for AboutUserControl.xaml
    /// </summary>
    public partial class AboutUserControl : UserControl
    {
        public AboutUserControl()
        {
            InitializeComponent();
            versionLabel.Content = Version;
        }

        public string Version { get; } = "v0.1.1";

        /// <summary>
        /// Opens the window for displaying a changelog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisplayChangelog(object sender, RoutedEventArgs e)
        {
            var changelogWindow = new MyWindows.ChangelogWindow {Owner = Window.GetWindow(this)};
            changelogWindow.ShowDialog();
        }
    }
}