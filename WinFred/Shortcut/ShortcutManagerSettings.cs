using System.Collections.Generic;
using System.Windows.Input;
using MahApps.Metro.Controls;

namespace James.Shortcut
{
    public class ShortcutManagerSettings
    {
        public List<Shortcut> Shortcuts { get; set; } = new List<Shortcut>();
        public Shortcut JamesHotKey { get; set; } = new Shortcut() { HotKey = new HotKey(Key.Space, ModifierKeys.Alt) };
        public Shortcut LargeTypeHotKey { get; set; } = new Shortcut() { HotKey = new HotKey(Key.L, ModifierKeys.Alt) };
        public Shortcut SettingsHotKey { get; set; } = new Shortcut() { HotKey = new HotKey(Key.S, ModifierKeys.Alt) };

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
