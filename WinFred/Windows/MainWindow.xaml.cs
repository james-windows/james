using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using James.ResultItems;
using James.Shortcut;
using James.Workflows;
using MahApps.Metro.Controls;
using Microsoft.Win32;

namespace James.Windows
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
            Loaded += SetPosition;
            SystemEvents.DisplaySettingsChanged += SetPosition;
            ShortcutManager.Instance.ShortcutPressed += (sender, args) => OnHotKeyHandler(sender as Shortcut.Shortcut);
            LargeType.Instance.Deactivated += LargeType_Deactivated;
            LargeType.Instance.Activated += LargeType_Activated;
        }

        private void SetPosition(object sender, EventArgs e)
        {
            var desktopWorkingArea = SystemParameters.WorkArea;
            this.Left = (desktopWorkingArea.Width - this.Width) / 2;
            this.Top = desktopWorkingArea.Height / 4;
        }

        public static MainWindow GetInstance(bool showOnStartup = false)
        {
            lock (SingeltonLock)
            {
                return _mainWindow ?? (_mainWindow = new MainWindow(showOnStartup));
            }
        }


        private void LargeType_Activated(object sender, EventArgs e) => _showLargeType = false;

        /// <summary>
        /// Bring the search window to the focus when LargeType gets closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Reacts for the hotkey of the search window and shows or hides it depending on the current state
        /// </summary>
        /// <param name="shortcut"></param>
        public void OnHotKeyHandler(Shortcut.Shortcut shortcut)
        {
            if (IsVisible || shortcut == null)
            {
                HideWindow();
            }
            else
            {
                if (Equals(shortcut.HotKey, Config.Instance.ShortcutManagerSettings.JamesHotKey.HotKey))
                {
                    if (Config.Instance.AlwaysClearLastInput)
                    {
                        SearchTextBox.Text = "";
                    }
                }
                else
                {
                    SearchTextBox.Text = shortcut.Action;
                }
                SearchTextBox.SelectAll();
                Show();
                Activate();
                SearchTextBox.Focus();
            }
        }

        /// <summary>
        /// Hides search window
        /// </summary>
        public void HideWindow()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                LargeType.Instance.Hide();
                if (Config.Instance.AlwaysClearLastInput)
                {
                    SearchTextBox.Text = "";
                }
                Hide();
            });
        }

        #region Window-Events

        /// <summary>
        /// Some key listeners for special events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    searchResultControl.Open(e, SearchTextBox.Text, e.KeyboardDevice.IsKeyDown(Key.RightAlt));
                    break;
                case Key.Tab:
                    string text = searchResultControl.AutoComplete();
                    if (text != null)
                    {
                        SearchTextBox.Text = text;
                        SearchTextBox.SelectionStart = SearchTextBox.Text.Length;
                    }
                    break;
                case Key.Escape:
                    Hide();
                    if (Config.Instance.AlwaysClearLastInput)
                    {
                        SearchTextBox.Text = "";
                    }
                    break;
            }
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                if (e.KeyboardDevice.IsKeyDown(Key.Up))
                {
                    searchResultControl.IncreasePriority();
                    e.Handled = true;
                }
                else if (e.KeyboardDevice.IsKeyDown(Key.Down))
                {
                    searchResultControl.DecreasePriority();
                    e.Handled = true;
                }
            }
            var shortcutSettings = Config.Instance.ShortcutManagerSettings;
            var largeTypeHotKey = shortcutSettings.LargeTypeHotKey.HotKey;
            var settingsHotKey = shortcutSettings.SettingsHotKey.HotKey;
            if (HotKeyPressed(largeTypeHotKey, e))
            {
                CallLargeType();
            }
            else if (HotKeyPressed(settingsHotKey, e))
            {
                new OptionWindow().Show();
                HideWindow();
            }
        }

        /// <summary>
        /// Opens LargeType and brings it to the front
        /// </summary>
        private void CallLargeType()
        {
            int index = searchResultControl.FocusedIndex;
            if (index < searchResultControl.results?.Count)
            {
                var resultItem = searchResultControl.results[index] as MagicResultItem;
                if (resultItem != null)
                {
                    DisplayLargeType(resultItem.Title);
                }
                else if (SearchTextBox.Text.Trim().Length > 0)
                {
                    DisplayLargeType(SearchTextBox.Text);
                }
            }
            else if (SearchTextBox.Text.Trim().Length > 0)
            {
                DisplayLargeType(SearchTextBox.Text);
            }
        }

        /// <summary>
        /// Opens LargeType and shows the providen message
        /// </summary>
        /// <param name="message"></param>
        public void DisplayLargeType(string message)
        {
            _showLargeType = true;
            LargeType.Instance.DisplayMessage(message);
        }

        /// <summary>
        /// Tests if the key and the modifiers are pressed
        /// </summary>
        /// <param name="hotkey"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool HotKeyPressed(HotKey hotkey, KeyEventArgs e)
        {
            return e.KeyboardDevice.IsKeyDown(hotkey.Key) && e.KeyboardDevice.Modifiers == hotkey.ModifierKeys;
        }

        /// <summary>
        /// Starts a new search on TextChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var str = SearchTextBox.Text.Trim();
            new Task(() => searchResultControl.Search(str)).Start();
        }

        /// <summary>
        /// Hides current window if it's no longer in the focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Deactivated(object sender, EventArgs e)
        {
            if (!_showLargeType)
            {
                OnHotKeyHandler(null);
            }
        }

        /// <summary>
        /// Opens the settings window and hides james
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            new OptionWindow().Show();
            HideWindow();
        }

        /// <summary>
        /// Closes the hole application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseApplication(object sender, RoutedEventArgs e) => Environment.Exit(1);

        #endregion
    }
}