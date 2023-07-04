using System.Runtime.InteropServices;
using DeviceProfiles.Infrastructure.Environment.Windows.Common.User32.NativeTypes.Enums;

namespace DeviceProfiles.Infrastructure.Environment.Windows.Common.User32.NativeTypes.Structs;

/// <summary>
/// The DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO structure contains information about the device advanced color state. <br/>
/// From "wingdi.h"
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO
{
    /// <summary>
    /// A DISPLAYCONFIG_DEVICE_INFO_HEADER structure that contains information about the request for the target device name. The caller should set the type member of DISPLAYCONFIG_DEVICE_INFO_HEADER to DISPLAYCONFIG_DEVICE_INFO_GET_ADVANCED_COLOR_INFO and the adapterId and id members of DISPLAYCONFIG_DEVICE_INFO_HEADER to the target for which the caller wants the target device name. The caller should set the size member of DISPLAYCONFIG_DEVICE_INFO_HEADER to at least the size of the DISPLAYCONFIG_DEVICE_INFO_GET_ADVANCED_COLOR_INFO structure.
    /// </summary>
    internal DISPLAYCONFIG_DEVICE_INFO_HEADER header;
    /// <summary>
    /// Value received from the API. Bitwise or flags.
    /// </summary>
    internal DISPLAYCONFIG_ADVANCED_COLOR_INFO_VALUE_FLAGS value;
    /// <summary>
    /// Color encoding of the device.
    /// </summary>
    internal DISPLAYCONFIG_COLOR_ENCODING colorEncoding;
    /// <summary>
    /// Color channel bits.
    /// </summary>
    internal uint bitsPerColorChannel;
}