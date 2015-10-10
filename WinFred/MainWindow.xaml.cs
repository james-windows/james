using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace James
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool showLargeType;

        public MainWindow(bool showOnStartup = false)
        {
            if (!showOnStartup)
            {
                Visibility = Visibility.Hidden;
            }
            InitializeComponent();
            new HotKey(Key.Space, KeyModifier.Alt, OnHotKeyHandler);
            LargeType.GetInstance().Deactivated += LargeType_Deactivated;
            LargeType.GetInstance().Activated += LargeType_Activated;
        }

        private void LargeType_Activated(object sender, EventArgs e)
        {
            showLargeType = false;
        }

        private void LargeType_Deactivated(object sender, EventArgs e)
        {
            if (!Keyboard.IsKeyDown(Key.Escape))
            {
                HideWindow();
            }
            else
            {
                Show();
                Activate();
                SearchTextBox.Focus();
            }
        }

        private void OnHotKeyHandler(HotKey hotKey)
        {
            if (IsVisible || hotKey == null)
            {
                HideWindow();
            }
            else
            {
                SearchTextBox.Text = "";
                Show();
                Activate();
                SearchTextBox.Focus();
            }
        }

        private void HideWindow()
        {
            LargeType.GetInstance().Hide();
            SearchTextBox.Text = "";
            Hide();
        }

        #region Window-Events

        private void SearchTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Down:
                    searchResultControl.MoveDown();
                    break;
                case Key.Up:
                    searchResultControl.MoveUp();
                    break;
                case Key.Enter:
                    searchResultControl.Open(e);
                    break;
                case Key.Escape:
                    Hide();
                    SearchTextBox.Text = "";
                    break;
            }
            if (e.KeyboardDevice.IsKeyDown(Key.L) && e.KeyboardDevice.IsKeyDown(Key.LeftAlt) &&
                SearchTextBox.Text.Length > 0)
            {
                DisplayLargeType(SearchTextBox.Text);
            }
            else if (e.KeyboardDevice.IsKeyDown(Key.S) && e.KeyboardDevice.IsKeyDown(Key.LeftAlt))
            {
                new OptionWindow().Show();
                HideWindow();
            }
        }

        private void DisplayLargeType(string message)
        {
            LargeType.GetInstance().Message = message;
            showLargeType = true;
            LargeType.GetInstance().Hide();
            LargeType.GetInstance().Show();
            LargeType.GetInstance().Activate();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var str = SearchTextBox.Text.Trim();
            new Task(() => searchResultControl.Search(str)).Start();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            if (!showLargeType)
            {
                OnHotKeyHandler(null);
            }
        }

        #endregion
    }
}