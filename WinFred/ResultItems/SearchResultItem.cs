using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using James.HelperClasses;
using James.Search;

namespace James.ResultItems
{
    public class SearchResultItem: ResultItem
    {
        public SearchResultItem()
        {
            
        }

        public override ImageSource Icon => FileIconCache.Instance.GetFileIcon(Subtitle);

        private readonly bool _isDirectory;

        public SearchResultItem(string path, int priority)
        {
            Subtitle = path;
            Priority = priority;
            if (Directory.Exists(path))
            {
                Title = path.Split('\\').Last();
            }
            else
            {
                Title = PathHelper.GetFilename(path);
                if (!Config.Instance.DisplayFileExtensions)
                {
                    Title = Title.Substring(0, Title.LastIndexOf('.'));
                }
            }
            _isDirectory = Directory.Exists(Subtitle);
        }

        /// <summary>
        /// Gets triggered if Enter is pressed or item got clicked
        /// </summary>
        /// <param name="e"></param>
        /// <param name="search"></param>
        public override void Open(KeyEventArgs e, string search, bool showFileProperties)
        {
            if (showFileProperties)
            {
                PathHelper.OpenPathPropertyWindow(Subtitle);
            }
            else if (e.KeyboardDevice.Modifiers == (ModifierKeys.Shift | ModifierKeys.Control))
            {
                if (_isDirectory)
                {
                    OpenFolder(true);
                }
                else
                {
                    Open(Subtitle, true);
                }
            }
            else if (e.KeyboardDevice.Modifiers == ModifierKeys.Shift)
            {
                OpenFolder(true);
            }
            else
            {
                Open(Subtitle);
            }
            SearchEngine.Instance.IncrementPriority(this);
        }

        /// <summary>
        /// Provides the string for  the auto completion
        /// </summary>
        /// <returns></returns>
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
        private void OpenFolder(bool selectFolder = false)
        {
            if (selectFolder)
            {
                Process.Start("explorer.exe", "/select," + Subtitle);
            }
            else
            {
                Process.Start(Subtitle);
            }
        }
    }
}
