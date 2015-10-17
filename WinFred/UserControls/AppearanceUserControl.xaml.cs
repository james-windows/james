using System;
using System.Windows.Controls;
using James.Enumerations;

namespace James.UserControls
{
    /// <summary>
    ///     Interaction logic for CustomizeUserControl.xaml
    /// </summary>
    public partial class AppearanceUserControl : UserControl
    {
        public AppearanceUserControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            AccentColorComboBox.ItemsSource = Enum.GetNames(typeof(AccentColorTypes));
            DataContext = Config.Instance;
        }
    }
}