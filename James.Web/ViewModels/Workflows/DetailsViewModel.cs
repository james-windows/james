using System;
using System.IO;
using James.Web.Models;

namespace James.Web.ViewModels.Workflows
{
    public class DetailsViewModel
    {
        public Workflow Workflow { get; set; }

        public bool EditAllowed { get; set; }

        public DisqusViewModel DisqusViewModel { get; set; }

        private string _iconPath;
        public string IconPath
        {
            get { return _iconPath; }
            set {
                _iconPath = File.Exists(value) ? value.Substring(value.IndexOf("workflows") - 1) : $"/images/default.png";
            }
        }

        public string FileSize
        {
            get
            {
                string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
                if (Workflow.FileSize == 0)
                    return "0" + suf[0];
                long bytes = Math.Abs(Workflow.FileSize);
                int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
                double num = Math.Round(bytes / Math.Pow(1024, place), 1);
                return (Math.Sign(Workflow.FileSize) * num) + suf[place];
            }
        }
    }
}
