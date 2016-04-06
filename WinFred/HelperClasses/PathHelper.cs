using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace James.HelperClasses
{
    public static class PathHelper
    {
        /// <summary>
        /// Fetches the file name of an providen path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFilename(string path)
        {
            return path.Split('\\').Last();
        }

        /// <summary>
        /// Returns online the path to the folder, of the given file path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFolderPath(string path)
        {
            return path.Substring(0, path.LastIndexOf('\\'));
        }

        /// <summary>
        /// Searchs through the Windows Paths location for an given executable to get the full path
        /// </summary>
        /// <param name="executableName"></param>
        /// <returns></returns>
        public static string GetFullPathOfExe(string executableName)
        {
            var path = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);
            string executablePath = null;
            foreach (var p in path.Split(';'))
            {
                var fullPath = Path.Combine(p, executableName);
                if (File.Exists(fullPath))
                {
                    executablePath = fullPath;
                    break;
                }
            }
            return executablePath;
        }

        /// <summary>
        /// Opens the property window of the explorer.exe for a providen path
        /// </summary>
        /// <param name="path"></param>
        public static void OpenPathPropertyWindow(string path)
        {
            if (File.Exists(path) || Directory.Exists(path))
            {
                if (!ShowFileProperties(path))
                {
                    Console.WriteLine("Something went wrong...");
                }
            }
        }

        #region P/Invoke for open explorer's file properties window
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern bool ShellExecuteEx(ref SHELLEXECUTEINFO lpExecInfo);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHELLEXECUTEINFO
        {
            public int cbSize;
            public uint fMask;
            public IntPtr hwnd;
            [MarshalAs(UnmanagedType.LPTStr)] public string lpVerb;
            [MarshalAs(UnmanagedType.LPTStr)] public string lpFile;
            [MarshalAs(UnmanagedType.LPTStr)] public string lpParameters;
            [MarshalAs(UnmanagedType.LPTStr)] public string lpDirectory;
            public int nShow;
            public IntPtr hInstApp;
            public IntPtr lpIDList;
            [MarshalAs(UnmanagedType.LPTStr)] public string lpClass;
            public IntPtr hkeyClass;
            public uint dwHotKey;
            public IntPtr hIcon;
            public IntPtr hProcess;
        }

        private const int SW_SHOW = 5;
        private const uint SEE_MASK_INVOKEIDLIST = 12;

        private static bool ShowFileProperties(string Filename)
        {
            SHELLEXECUTEINFO info = new SHELLEXECUTEINFO();
            info.cbSize = Marshal.SizeOf(info);
            info.lpVerb = "properties";
            info.lpFile = Filename;
            info.nShow = SW_SHOW;
            info.fMask = SEE_MASK_INVOKEIDLIST;
            return ShellExecuteEx(ref info);
        }
        #endregion
    }
}