using System.Runtime.InteropServices;
using DeviceProfiles.Infrastructure.Environment.Windows.Common.User32.NativeTypes.Enums;

namespace DeviceProfiles.Infrastructure.Environment.Windows.Common.User32.NativeTypes.Structs;

/// <summary>
/// he DISPLAYCONFIG_MODE_INFO structure contains either source mode or target mode information.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct DISPLAYCONFIG_MODE_INFO
{
    /// <summary>
    /// A value that indicates whether the DISPLAYCONFIG_MODE_INFO structure represents source or target mode information. If infoType is DISPLAYCONFIG_MODE_INFO_TYPE_TARGET, the targetMode parameter value contains a valid DISPLAYCONFIG_TARGET_MODE structure describing the specified target. If infoType is DISPLAYCONFIG_MODE_INFO_TYPE_SOURCE, the sourceMode parameter value contains a valid DISPLAYCONFIG_SOURCE_MODE structure describing the specified source.
    /// </summary>
    internal DISPLAYCONFIG_MODE_INFO_TYPE infoType;
    /// <summary>
    /// The source or target identifier on the specified adapter that this path relates to.
    /// </summary>
    internal uint id;
    /// <summary>
    /// The identifier of the adapter that this source or target mode information relates to.
    /// </summary>
    internal LUID adapterId;
    /// <summary>
    /// Info based on infoType
    /// </summary>
    internal DISPLAYCONFIG_MODE_INFO_union info;
}