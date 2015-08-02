using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace WinFred
{
    public delegate void ChangedWindowAccentColorEventHandler(object sender, EventArgs e);
    public class Config
    {
        #region singleton
        public static Config config;
        private static readonly Object lookObject = new object();
        public static Config GetInstance()
        {
            lock (lookObject)
            {
                if (config == null)
                try
                {
                    config = HelperClass.Derialize<Config>(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                        "\\WinFred\\config.xml");
                }
                catch (Exception)
                {
                    InitConfig();
                }
                return config;
            }
        }

        private static void InitConfig()
        {
            config = new Config();
            config.Paths.Add(new Path() { Location = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) });
            loadDefaultFileExtensions();
            //add suppl workflow
            config.Persist();            
        }

        /// <summary>
        /// Saves the current configuration as xml in the config for
        /// </summary>
        public void Persist()
        {
            lock (config)
            {
                DefaultFileExtensions.Sort();
                foreach (var item in Paths)
                {
                    item.FileExtensions.Sort();
                }
                File.WriteAllText(config.ConfigFolderLocation + "\\config.xml", HelperClass.Serialize(config));
            }
        }

        #endregion

        #region fields
        public ObservableCollection<Path> Paths { get; set; }
        public List<FileExtension> DefaultFileExtensions { get; set; }

        public ObservableCollection<Workflow> Workflows { get; set; }

        public int MaxSearchResults { get; set; } = 8;
        public int StartSearchMinTextLength { get; set; } = 3;
        public string ConfigFolderLocation { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\WinFred";
        private bool startProgramOnStartup = false;

        public event ChangedWindowAccentColorEventHandler WindowChangedAccentColor;
        private string windowAccentColor = "Lime";
        private bool isBaseLight = true;

        public bool StartProgramOnStartup
        {
            get
            {
                return startProgramOnStartup;
            }
            set
            {
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey
                        ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (value)
                {
                    registryKey.SetValue("Winfred", System.Reflection.Assembly.GetExecutingAssembly().Location);
                }
                else
                {
                    registryKey.DeleteValue("Winfred", false);
                }
                startProgramOnStartup = value;
            }
        }

        public string WindowAccentColor
        {
            get
            {
                return windowAccentColor;
            }

            set
            {
                windowAccentColor = value;
                if(WindowChangedAccentColor != null)
                    WindowChangedAccentColor(this, new EventArgs());
            }
        }

        public bool IsBaseLight
        {
            get
            {
                return isBaseLight;
            }

            set
            {
                isBaseLight = value;
                if (WindowChangedAccentColor != null)
                    WindowChangedAccentColor(this, new EventArgs());
            }
        }

        #endregion
        private Config()
        {
            Paths = new ObservableCollection<Path>();
            DefaultFileExtensions = new List<FileExtension>();
            Workflows = new ObservableCollection<Workflow>();
            
        }
        static public void loadDefaultFileExtensions()
        {
            List<FileExtension> tmp = new List<FileExtension>();
            tmp.Add(new FileExtension("exe", 100));
            tmp.Add(new FileExtension("png", 10));
            tmp.Add(new FileExtension("jpg", 10));
            tmp.Add(new FileExtension("pdf", 40));
            tmp.Add(new FileExtension("doc", 10));
            tmp.Add(new FileExtension("c", 11));
            tmp.Add(new FileExtension("cpp", 11));
            tmp.Add(new FileExtension("html", 15));
            tmp.Add(new FileExtension("js", 10));
            tmp.Add(new FileExtension("html", 10));
            tmp.Add(new FileExtension("msi", 80));
            tmp.Add(new FileExtension("zip", 50));
            tmp.Add(new FileExtension("csv", 10));
            tmp.Add(new FileExtension("cs", 10));
            tmp.Add(new FileExtension("cshtml", 10));
            tmp.Add(new FileExtension("jar", 20));
            tmp.Add(new FileExtension("java", 30));
            tmp.Add(new FileExtension("txt", 20));
            tmp.Add(new FileExtension("docx", 10));
            tmp.Add(new FileExtension("xmcd", 39));
            tmp.Add(new FileExtension("mcdx", 40));
            config.DefaultFileExtensions.AddRange(tmp);
            config.DefaultFileExtensions.Sort();
        }
    }
}
