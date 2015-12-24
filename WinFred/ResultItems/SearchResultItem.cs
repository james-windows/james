using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using James.HelperClasses;
using James.Search;

namespace James.ResultItems
{
    public class SearchResultItem: ResultItem
    {
        public SearchResultItem()
        {
            
        }

        public SearchResultItem(string path, int priority)
        {
            Subtitle = path;
            Priority = priority;
            Title = Directory.Exists(path) ? path.Split('\\').Last() : PathHelper.GetFilename(path);
        }

        public override void Open(KeyEventArgs e)
        {
            if (KeyboardHelper.IsShiftKeyDown(e) && KeyboardHelper.IsCtrlKeyDown(e))
            {
                Open(Subtitle, !Directory.Exists(Subtitle));
            }
            else if (KeyboardHelper.IsShiftKeyDown(e))
            {
                OpenFolder();
            }
            else
            {
                Open(Subtitle);
            }
            SearchEngine.Instance.IncrementPriority(this);
        }

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

        private void OpenFolder()
        {
            Process.Start("explorer.exe", "/select," + Subtitle);
        }
    }
}
