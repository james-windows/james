using System;
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
#if !DEBUG 
                if(AppDomain.CurrentDomain.BaseDirectory.Split('-').Length == 2)
                    Version = "v" + AppDomain.CurrentDomain.BaseDirectory.Split('-')[1].Replace("\\","");
            #endif
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
            var changelogWindow = new Windows.ChangelogWindow {Owner = Window.GetWindow(this)};
            changelogWindow.ShowDialog();
        }
    }
}