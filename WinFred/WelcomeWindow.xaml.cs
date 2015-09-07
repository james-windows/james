using System.Windows;
using MahApps.Metro.Controls;

namespace James
{
    /// <summary>
    ///     Interaction logic for WelcomeWindow.xaml
    /// </summary>
    public partial class WelcomeWindow : MetroWindow
    {
        public WelcomeWindow()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var window = new MainWindow(true);
            window.Show();
            Close();
        }
    }
}