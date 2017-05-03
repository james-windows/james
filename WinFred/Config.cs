using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media.Imaging;
using James.HelperClasses;
using James.Search;
using James.Shortcut;
using Newtonsoft.Json;
using Path = James.Search.Path;

namespace James
{
    public delegate void ChangedWindowAccentColorEventHandler(object sender, EventArgs e);

    public class Config
    {
        private Config()
        {
            Paths = new ObservableCollection<Path>();
            DefaultFileExtensions = new List<FileExtension>();
            ExcludedFolders = new ObservableCollection<string>();
        }

        #region singleton

        private static Config _config;
        private static readonly object SingeltonLock = new object();

        public static Config Instance
        {
            get
            {
                lock (SingeltonLock)
                {
                    if (_config == null)
                    {
                        try
                        {
                            var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\James";
                            Directory.CreateDirectory(path);
                            _config = SerializationHelper.Deserialize<Config>(path + "\\config.json");
                        }
                        catch (FileNotFoundException e)
                        {
                            InitConfig();
                        }
                    }
                    return _config;
                }
            }
        }

        /// <summary>
        /// Initialiazes the config
        /// </summary>
        private static void InitConfig()
        {
            _config = new Config {Paths = new ObservableCollection<Path>(DefaultPaths.GetDefault())};
            _config.DefaultFileExtensions.AddRange(HelperClasses.DefaultFileExtensions.GetDefault());
            _config.ShortcutManagerSettings = new ShortcutManagerSettings();
            _config.Persist();
            RegistryHelper.AssociateFileExtension();
            RegistryHelper.RegisterCustomProtocol();
        }
        
        /// <summary>
        /// Persists config into a .json file
        /// </summary>
        public void Persist()
        {
            lock (_config)
            {
                File.WriteAllText(ConfigFolderLocation + "\\config.json", _config.Serialize());
            }
        }

        /// <summary>
        /// Resets the Config and overrides all user's changes
        /// </summary>
        public void ResetConfig()
        {
            lock (SingeltonLock)
            {
                InitConfig();
                ShortcutManagerSettings.Reset();
                StartProgramOnStartup = false;
                WindowChangedAccentColor?.Invoke(this, null);
            }
            Persist();
        }

        #endregion

        #region fields

        public ObservableCollection<Path> Paths { get; set; }
        public List<FileExtension> DefaultFileExtensions { get; set; }
        public ObservableCollection<string> ExcludedFolders { get; set; }
        public ShortcutManagerSettings ShortcutManagerSettings { get; set; }

        public static string ConfigFolderLocation = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\James";
        public static string WorkflowFolderLocation => ConfigFolderLocation + "\\workflows";

        private string _windowAccentColor = "Cyan";
        public string ReleaseUrl { get; set; } = @"http://www.moserm.tk/Releases";

        public int DefaultFolderPriority { get; set; } = 80;
        public int MaxSearchResults { get; set; } = 8;
        public int StartSearchMinTextLength { get; set; } = 1;
        public double LargeTypeOpacity { get; set; } = 0.75;

        public event ChangedWindowAccentColorEventHandler WindowChangedAccentColor;

        private bool _isBaseLight = true;
        private bool _startProgramOnStartup;
        public bool AlwaysClearLastInput { get; set; } = true;
        public bool DisplayFileIcons { get; set; } = true;
        public bool DisplayPriorities { get; set; } = false;
        public bool DisplayFileExtensions { get; set; } = false;

        [JsonIgnore] //sets by the first instance
        public bool FirstInstance { get; set; } = true;

        public bool StartProgramOnStartup
        {
            get { return _startProgramOnStartup; }
            set
            {
                RegistryHelper.SetProgramAtStartup(value);
                _startProgramOnStartup = value;
            }
        }

        public string WindowAccentColor
        {
            get { return _windowAccentColor; }
            set
            {
                _windowAccentColor = value;
                WindowChangedAccentColor?.Invoke(this, new EventArgs());
            }
        }

        public bool IsBaseLight
        {
            get { return _isBaseLight; }
            set
            {
                _isBaseLight = value;
                WindowChangedAccentColor?.Invoke(this, new EventArgs());
            }
        }

        [JsonIgnore]
        public BitmapImage Icon { get; set; } = IconHelper.GetIcon("logo2.ico");

        #endregion
    }
}