using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using James.ResultItems;
using James.Workflows;
using MahApps.Metro.Controls;

namespace James.MyWindows
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
            BindEvents();
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
                ShowWindow();
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
                    ClearInputAtUsersDesire();
                }
                else
                {
                    SearchTextBox.Text = shortcut.Action;
                }
                SearchTextBox.SelectAll();
                ShowWindow();
            }
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
                    HideWindow();
                    break;
            }
            HandleShortcuts(e);
        }

        private void HandleShortcuts(KeyEventArgs e)
        {
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
        /// Clears Input if user specifies this behavoir in the settings
        /// </summary>
        private void ClearInputAtUsersDesire()
        {
            if (Config.Instance.AlwaysClearLastInput)
            {
                SearchTextBox.Text = "";
            }
        }

        /// <summary>
        /// Opens LargeType and brings it to the front
        /// </summary>
        private void CallLargeType()
        {
            var resultItem = searchResultControl.FocusedIndex < searchResultControl.results.Count ? searchResultControl.results[searchResultControl.FocusedIndex] as MagicResultItem: null;
            string message = resultItem?.Title ?? SearchTextBox.Text;
            if (message.Trim().Length != 0)
            {
                DisplayLargeType(message);
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
        private bool HotKeyPressed(HotKey hotkey, KeyEventArgs e) => e.KeyboardDevice.Modifiers == hotkey.ModifierKeys && e.KeyboardDevice.IsKeyDown(hotkey.Key);

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
        /// Opens the settings window and hides james
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenSettings(object sender, RoutedEventArgs e) => new OptionWindow().Show();

        /// <summary>
        /// Closes the whole application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseApplication(object sender, RoutedEventArgs e) => Environment.Exit(1);

        #endregion
    }
}