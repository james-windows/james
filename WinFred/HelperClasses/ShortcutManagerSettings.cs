using System.Collections.Generic;
using System.Windows.Input;
using MahApps.Metro.Controls;

namespace James.HelperClasses
{
    public class ShortcutManagerSettings
    {
        public List<Shortcut> Shortcuts { get; set; }
        public Shortcut JamesHotKey { get; set; }
        public Shortcut LargeTypeHotKey { get; set; }
        public Shortcut SettingsHotKey { get; set; }

        public ShortcutManagerSettings Reset()
        {
            Shortcuts = new List<Shortcut>();
            JamesHotKey = new Shortcut() { HotKey = new HotKey(Key.Space, ModifierKeys.Alt) };
            LargeTypeHotKey = new Shortcut() { HotKey = new HotKey(Key.L, ModifierKeys.Alt) };
            SettingsHotKey = new Shortcut() { HotKey = new HotKey(Key.S, ModifierKeys.Alt) };
            return this;
        }
    }
}
