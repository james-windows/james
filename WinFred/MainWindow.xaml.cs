using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using WinFred.Search;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace WinFred
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LargeType lt;
        private SearchEngine search;

        public MainWindow()
        {
            InitializeComponent();
            var _hotKey = new HotKey(Key.Space, KeyModifier.Alt, OnHotKeyHandler);
            FileSystemWatcher watcher = new FileSystemWatcher(@"C:");
            InitFileSystemWatcher(ref watcher);
            this.Visibility = Visibility.Hidden;

            search = SearchEngine.GetInstance();
            resultList = new ObservableCollection<SearchResult>();
            SearchResultListBox.ItemsSource = resultList;
            Config.GetInstance();
        }

        private void OnHotKeyHandler(HotKey hotKey)
        {
            if (this.IsVisible || hotKey == null)
            {
                HideWindow();
            }
            else
            {
                this.Show();
                this.Activate();
                this.SearchTextBox.Focus();
            }
        }

        private void HideWindow()
        {
            if (lt != null && lt.IsActive)
            {
                lt.Close();
            }
            this.SearchTextBox.Text = "";
            this.Hide();
        }

        private void InitFileSystemWatcher(ref FileSystemWatcher watcher)
        {
            watcher.IncludeSubdirectories = true;
         //   watcher.Filter = @"^.*\.(pdf|txt|html|doc|docx|csv|cs|c|cpp|exe|msi|zip|rar|xml|xaml|js|css|jpg|png|jpeg|gif|mp4|mp3)$";
            watcher.Created += file_Changed;
            watcher.Deleted += file_Changed;
            watcher.Renamed += file_Changed;
            watcher.Changed += file_Changed;
            watcher.EnableRaisingEvents = true;
        }

        void file_Changed(object sender, FileSystemEventArgs e)
        {
            String path = e.FullPath;
            Path isInterestingPath = null;
            foreach (Path item in Config.GetInstance().Paths)
            {
                if (path.ToLower().StartsWith(item.Location.ToLower()) && item.IsEnabled)
                {
                    isInterestingPath = item;
                    break;
                }
            }

            if (isInterestingPath != null)//look if ending is interesting
            {
                FileExtension interestingFileExtension = null;
                String fileExtension = path.Split('.').Last();
                foreach (FileExtension item in isInterestingPath.FileExtensions)
                {
                    if (fileExtension == item.Extension)
                    {
                        interestingFileExtension = item;
                        break;
                    }
                }
                if (interestingFileExtension == null)
                {
                    foreach (FileExtension item in Config.GetInstance().DefaultFileExtensions)
                    {
                        if (fileExtension == item.Extension)
                        {
                            interestingFileExtension = item;
                            break;
                        }
                    }
                }
                if (interestingFileExtension != null) //matching file
                {
                    if (e.ChangeType == WatcherChangeTypes.Created)
                    {
                        search.AddFile(new Data(path, interestingFileExtension.Priority));
                    }
                    else if (e.ChangeType == WatcherChangeTypes.Changed)
                    {
                        search.IncrementPriority(path);
                    }
                    else if (e.ChangeType == WatcherChangeTypes.Deleted)
                    {
                        search.DeleteFile(path);
                    }
                    //else if (e.ChangeType == WatcherChangeTypes.Renamed)
                    //{
                        
                    //}
                }
            }
        }

        #region Window-Events
        private void SearchTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.KeyboardDevice.IsKeyDown(Key.Escape))
            {
                if (lt != null && lt.IsActive) 
                {
                    lt.Close();
                }
                this.Hide();
                this.SearchTextBox.Text = "";
            }
            else if (e.KeyboardDevice.IsKeyDown(Key.L) && e.KeyboardDevice.IsKeyDown(Key.LeftAlt) && SearchTextBox.Text.Length > 0)
            {
                String message = this.SearchTextBox.Text;
                lt = new LargeType(message);
                lt.Owner = this;
                lt.ShowDialog();
                lt = null;
            }
            else if (e.KeyboardDevice.IsKeyDown(Key.S) && e.KeyboardDevice.IsKeyDown(Key.LeftAlt))
            {
                OptionWindow window = new OptionWindow();
                window.ShowDialog();
                HideWindow();
            }
        }
        private void SearchTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.IsKeyDown(Key.Down))
            {
                SearchResultListBox.SelectedIndex++;
            }
            else if (e.KeyboardDevice.IsKeyDown(Key.Up))
            {
                if(SearchResultListBox.SelectedIndex > 0)
                    SearchResultListBox.SelectedIndex--;
            }
            else if (e.KeyboardDevice.IsKeyDown(Key.Enter) && SearchResultListBox.SelectedItem != null)
            {
                HideWindow();
                if (!(e.KeyboardDevice.IsKeyDown(Key.LeftShift) || e.KeyboardDevice.IsKeyDown(Key.RightShift)))
                {
                    ((SearchResult)SearchResultListBox.SelectedItem).Open();
                }
                else
                {
                    ((SearchResult)SearchResultListBox.SelectedItem).OpenFolder();
                }
            }
        }
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Focus();
        }
        private int SEARCH_ID;
        private ObservableCollection<SearchResult> resultList;
        private void Search(string str, int id)
        {
            DateTime tmp = DateTime.Now;
            var res = search.Query(str);
            
            Dispatcher.BeginInvoke((Action)(() =>
            {
                resultList.Clear();
                foreach (SearchResult x in res)
                    resultList.Add(x);
                if (res.Count > 0)
                {
                    SearchResultListBox.SelectedIndex = 0;
                }
            }), DispatcherPriority.Send);
            Debug.WriteLine((DateTime.Now - tmp).TotalMilliseconds);
        }

        private void ExecuteWorkflow(Workflow workflow, String str)
        {
            DateTime tmp = DateTime.Now;
            String line = "";
            Process process = workflow.Execute(str.Replace(workflow.Keyword, ""));
            process.Start();
            while (!process.StandardOutput.EndOfStream)
            {
                line += process.StandardOutput.ReadLine();
            }
            line = HelperClass.BuildHTML(line);
            line = line.Replace("suppldata", "\"table table-bordered table-striped\"");
            Dispatcher.BeginInvoke((Action)(() => OutputWebBrowser.NavigateToString(line)));
            Debug.WriteLine((DateTime.Now - tmp).TotalMilliseconds);
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string str = SearchTextBox.Text;
            bool workflowExecuted = false;
            foreach (Workflow item in Config.GetInstance().Workflows)//todo update to binary search is necessary
            {
                if (item.IsEnabled && str.StartsWith(item.Keyword))
                {
                    workflowExecuted = true;
                    resultList.Clear();
                    OutputWebBrowser.Visibility = Visibility.Visible;
                    new Task(()=>ExecuteWorkflow(item, str)).Start();
                    return;
                }
            }
            if(!workflowExecuted)
            {
                OutputWebBrowser.Navigate("about:blank");
                OutputWebBrowser.Visibility = Visibility.Collapsed;
                int id = SEARCH_ID;
                SEARCH_ID = (SEARCH_ID + 1) % 1000000007;
                new Task(() => Search(str, id)).Start();
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            HideWindow();
            ((SearchResult)SearchResultListBox.SelectedItem).Open();
        }
        private void Window_Deactivated(object sender, EventArgs e)
        {
            if (!(lt != null))
                OnHotKeyHandler(null);
        }

        #region region for hidding the window in the taskswitch
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowInteropHelper wndHelper = new WindowInteropHelper(this);

            int exStyle = (int)GetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE);

            exStyle |= (int)ExtendedWindowStyles.WS_EX_TOOLWINDOW;
            SetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);
        }
        #region Window styles
        [Flags]
        public enum ExtendedWindowStyles
        {
            // ...
            WS_EX_TOOLWINDOW = 0x00000080,
            // ...
        }

        public enum GetWindowLongFields
        {
            // ...
            GWL_EXSTYLE = (-20),
            // ...
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            int error = 0;
            IntPtr result = IntPtr.Zero;
            // Win32 SetWindowLong doesn't clear error on success
            SetLastError(0);

            if (IntPtr.Size == 4)
            {
                // use SetWindowLong
                Int32 tempResult = IntSetWindowLong(hWnd, nIndex, IntPtrToInt32(dwNewLong));
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
                throw new System.ComponentModel.Win32Exception(error);
            }

            return result;
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
        private static extern IntPtr IntSetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
        private static extern Int32 IntSetWindowLong(IntPtr hWnd, int nIndex, Int32 dwNewLong);

        private static int IntPtrToInt32(IntPtr intPtr)
        {
            return unchecked((int)intPtr.ToInt64());
        }

        [DllImport("kernel32.dll", EntryPoint = "SetLastError")]
        public static extern void SetLastError(int dwErrorCode);
        #endregion
        #endregion

        #endregion
    }
}
