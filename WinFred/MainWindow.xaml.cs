using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
            //FileSystemWatcher watcher = new FileSystemWatcher(@"C:\Users\fleimgruber\Desktop");
            //initFileSystemWatcher(ref watcher);
            this.Visibility = Visibility.Hidden;

            search = SearchEngine.GetInstance();

            Config.GetInstance();
        }

        private void OnHotKeyHandler(HotKey hotKey)
        {
            if (this.IsVisible)
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

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Focus();
        }

        private void HideWindow()
        {
            if (lt != null)
            {
                lt.Close();
            }
            this.SearchTextBox.Text = "";
            this.Hide();
        }

        private void InitFileSystemWatcher(ref FileSystemWatcher watcher)
        {
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastAccess;
            watcher.IncludeSubdirectories = true;
         //   watcher.Filter = @"^.*\.(pdf|txt|html|doc|docx|csv|cs|c|cpp|exe|msi|zip|rar|xml|xaml|js|css|jpg|png|jpeg|gif|mp4|mp3)$";
            watcher.Created += file_Changed;
            watcher.Renamed += file_Changed;
            watcher.EnableRaisingEvents = true;
        }

        void file_Changed(object sender, FileSystemEventArgs e)
        {
         //   Trace.WriteLine("ok: "+e.FullPath);
          //  search.AddFile(new SearchEngine.Data(e.FullPath) { Priority = 2 });
            //do stuff
            //MessageBox.Show("new File created!: " + e.FullPath);
        }

        private void SearchResultListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //todo open
        }

        private void SearchTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.KeyboardDevice.IsKeyDown(Key.Escape))
            {
                if (lt != null) 
                {
                    lt.Close();
                }
                this.Hide();
                this.SearchTextBox.Text = "";
            }
            else if (e.KeyboardDevice.IsKeyDown(Key.L) && e.KeyboardDevice.IsKeyDown(Key.LeftAlt) && SearchTextBox.Text.Length > 0)
            {
                lt = new LargeType(this.SearchTextBox.Text);
                lt.ShowDialog();
            }
            else if (e.KeyboardDevice.IsKeyDown(Key.S) && e.KeyboardDevice.IsKeyDown(Key.LeftAlt))
            {
                HideWindow();
                OptionWindow window = new OptionWindow();
                window.ShowDialog();
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
                ((SearchResult)SearchResultListBox.SelectedItem).Open();
            }
        }
        private int SEARCH_ID;

        private void Search(string str, int id)
        {
            var res = search.Query(str);
            if (id != SEARCH_ID - 1)
                return;
            Dispatcher.BeginInvoke((Action)(() =>
            {
                ObservableCollection<SearchResult> resultList = new ObservableCollection<SearchResult>();
                foreach (SearchResult x in res)
                    resultList.Add(x);
                SearchResultListBox.ItemsSource = resultList;
            }));
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string str = SearchTextBox.Text;
            int id = SEARCH_ID;
            SEARCH_ID = (SEARCH_ID + 1) % 1000000007;
            new Task(() => Search(str, id)).Start();
        }
    }
}
