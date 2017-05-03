using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using James.HelperClasses;
using James.Shortcut;
using Microsoft.Win32;

namespace James.Windows
{
    /// <summary>
    /// Blurs the MainWindow and hides it from Tasklist
    /// </summary>
    partial class MainWindow
    {
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowHelper.HideWindowFromTaskList(this);

            //blur
            var windowHelper = new WindowInteropHelper(this);

            var accent = new AccentPolicy();
            var accentStructSize = Marshal.SizeOf(accent);
            accent.AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND;

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData();
            data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
            data.SizeOfData = accentStructSize;
            data.Data = accentPtr;

            SetWindowCompositionAttribute(windowHelper.Handle, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }

        /// Used the code from the following source:
        /// http://withinrafael.com/adding-the-aero-glass-blur-to-your-windows-10-apps/
        /// only working with Windows 10
        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        [StructLayout(LayoutKind.Sequential)]
        internal struct WindowCompositionAttributeData
        {
            public WindowCompositionAttribute Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }

        internal enum WindowCompositionAttribute
        {
            // ...
            WCA_ACCENT_POLICY = 19
            // ...
        }

        internal enum AccentState
        {
            ACCENT_DISABLED = 0,
            ACCENT_ENABLE_GRADIENT = 1,
            ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
            ACCENT_ENABLE_BLURBEHIND = 3,
            ACCENT_INVALID_STATE = 4
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct AccentPolicy
        {
            public AccentState AccentState;
            public int AccentFlags;
            public int GradientColor;
            public int AnimationId;
        }

        private void BindEvents()
        {
            Loaded += SetWindowPosition;
            SystemEvents.DisplaySettingsChanged += SetWindowPosition;
            ShortcutManager.Instance.ShortcutPressed += (sender, args) => OnHotKeyHandler(sender as Shortcut.Shortcut);
            LargeType.Instance.Deactivated += LargeType_Deactivated;
            LargeType.Instance.Activated += LargeType_Activated;
        }

        private void SetWindowPosition(object sender, EventArgs e)
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

        /// <summary>
        /// Hides current window if it's no longer in the focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Deactivated(object sender, EventArgs e)
        {
            if (!_showLargeType)
            {
                HideWindow();
            }
        }

        /// <summary>
        /// Shows search window
        /// </summary>
        private void ShowWindow()
        {
            Show();
            Activate();
            SearchTextBox.Focus();
        }

        /// <summary>
        /// Hides search window
        /// </summary>
        public void HideWindow()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                LargeType.Instance.Hide();
                ClearInputAtUsersDesire();
                Hide();
            });
        }
    }
}