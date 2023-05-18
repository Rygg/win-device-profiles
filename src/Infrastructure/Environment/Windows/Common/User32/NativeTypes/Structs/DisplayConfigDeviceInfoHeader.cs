using System.Runtime.InteropServices;
using Infrastructure.Environment.Windows.Common.User32.NativeTypes.Enums;

namespace Infrastructure.Environment.Windows.Common.User32.NativeTypes.Structs;

/// <summary>
/// The DISPLAYCONFIG_DEVICE_INFO_HEADER structure contains display information about the device.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct DISPLAYCONFIG_DEVICE_INFO_HEADER
{
    /// <summary>
    /// A DISPLAYCONFIG_DEVICE_INFO_TYPE enumerated value that determines the type of device information to retrieve or set. The remainder of the packet for the retrieve or set operation follows immediately after the DISPLAYCONFIG_DEVICE_INFO_HEADER structure.
    /// </summary>
    internal DISPLAYCONFIG_DEVICE_INFO_TYPE type;
    /// <summary>
    /// The size, in bytes, of the device information that is retrieved or set. This size includes the size of the header and the size of the additional data that follows the header. This device information depends on the request type.
    /// </summary>
    internal int size;
    /// <summary>
    /// A locally unique identifier (LUID) that identifies the adapter that the device information packet refers to.
    /// </summary>
    internal LUID adapterId;
    /// <summary>
    /// The source or target identifier to get or set the device information for. The meaning of this identifier is related to the type of information being requested. For example, in the case of DISPLAYCONFIG_DEVICE_INFO_GET_SOURCE_NAME, this is the source identifier.
    /// </summary>
    internal uint id;
}