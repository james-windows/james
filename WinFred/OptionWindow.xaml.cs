using System.ComponentModel;
using System.Threading;
using System.Windows;
using MahApps.Metro.Controls;

namespace James
{
    /// <summary>
    ///     Interaction logic for OptionWindow.xaml
    /// </summary>
    public partial class OptionWindow : MetroWindow
    {
        public OptionWindow()
        {
            InitializeComponent();
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            AboutFlyout.IsOpen = !AboutFlyout.IsOpen;
        }

        private void TabChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) => Config.Instance.Persist();

        private void MetroWindow_Closing(object sender, CancelEventArgs e) => Config.Instance.Persist();
    }
}