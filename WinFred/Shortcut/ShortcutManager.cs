using System;
using System.Linq;
using GlobalHotKey;
using James.HelperClasses;

namespace James.Shortcut
{
    public class ShortcutManager
    {
        private static ShortcutManager _workflowManager;
        private static readonly object SingeltonLock = new object();

        private HotKeyManager _hotkeyManager;
        private ShortcutManagerSettings _managerSettings;

        private ShortcutManager()
        {
            Reload();
        }

        /// <summary>
        /// Clears all current active Shortcut listeners and reloads the shortcuts to listen
        /// </summary>
        public void Reload()
        {
            _managerSettings = Config.Instance.ShortcutManagerSettings;
            _hotkeyManager?.Dispose();
            _hotkeyManager = new HotKeyManager();
            _hotkeyManager.KeyPressed += HotKeyPressed;
            _hotkeyManager.Register(_managerSettings.JamesHotKey.HotKey.ToGlobalHotKey());
            foreach (Shortcut item in _managerSettings.Shortcuts)
            {
                _hotkeyManager.Register(item.HotKey.ToGlobalHotKey());
            }
        }

        public delegate void ShortcutPressedEventHandler(object sender, EventArgs e);
        public event ShortcutPressedEventHandler ShortcutPressed;

        /// <summary>
        /// determine what should happen if a HotKey got pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HotKeyPressed(object sender, KeyPressedEventArgs e)
        {
            Shortcut result;
            var jamesHotKey = _managerSettings.JamesHotKey.HotKey;
            if (e.HotKey.Key == jamesHotKey.Key && e.HotKey.Modifiers == jamesHotKey.ModifierKeys)
            {
                result = _managerSettings.JamesHotKey;
            }
            else
            {
                result = _managerSettings.Shortcuts.FirstOrDefault(key => key.HotKey.Key == e.HotKey.Key && key.HotKey.ModifierKeys == e.HotKey.Modifiers); 
            }
            ShortcutPressed?.Invoke(result, new EventArgs());
        }

        public static ShortcutManager Instance
        {
            get
            {
                lock (SingeltonLock)
                {
                    return _workflowManager ?? (_workflowManager = new ShortcutManager());
                }
            }
        }
    }
}
