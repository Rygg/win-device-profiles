using System;
using System.Windows.Forms;

namespace DisplayController.App.Control.Types
{
    /// <summary>
    /// Event Arguments for the key press events.
    /// </summary>
    internal class KeyPressedEventArgs : EventArgs
    {
        private KeyModifiers _modifier;
        private Keys _key;
        private int _registrationId;

        internal KeyPressedEventArgs(KeyModifiers modifier, Keys key, int registrationId)
        {
            _modifier = modifier;
            _key = key;
            _registrationId = registrationId;
        }
        /// <summary>
        /// Pressed Modifier keys.
        /// </summary>
        public KeyModifiers Modifier
        {
            get { return _modifier; }
        }
        /// <summary>
        /// Pressed key.
        /// </summary>
        public Keys Key
        {
            get { return _key; }
        }
        /// <summary>
        /// Hotkey registration Id.
        /// </summary>
        public int RegistrationId
        {
            get { return _registrationId; }
        }
    }
}
