using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using James.HelperClasses;
using James.Search;

namespace James
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LargeType _largeType;

        public MainWindow(bool showOnStartup = false)
        {
            if (!showOnStartup)
            {
                Visibility = Visibility.Hidden;
            }
            InitializeComponent();
            new HotKey(Key.Space, KeyModifier.Alt, OnHotKeyHandler);
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
            _largeType?.Close();
            _largeType = null;
            Hide();
        }

        #region Window-Events

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.IsKeyDown(Key.Escape))
            {
                Hide();
                SearchTextBox.Text = "";
            }
            else if (e.KeyboardDevice.IsKeyDown(Key.L) && e.KeyboardDevice.IsKeyDown(Key.LeftAlt) &&
                     SearchTextBox.Text.Length > 0)
            {
                var message = SearchTextBox.Text;
                _largeType = new LargeType(message) {Owner = this};
                _largeType.ShowDialog();
                _largeType = null;
            }
            else if (e.KeyboardDevice.IsKeyDown(Key.S) && e.KeyboardDevice.IsKeyDown(Key.LeftAlt))
            {
                var window = new OptionWindow();
                window.ShowDialog();
                HideWindow();
            }
        }

        private void SearchTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.IsKeyDown(Key.Down))
            {
                searchResultControl.MoveDown();
            }
            else if (e.KeyboardDevice.IsKeyDown(Key.Up))
            {
                searchResultControl.MoveUp();
            }
            else if (e.KeyboardDevice.IsKeyDown(Key.Enter))
            {
                HideWindow();
                searchResultControl.Open(e);
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Focus();
        }

        private void ExecuteWorkflow(Workflow workflow, string str)
        {
            var tmp = DateTime.Now;
            var line = "";
            var process = workflow.GetProcess(str.Replace(workflow.Keyword, ""));
            process.Start();
            while (!process.StandardOutput.EndOfStream)
            {
                line += process.StandardOutput.ReadLine();
            }
            line = GeneralHelper.BuildHtml(line);
            line = line.Replace("suppldata", "\"table table-bordered table-striped\"");
            Dispatcher.BeginInvoke((Action) (() => OutputWebBrowser.NavigateToString(line)));
            Debug.WriteLine((DateTime.Now - tmp).TotalMilliseconds);
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var str = SearchTextBox.Text;
            var workflowExecuted = false;
            foreach (var item in Config.GetInstance().Workflows.Where(item => item.IsEnabled && str.StartsWith(item.Keyword)))
            {
                workflowExecuted = true;
                OutputWebBrowser.Visibility = Visibility.Visible;
                new Task(() => ExecuteWorkflow(item, str)).Start();
                return;
            }
            if (!workflowExecuted)
            {
                OutputWebBrowser.Navigate("about:blank");
                OutputWebBrowser.Visibility = Visibility.Collapsed;
                new Task(() => searchResultControl.Search(str)).Start();
            }
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            if (_largeType == null)
            {
                OnHotKeyHandler(null);
            }
        }

        #region region for hidding the window in the taskswitch

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var wndHelper = new WindowInteropHelper(this);
            var exStyle = (int) GetWindowLong(wndHelper.Handle, (int) GetWindowLongFields.GWL_EXSTYLE);
            exStyle |= (int) ExtendedWindowStyles.WS_EX_TOOLWINDOW;
            SetWindowLong(wndHelper.Handle, (int) GetWindowLongFields.GWL_EXSTYLE, (IntPtr) exStyle);
        }

        #region Window styles

        [Flags]
        public enum ExtendedWindowStyles
        {
            WS_EX_TOOLWINDOW = 0x00000080
        }

        public enum GetWindowLongFields
        {
            GWL_EXSTYLE = (-20)
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            var error = 0;
            var result = IntPtr.Zero;
            // Win32 SetWindowLong doesn't clear error on success
            SetLastError(0);

            if (IntPtr.Size == 4)
            {
                // use SetWindowLong
                var tempResult = IntSetWindowLong(hWnd, nIndex, IntPtrToInt32(dwNewLong));
                error = Marshal.GetLastWin32Error();
                result = new IntPtr(tempResult);
            }
            else
            {
                // use SetWindowLongPtr
                result = IntSetWindowLongPtr(hWnd, nIndex, dwNewLong);
                error = Marshal.GetLastWin32Error();
            }

            if ((result == IntPtr.Zero) && (error != 0))
            {
                throw new Win32Exception(error);
            }
            return result;
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
        private static extern IntPtr IntSetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
        private static extern int IntSetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private static int IntPtrToInt32(IntPtr intPtr)
        {
            return unchecked((int) intPtr.ToInt64());
        }

        [DllImport("kernel32.dll", EntryPoint = "SetLastError")]
        public static extern void SetLastError(int dwErrorCode);

        #endregion

        #endregion

        #endregion
    }
}