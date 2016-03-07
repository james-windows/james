using System;
using System.Collections.Generic;
using James.Search;
using Microsoft.Win32;

namespace James.HelperClasses
{
    public class DefaultPaths
    {
        public static IEnumerable<Path> GetDefault()
        {
            List<Path> paths = new List<Path>();
            paths.Add(new Path { Location = Environment.GetFolderPath(Environment.SpecialFolder.Desktop), Priority = 50 });
            //download folder:
            string downloadFolder =
                Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders",
                    "{374DE290-123F-4565-9164-39C4925E467B}", string.Empty).ToString();
            if (downloadFolder != string.Empty)
            {
                paths.Add(new Path { Location = downloadFolder, Priority = 50 });
            }
            paths.Add(new Path { Location = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Priority = 50});
            paths.Add(new Path { Location = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), Priority = 45 });
            paths.Add(new Path { Location = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), Priority = 45 });
            paths.Add(new Path { Location = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), Priority = 45 });
            paths.Add(CreateProgramPath(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), 100));
            paths.Add(CreateProgramPath(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), 95));

            paths.Add(CreateStartMenuPaths(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), 100));
            paths.Add(CreateStartMenuPaths(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu), 95));

            return paths;
        }

        private static Path CreateProgramPath(string path, int priority)
        {
            return new Path()
            {
                Location = path,
                IsDefaultConfigurationEnabled = false,
                IndexFolders = false,
                Priority = priority,
                FileExtensions = new List<FileExtension>() { new FileExtension("exe", 100)}
            };
        }

        private static Path CreateStartMenuPaths(string path, int priority)
        {
            return new Path
            {
                Location = path,
                IsDefaultConfigurationEnabled = false,
                IndexFolders = false,
                Priority = priority,
                FileExtensions = new List<FileExtension>() { new FileExtension("lnk", 90)}
            };
        }
    }
}
