using System;
using System.Windows.Forms;
using DeviceProfiles.Enums;

namespace DeviceProfiles.Classes
{
    /// <summary>
    /// Event Arguments for the key press events.
    /// </summary>
    internal class KeyPressedEventArgs : EventArgs
    {   
        /// <summary>
        /// Pressed Modifier keys.
        /// </summary>
        public EKeyModifiers Modifier { get; }
        /// <summary>
        /// Pressed key.
        /// </summary>
        public Keys Key { get; }
        /// <summary>
        /// Hotkey registration Id.
        /// </summary>
        public int RegistrationId { get; }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="modifier"></param>
        /// <param name="key"></param>
        /// <param name="registrationId"></param>
        internal KeyPressedEventArgs(EKeyModifiers modifier, Keys key, int registrationId)
        {
            Modifier = modifier;
            Key = key;
            RegistrationId = registrationId;
        }

    }
}
