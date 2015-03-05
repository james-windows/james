﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFred
{
    public class Config
    {
        #region singleton
        private static Config config;
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
                catch (Exception e)
                {
                    InitConfig();
                }
                return config;
            }
        }

        private static void InitConfig()
        {
            config = new Config();
            config.ConfigFolderLocation = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\WinFred";
            config.MaxSearchResults = 10;
            config.StartSearchMinTextLength = 3;
            config.Paths.Add(new Path() { Location = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) });
            string xml = HelperClass.Serialize(config);
            File.WriteAllText(config.ConfigFolderLocation + "\\config.xml", xml);
        }

        #endregion

        #region fields
        public List<Path> Paths { get; set; }
        public List<string> DefaultFileExtensions { get; set; } 

        public int MaxSearchResults;
        public int StartSearchMinTextLength;
        public String ConfigFolderLocation { get; set; }



        #endregion
        private Config()
        {
            Paths = new List<Path>();
            DefaultFileExtensions = new List<string>();
            
        }
    }

    public class Path
    {
        public string Location { get; set; }
        public List<string> IncludedExtensions { get; set; }
        public List<string> ExcludedExtensions { get; set; }
        public List<string> ExcludedFolders { get; set; }

        public Path()
        {
            IncludedExtensions = new List<string>();
            ExcludedExtensions = new List<string>();
            ExcludedFolders = new List<string>();
        }
    }

}
