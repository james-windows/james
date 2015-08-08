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
        private static readonly object LookObject = new object();
        public static Config GetInstance()
        {
            lock (LookObject)
            {
                if (config == null)
                {
                    try
                    {
                        config =
                            HelperClass.Derialize<Config>(
                                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                "\\WinFred\\config.xml");
                    }
                    catch (Exception)
                    {
                        InitConfig();
                    }
                }
                return config;
            }
        }

        private static void InitConfig()
        {
            config = new Config();
            config.Paths.Add(new Path() { Location = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) });
            LoadDefaultFileExtensions();
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
        private bool _startProgramOnStartup = false;

        public event ChangedWindowAccentColorEventHandler WindowChangedAccentColor;
        private string _windowAccentColor = "Lime";
        private bool _isBaseLight = true;

        public bool StartProgramOnStartup
        {
            get
            {
                return _startProgramOnStartup;
            }
            set
            {
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey
                        ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (value)
                {
                    registryKey?.SetValue("Winfred", System.Reflection.Assembly.GetExecutingAssembly().Location);
                }
                else
                {
                    registryKey?.DeleteValue("Winfred", false);
                }
                _startProgramOnStartup = value;
            }
        }

        public string WindowAccentColor
        {
            get
            {
                return _windowAccentColor;
            }

            set
            {
                _windowAccentColor = value;
                WindowChangedAccentColor?.Invoke(this, new EventArgs());
            }
        }

        public bool IsBaseLight
        {
            get
            {
                return _isBaseLight;
            }

            set
            {
                _isBaseLight = value;
                WindowChangedAccentColor?.Invoke(this, new EventArgs());
            }
        }

        #endregion
        private Config()
        {
            Paths = new ObservableCollection<Path>();
            DefaultFileExtensions = new List<FileExtension>();
            Workflows = new ObservableCollection<Workflow>();
            
        }
        static public void LoadDefaultFileExtensions()
        {
            List<FileExtension> tmp = new List<FileExtension>
            {
                new FileExtension("exe", 100),
                new FileExtension("png", 10),
                new FileExtension("jpg", 10),
                new FileExtension("pdf", 40),
                new FileExtension("doc", 10),
                new FileExtension("c", 11),
                new FileExtension("cpp", 11),
                new FileExtension("html", 15),
                new FileExtension("js", 10),
                new FileExtension("html", 10),
                new FileExtension("msi", 80),
                new FileExtension("zip", 50),
                new FileExtension("csv", 10),
                new FileExtension("cs", 10),
                new FileExtension("cshtml", 10),
                new FileExtension("jar", 20),
                new FileExtension("java", 30),
                new FileExtension("txt", 20),
                new FileExtension("docx", 10),
                new FileExtension("xmcd", 39),
                new FileExtension("mcdx", 40)
            };
            config.DefaultFileExtensions.AddRange(tmp);
            config.DefaultFileExtensions.Sort();
        }
    }
}
