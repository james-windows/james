﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media.Imaging;
using James.HelperClasses;
using James.Search;
using James.Shortcut;
using Microsoft.Win32;
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
                            var instance = ShortcutManager.Instance;
                            _config.Icon = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\Resources\\logo2.ico"));
                        }
                        catch (Exception e)
                        {
                            InitConfig();
                        }
                    }
                    return _config;
                }
            }
        }

        private static void InitConfig()
        {
            _config = new Config();
            _config.Paths.Add(new Path {Location = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)});
            _config.DefaultFileExtensions.AddRange(HelperClasses.DefaultFileExtensions.GetDefault());
            _config.ShortcutManagerSettings = new ShortcutManagerSettings();
            _config.Persist();
            AssociateFileExtension();
        }

        public static void AssociateFileExtension()
        {
            string executablePath = Directory.GetCurrentDirectory() + "\\James.exe";
            string iconPath = Directory.GetCurrentDirectory() + "\\Resources\\logo2.ico";
            RegistryKey FileReg = Registry.CurrentUser.CreateSubKey("Software\\Classes\\.james");
            RegistryKey AppReg = Registry.CurrentUser.CreateSubKey("Software\\Classes\\Applicatons\\MyNotepad.exe");
            RegistryKey AppAssoc = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\.james");

            FileReg.CreateSubKey("DefaultIcon").SetValue("", iconPath);
            FileReg.CreateSubKey("shell\\open\\command").SetValue("", "\""+ executablePath + "\" %1");
        }

        public void Persist()
        {
            lock (_config)
            {
                File.WriteAllText(_config.ConfigFolderLocation + "\\config.json", _config.Serialize());
            }
        }

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

        public string ConfigFolderLocation { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\James";
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

        public bool StartProgramOnStartup
        {
            get { return _startProgramOnStartup; }
            set
            {
                var registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (value)
                {
                    registryKey?.SetValue("James", "\"" + ConfigFolderLocation + "\\Update.exe\" --processStart James.exe");
                }
                else
                {
                    registryKey?.DeleteValue("James", false);
                }
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
        public BitmapImage Icon { get; set; }

        #endregion
    }
}