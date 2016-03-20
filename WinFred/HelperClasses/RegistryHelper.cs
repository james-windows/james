using System.IO;
using Microsoft.Win32;

namespace James.HelperClasses
{
    class RegistryHelper
    {
        /// <summary>
        /// Defines a new custom url protocol for james
        /// </summary>
        public static void RegisterCustomProtocol()
        {
            string executablePath = Directory.GetCurrentDirectory() + "\\James.exe";
            RegistryKey reg = Registry.CurrentUser.CreateSubKey("Software\\Classes\\james");
            reg?.SetValue("URL PROTOCOL", "");
            reg?.CreateSubKey("shell\\open\\command")?.SetValue("", $"\"{executablePath}\" \"%1\"");
        }

        /// <summary>
        /// Sets the registry key to associat .james files
        /// </summary>
        public static void AssociateFileExtension()
        {
            string executablePath = Directory.GetCurrentDirectory() + "\\James.exe";
            string iconPath = Directory.GetCurrentDirectory() + "\\Resources\\logo2.ico";
            RegistryKey FileReg = Registry.CurrentUser.CreateSubKey("Software\\Classes\\.james");
            Registry.CurrentUser.CreateSubKey("Software\\Classes\\Applicatons\\MyNotepad.exe");
            Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\.james");

            FileReg?.CreateSubKey("DefaultIcon")?.SetValue("", iconPath);
            FileReg?.CreateSubKey("shell\\open\\command")?.SetValue("", $"\"{executablePath}\" %1");
        }

        /// <summary>
        /// Sets to add/delete the registry key, to start James on startup
        /// </summary>
        /// <param name="value">determines if the key should be deleted or added</param>
        public static void SetProgramAtStartup(bool value)
        {
            string executablePath = Directory.GetCurrentDirectory() + "\\James.exe";
            var registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (value)
            {
                registryKey?.SetValue("James", "\"" + executablePath + "\"");
            }
            else
            {
                registryKey?.DeleteValue("James", false);
            }
        }

        /// <summary>
        /// Returns the path to the users download folder
        /// </summary>
        /// <returns></returns>
        public static string GetDownloadFolderLocation()
        {
            return Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders",
                    "{374DE290-123F-4565-9164-39C4925E467B}", string.Empty).ToString();
        }
    }
}
