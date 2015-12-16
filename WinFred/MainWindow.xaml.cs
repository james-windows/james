using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using James.Workflows;

namespace James
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static MainWindow _mainWindow;
        private static readonly object SingeltonLock = new object();
        private bool _showLargeType;

        public MainWindow(bool showOnStartup)
        {
            if (!showOnStartup)
            {
                Visibility = Visibility.Hidden;
            }
            InitializeComponent();
            new HotKey(Key.Space, KeyModifier.Alt, OnHotKeyHandler);
            LargeType.Instance.Deactivated += LargeType_Deactivated;
            LargeType.Instance.Activated += LargeType_Activated;
        }

        public static MainWindow GetInstance(bool showOnStartup = false)
        {
            lock (SingeltonLock)
            {
                return _mainWindow ?? (_mainWindow = new MainWindow(showOnStartup));
            }
        }

        private void LargeType_Activated(object sender, EventArgs e)
        {
            _showLargeType = false;
        }

        private void LargeType_Deactivated(object sender, EventArgs e)
        {
            WorkflowManager.Instance.CancelWorkflows();
            if (!Keyboard.IsKeyDown(Key.Escape) && !Keyboard.IsKeyDown(Key.L) && !Keyboard.IsKeyDown(Key.LeftAlt))
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
                if (Config.Instance.AlwaysClearLastInput)
                {
                    SearchTextBox.Text = "";
                }
                SearchTextBox.SelectAll();
                Show();
                Activate();
                SearchTextBox.Focus();
            }
        }

        private void HideWindow()
        {
            LargeType.Instance.Hide();
            if (Config.Instance.AlwaysClearLastInput)
            {
                SearchTextBox.Text = "";
            }
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
                    WorkflowManager.Instance.CancelWorkflows();
                    searchResultControl.Open(e);
                    break;
                case Key.Escape:
                    Hide();
                    if (Config.Instance.AlwaysClearLastInput)
                    {
                        SearchTextBox.Text = "";
                    }
                    break;
            }
            if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl))
            {
                if (e.KeyboardDevice.IsKeyDown(Key.Up))
                {
                    searchResultControl.IncreasePriority();
                }
                else if (e.KeyboardDevice.IsKeyDown(Key.Down))
                {
                    searchResultControl.DecreasePriority();
                }
            }
            if (e.KeyboardDevice.IsKeyDown(Key.L) && e.KeyboardDevice.IsKeyDown(Key.LeftAlt) &&
                SearchTextBox.Text.Trim().Length > 0)
            {
                DisplayLargeType(SearchTextBox.Text);
            }
            else if (e.KeyboardDevice.IsKeyDown(Key.S) && e.KeyboardDevice.IsKeyDown(Key.LeftAlt))
            {
                new OptionWindow().Show();
                HideWindow();
            }
        }

        public void DisplayLargeType(string message)
        {
            _showLargeType = true;
            LargeType.Instance.DisplayMessage(message);
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var str = SearchTextBox.Text.Trim();
            new Task(() => searchResultControl.Search(str)).Start();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            if (!_showLargeType)
            {
                OnHotKeyHandler(null);
            }
        }

        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            new OptionWindow().Show();
            HideWindow();
        }

        private void CloseApplication(object sender, RoutedEventArgs e) => Environment.Exit(1);

        #endregion
    }
}