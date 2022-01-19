using System;

namespace Win32NativeMethods.User32.Display.NativeTypes
{
    /// <summary>
    /// The ChangeDisplaySettings function changes the settings of the default display device to the specified graphics mode. <br/>
    /// To change the settings of a specified display device, use the ChangeDisplaySettingsEx function.
    /// </summary>
    [Flags()]
    public enum ChangeDisplaySettingsFlags : uint
    {
        /// <summary>
        /// The graphics mode for the current screen will be changed dynamically. 
        /// </summary>
        CDS_NONE = 0,
        /// <summary>
        /// The graphics mode for the current screen will be changed dynamically and the graphics mode will be updated in the registry. The mode information is stored in the USER profile. 
        /// </summary>
        CDS_UPDATEREGISTRY = 0x00000001,
        /// <summary>
        /// The system tests if the requested graphics mode could be set. 
        /// </summary>
        CDS_TEST = 0x00000002,
        /// <summary>
        /// The mode is temporary in nature. If you change to and from another desktop, this mode will not be reset.
        /// </summary>
        CDS_FULLSCREEN = 0x00000004,
        /// <summary>
        /// The settings will be saved in the global settings area so that they will affect all users on the machine. Otherwise, only the settings for the user are modified. This flag is only valid when specified with the CDS_UPDATEREGISTRY flag. 
        /// </summary>
        CDS_GLOBAL = 0x00000008,
        /// <summary>
        /// This device will become the primary device. 
        /// </summary>
        CDS_SET_PRIMARY = 0x00000010,
        CDS_VIDEOPARAMETERS = 0x00000020,
        CDS_ENABLE_UNSAFE_MODES = 0x00000100,
        CDS_DISABLE_UNSAFE_MODES = 0x00000200,
        /// <summary>
        /// The settings should be changed, even if the requested settings are the same as the current settings. 
        /// </summary>
        CDS_RESET = 0x40000000,
        CDS_RESET_EX = 0x20000000,
        /// <summary>
        /// The settings will be saved in the registry, but will not take effect. This flag is only valid when specified with the CDS_UPDATEREGISTRY flag. 
        /// </summary>
        CDS_NORESET = 0x10000000
    }
}
