using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using WinFred.Enumerations;

namespace WinFred.UserControls
{
    /// <summary>
    ///     Interaction logic for GeneralUserControl.xaml
    /// </summary>
    public partial class GeneralUserControl : UserControl
    {
        public GeneralUserControl()
        {
            InitializeComponent();
            DataContext = Config.GetInstance();
            AccentColorComboBox.ItemsSource = Enum.GetNames(typeof (AccentColorTypes));
        }

        private void UninstallProgram(object sender, RoutedEventArgs e)
        {
            Process.Start(Environment.SystemDirectory + @"\appwiz.cpl");
        }

        private void CloseProgram(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}