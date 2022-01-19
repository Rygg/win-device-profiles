using WinApi.User32.Keyboard.NativeTypes;

namespace DeviceProfiles.App.Control.Types
{
    /// <summary>
    /// Class structure for holding the necessary information about hot key registrations.
    /// </summary>
    public class HotKeyRegistration
    {
        /// <summary>
        /// ProfileName working as a link to the profile to which this hot key is registered to.
        /// </summary>
        public string ProfileName { get; }
        /// <summary>
        /// Modifiers.
        /// </summary>
        public FsModifiers Modifiers { get; }
        /// <summary>
        /// Key.
        /// </summary>
        public uint Key { get; }
        /// <summary>
        /// HotKeyRegistrationId. Required to link the event to the profile.
        /// </summary>
        public int HotKeyRegistrationId { get; }
        /// <summary>
        /// Constructor for HotKeyRegistration.
        /// </summary>
        /// <param name="profileName">Name of the profile linked to hotkey</param>
        /// <param name="registrationId">HotKey registration id.</param>
        /// <param name="modifiers">HotKey modifiers</param>
        /// <param name="key">HotKey</param>
        public HotKeyRegistration(string profileName, int registrationId, FsModifiers modifiers, uint key)
        {
            ProfileName = profileName;
            Modifiers = modifiers;
            Key = key;
            HotKeyRegistrationId = registrationId;
        }
    }
}
