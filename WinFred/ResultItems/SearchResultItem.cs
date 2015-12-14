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
            Title = Directory.Exists(path) ? path.Split('\\').Last() : James.HelperClasses.PathHelper.GetFilename(path);
        }

        public override void Open(KeyEventArgs e)
        {
            if (KeyboardHelper.IsShiftKeyDown(e) && KeyboardHelper.IsCtrlKeyDown(e))
            {
                Open(Subtitle, true);
            }
            else if (KeyboardHelper.IsShiftKeyDown(e))
            {
                Open(Directory.Exists(Subtitle) ? Subtitle : PathHelper.GetFoldername(Subtitle));
            }
            else
            {
                Open(Subtitle);
            }
        }

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
                    throw;
            }
            SearchEngine.Instance.IncrementPriority(this);
        }
    }
}
