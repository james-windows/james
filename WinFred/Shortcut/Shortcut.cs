using MahApps.Metro.Controls;

namespace James.Shortcut
{
    public class Shortcut
    {
        public string Action { get; set; }
        public bool IsEnabled { get; set; }
        public bool AutoRun { get; set; }
        public HotKey HotKey { get; set; }
    }
}
