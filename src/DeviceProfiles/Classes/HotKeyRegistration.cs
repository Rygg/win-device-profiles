using WinApi.User32.Keyboard.NativeTypes;

namespace DeviceProfiles.Classes
{
    /// <summary>
    /// Class structure for holding the necessary information about hot key registrations.
    /// </summary>
    public class HotKeyRegistration
    {
        /// <summary>
        /// ProfileId working as a link to the profile to which this hot key is registered to.
        /// </summary>
        public int ProfileId { get; }
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
        /// <param name="profileId">Id of the profile linked to hotkey</param>
        /// <param name="registrationId">HotKey registration id.</param>
        /// <param name="modifiers">HotKey modifiers</param>
        /// <param name="key">HotKey</param>
        public HotKeyRegistration(int profileId, int registrationId, FsModifiers modifiers, uint key)
        {
            ProfileId = profileId;
            Modifiers = modifiers;
            Key = key;
            HotKeyRegistrationId = registrationId;
        }
    }
}
