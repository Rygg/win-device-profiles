using System;

namespace WinApi.User32.Display.NativeTypes
{
    /// <summary>
    /// A bitwise OR of flag values that indicates the advanced color info. The following values are supported:
    /// </summary>
    [Flags]
    public enum DISPLAYCONFIG_ADVANCED_COLOR_INFO_FLAGS : uint
    {
        /// <summary>
        /// No values.
        /// </summary>
        None = 0x0,
        /// <summary>
        /// AdvancedColor is supported.
        /// </summary>
        AdvancedColorSupported = 0x1,
        /// <summary>
        /// AdvancedColor is enabled.
        /// </summary>
        AdvancedColorEnabled = 0x2,
        /// <summary>
        /// WideColor is enforced.
        /// </summary>
        WideColorEnforced = 0x4,
        /// <summary>
        /// AdvancedColor is disabled by force.
        /// </summary>
        AdvancedColorForceDisabled = 0x8,
    }
}
