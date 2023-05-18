using System.Runtime.InteropServices;

namespace Infrastructure.Windows.Native.User32.Display.NativeTypes
{
    /// <summary>
    /// The DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO structure contains information about the device advanced color state. <br/>
    /// From "wingdi.h"
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO
    {
        /// <summary>
        /// A DISPLAYCONFIG_DEVICE_INFO_HEADER structure that contains information about the request for the target device name. The caller should set the type member of DISPLAYCONFIG_DEVICE_INFO_HEADER to DISPLAYCONFIG_DEVICE_INFO_GET_ADVANCED_COLOR_INFO and the adapterId and id members of DISPLAYCONFIG_DEVICE_INFO_HEADER to the target for which the caller wants the target device name. The caller should set the size member of DISPLAYCONFIG_DEVICE_INFO_HEADER to at least the size of the DISPLAYCONFIG_DEVICE_INFO_GET_ADVANCED_COLOR_INFO structure.
        /// </summary>
        public DISPLAYCONFIG_DEVICE_INFO_HEADER header;
        /// <summary>
        /// Value received from the API. Bitwise or flags.
        /// </summary>
        public DISPLAYCONFIG_ADVANCED_COLOR_INFO_VALUE_FLAGS value;
        /// <summary>
        /// Color encoding of the device.
        /// </summary>
        public DISPLAYCONFIG_COLOR_ENCODING colorEncoding;
        /// <summary>
        /// Color channel bits.
        /// </summary>
        public uint bitsPerColorChannel;
    }

    /// <summary>
    /// A bitwise OR of flag values that indicates the advanced color info. The following values are supported:
    /// </summary>
    [Flags]
    public enum DISPLAYCONFIG_ADVANCED_COLOR_INFO_VALUE_FLAGS : uint
    {
        /// <summary>
        /// No support for advanced color.
        /// </summary>
        AdvancedColorNotSupported = 0x0,
        /// <summary>
        /// A type of advanced color is supported
        /// </summary>
        AdvancedColorSupported = 0x1,
        /// <summary>
        /// A type of advanced color is enabled
        /// </summary>
        AdvancedColorEnabled = 0x2,
        /// <summary>
        /// Wide color gamut is enabled
        /// </summary>
        WideColorEnforced = 0x4,
        /// <summary>
        /// Advanced color is force disabled due to system/OS policy
        /// </summary>
        AdvancedColorForceDisabled = 0x8,
    }
}
