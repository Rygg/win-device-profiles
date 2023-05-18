using System.Runtime.InteropServices;
using Infrastructure.Environment.Windows.Common.User32.NativeTypes.Enums;

namespace Infrastructure.Environment.Windows.Common.User32.NativeTypes.Structs;
/// <summary>
/// The DISPLAYCONFIG_PATH_SOURCE_INFO structure contains source information for a single path.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct DISPLAYCONFIG_PATH_SOURCE_INFO
{
    /// <summary>
    /// The identifier of the adapter that this source information relates to.
    /// </summary>
    internal LUID adapterId;
    /// <summary>
    /// The source identifier on the specified adapter that this path relates to.
    /// </summary>
    internal uint id;
    /// <summary>
    /// A valid index into the mode information table that contains the source mode information for this path only when DISPLAYCONFIG_PATH_SUPPORT_VIRTUAL_MODE is not set. If source mode information is not available, the value of modeInfoIdx is DISPLAYCONFIG_PATH_MODE_IDX_INVALID.
    /// </summary>
    internal uint modeInfoIdx;
    /// <summary>
    /// A bitwise OR of flag values that indicates the status of the source. The following values are supported: DISPLAYCONFIG_SOURCE_IN_USE
    /// </summary>
    internal DISPLAYCONFIG_SOURCE_FLAGS statusFlags;
}