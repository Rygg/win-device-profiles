using System;

namespace WinApi.User32.Display.NativeTypes
{
    /// <summary>
    /// A bitwise OR of flag values that indicates the state of the path. The following values are supported:
    /// </summary>
    [Flags]
    public enum DISPLAYCONFIG_PATH
    {
        /// <summary>
        /// Set by QueryDisplayConfig to indicate that the path is active and part of the desktop. If this flag value is set, SetDisplayConfig attempts to enable this path. 
        /// </summary>
        DISPLAYCONFIG_PATH_ACTIVE = 0x00000001,
        /// <summary>
        /// Set by QueryDisplayConfig to indicate that the path supports virtual modes. Supported starting in Windows 10. 
        /// </summary>
        DISPLAYCONFIG_PATH_PREFERRED_UNSCALED = 0x00000004,
        /// <summary>
        /// Set by QueryDisplayConfig to indicate that the path supports virtual refresh rates. Supported starting in Windows 11. 
        /// </summary>
        DISPLAYCONFIG_PATH_SUPPORT_VIRTUAL_MODE = 0x00000008,
    }
}
