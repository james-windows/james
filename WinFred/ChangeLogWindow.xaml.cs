using System.Net;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls;

namespace WinFred
{
    /// <summary>
    ///     Interaction logic for ChangelogWindow.xaml
    /// </summary>
    public partial class ChangelogWindow : MetroWindow
    {
        public ChangelogWindow()
        {
            InitializeComponent();
        }

        private async void ChangeLogWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ChangelogTextBlock.Text = await LoadChangelog();
            }
            catch (WebException)
            {
                ChangelogTextBlock.Text = "No internet connection available";
            }
            progressRing.Visibility = Visibility.Hidden;
        }

        private async Task<string> LoadChangelog()
        {
            using (var webClient = new WebClient())
            {
                return await webClient.DownloadStringTaskAsync(Config.GetInstance().ReleaseUrl + @"/changelog.txt");
            }
        }

        private void CloseWindow(object sender, RoutedEventArgs e) => Close();
        private void CopyToClipboard(object sender, RoutedEventArgs e) => Clipboard.SetText(ChangelogTextBlock.Text);
    }
}