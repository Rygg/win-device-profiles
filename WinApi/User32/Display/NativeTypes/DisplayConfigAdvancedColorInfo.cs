using System.Runtime.InteropServices;

namespace WinApi.User32.Display.NativeTypes
{
    /// <summary>
    /// The DISPLAYCONFIG_ADVANCED_COLOR_INFO structure contains information about the device advanced color state.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DISPLAYCONFIG_ADVANCED_COLOR_INFO
    {
        /// <summary>
        /// A DISPLAYCONFIG_DEVICE_INFO_HEADER structure that contains information about the request for the target device name. The caller should set the type member of DISPLAYCONFIG_DEVICE_INFO_HEADER to DISPLAYCONFIG_DEVICE_INFO_GET_ADVANCED_COLOR_INFO and the adapterId and id members of DISPLAYCONFIG_DEVICE_INFO_HEADER to the target for which the caller wants the target device name. The caller should set the size member of DISPLAYCONFIG_DEVICE_INFO_HEADER to at least the size of the DISPLAYCONFIG_DEVICE_INFO_GET_ADVANCED_COLOR_INFO structure.
        /// </summary>
        public DISPLAYCONFIG_DEVICE_INFO_HEADER header;
        /// <summary>
        /// Value received from the API. Bitwise or flags.
        /// </summary>
        public DISPLAYCONFIG_ADVANCED_COLOR_INFO_FLAGS value;
        /// <summary>
        /// Color encoding of the device.
        /// </summary>
        public DISPLAYCONFIG_COLOR_ENCODING colorEncoding;
        /// <summary>
        /// Color channel bits.
        /// </summary>
        public int bitsPerColorChannel;
    }
}
