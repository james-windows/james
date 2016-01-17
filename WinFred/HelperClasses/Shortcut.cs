using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MahApps.Metro.Controls;

namespace James.HelperClasses
{
    public class Shortcut
    {
        public string Action { get; set; }
        public bool IsEnabled { get; set; }
        public bool AutoRun { get; set; }
        public HotKey HotKey { get; set; }
    }
}
