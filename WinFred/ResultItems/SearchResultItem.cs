﻿using System;
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

        /// <summary>
        /// Gets triggered if Enter is pressed or item got clicked
        /// </summary>
        /// <param name="e"></param>
        public override void Open(KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == (ModifierKeys.Shift | ModifierKeys.Control))
            {
                Open(Subtitle, !Directory.Exists(Subtitle));
            }
            else if (e.KeyboardDevice.Modifiers == ModifierKeys.Shift || Directory.Exists(Subtitle))
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
