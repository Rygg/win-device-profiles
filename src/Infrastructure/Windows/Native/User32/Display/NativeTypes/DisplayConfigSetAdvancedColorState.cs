using System.Runtime.InteropServices;

namespace Infrastructure.Windows.Native.User32.Display.NativeTypes
{
    /// <summary>
    /// The DISPLAYCONFIG_SET_ADVANCED_COLOR_STATE structure contains information about desired advanced color state.
    /// From: "wingdi.h"
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_SET_ADVANCED_COLOR_STATE
    {
        /// <summary>
        /// A DISPLAYCONFIG_DEVICE_INFO_HEADER structure that contains information about the request for the target device name. The caller should set the type member of DISPLAYCONFIG_DEVICE_INFO_HEADER to DISPLAYCONFIG_DEVICE_INFO_SET_ADVANCED_COLOR_STATE and the adapterId and id members of DISPLAYCONFIG_DEVICE_INFO_HEADER to the target for which the caller wants the target device name. The caller should set the size member of DISPLAYCONFIG_DEVICE_INFO_HEADER to at least the size of the DISPLAYCONFIG_DEVICE_INFO_SET_ADVANCED_COLOR_STATE structure.
        /// </summary>
        public DISPLAYCONFIG_DEVICE_INFO_HEADER header;
        /// <summary>
        /// Value whether or not mode should be enabled. 1 = Enabled, 0 = Disabled.
        /// </summary>
        public DisplayConfigSetAdvancedColorStateValue value;
    }

    /// <summary>
    /// Enumeration of possible values for DISPLAYCONFIG_SET_ADVANCED_COLOR_STATE.value.
    /// </summary>
    public enum DisplayConfigSetAdvancedColorStateValue : uint
    {
        /// <summary>
        /// Disables advanced color state.
        /// </summary>
        Disable = 0x0,
        /// <summary>
        /// Enables advanced color state.
        /// </summary>
        Enable = 0x1,
    }
}
