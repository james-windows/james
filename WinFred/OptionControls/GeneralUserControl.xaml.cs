using System;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using WinFred.Enumerations;

namespace WinFred.OptionControls
{
    /// <summary>
    /// Interaction logic for GeneralUserControl.xaml
    /// </summary>
    public partial class GeneralUserControl : UserControl
    {
        public GeneralUserControl()
        {
            InitializeComponent();
            this.DataContext = Config.GetInstance();
            AccentColorComboBox.ItemsSource = Enum.GetNames(typeof(AccentColorTypes));
        }
    }
}
