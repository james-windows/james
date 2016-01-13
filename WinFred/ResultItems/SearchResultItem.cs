using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using James.HelperClasses;
using James.Search;

namespace James.ResultItems
{
    public class SearchResultItem: ResultItem
    {
        public SearchResultItem()
        {
            
        }

        public override ImageSource Icon
        {
            get
            {
                try
                {
                    return _isDirectory ? null : System.Drawing.Icon.ExtractAssociatedIcon(Subtitle).ToImageSource();
                }
                catch (FileNotFoundException)
                {
                    return null;
                }
            }
        }

        private readonly bool _isDirectory;

        public SearchResultItem(string path, int priority)
        {
            Subtitle = path;
            Priority = priority;
            Title = Directory.Exists(path) ? path.Split('\\').Last() : PathHelper.GetFilename(path);
            _isDirectory = Directory.Exists(Subtitle);
        }

        /// <summary>
        /// Gets triggered if Enter is pressed or item got clicked
        /// </summary>
        /// <param name="e"></param>
        public override void Open(KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == (ModifierKeys.Shift | ModifierKeys.Control))
            {
                Open(Subtitle, !_isDirectory);
            }
            else if (e.KeyboardDevice.Modifiers == ModifierKeys.Shift || _isDirectory)
            {
                OpenFolder();
            }
            else if (e.KeyboardDevice.Modifiers == ModifierKeys.Alt)
            {
                PathHelper.OpenPathPropertyWindow(Subtitle);
            }
            else
            {
                Open(Subtitle);
            }
            SearchEngine.Instance.IncrementPriority(this);
        }

        public override string AutoComplete() => Title;

        /// <summary>
        /// Starts the program/ file or opens the folder of the given path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="runAsAdmin">Should the file or program should be started as Admin?</param>
        private void Open(string path, bool runAsAdmin = false)
        {
            const int ERROR_CANCELLED = 1223; //The operation was canceled by the user.
            ProcessStartInfo info = new ProcessStartInfo(path);
            if (runAsAdmin)
            {
                info.Verb = "runas";
            }
            try
            {
                Process.Start(info);
            }
            catch (Win32Exception ex)
            {
                if (ex.NativeErrorCode == ERROR_CANCELLED)
                    Console.WriteLine("User cancelled");
                else
                    Console.WriteLine("other error occured: "+ ex.Message);
            }
        }

        /// <summary>
        /// Opens the explorer with the position of the file/folder.
        /// Also selects the given item
        /// </summary>
        private void OpenFolder()
        {
            Process.Start("explorer.exe", "/select," + Subtitle);
        }
    }
}
