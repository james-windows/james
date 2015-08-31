using System.Windows;
using System.Windows.Controls;

namespace WinFred.UserControls
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
                    Version = "v" + AppDomain.CurrentDomain.BaseDirectory.Split('-')[1];
            #endif
            versionLabel.Content = Version;
        }

        public string Version { get; private set;  } = "v0.1.1";

        private void DisplayChangelog(object sender, RoutedEventArgs e)
        {
            var changelogWindow = new ChangelogWindow();
            changelogWindow.ShowDialog();
        }
    }
}