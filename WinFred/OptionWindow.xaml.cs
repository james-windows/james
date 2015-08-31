using System.ComponentModel;
using System.Threading;
using System.Windows;
using MahApps.Metro.Controls;

namespace WinFred
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

        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            Config.GetInstance().Persist();
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            AboutFlyout.IsOpen = !AboutFlyout.IsOpen;
        }
    }
}