using System.Runtime.InteropServices;
using DeviceProfiles.Infrastructure.Environment.Windows.Common.User32.NativeTypes.Enums;

namespace DeviceProfiles.Infrastructure.Environment.Windows.Common.User32.NativeTypes.Structs;

/// <summary>
/// The DISPLAYCONFIG_PATH_INFO structure is used to describe a single path from a target to a source.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct DISPLAYCONFIG_PATH_INFO
{
    /// <summary>
    /// A DISPLAYCONFIG_PATH_SOURCE_INFO structure that contains the source information for the path.
    /// </summary>
    internal DISPLAYCONFIG_PATH_SOURCE_INFO sourceInfo;
    /// <summary>
    /// A DISPLAYCONFIG_PATH_TARGET_INFO structure that contains the target information for the path.
    /// </summary>
    internal DISPLAYCONFIG_PATH_TARGET_INFO targetInfo;
    /// <summary>
    /// A bitwise OR of flag values that indicates the state of the path. The following values are supported:
    /// </summary>
    internal DISPLAYCONFIG_PATH flags;
}