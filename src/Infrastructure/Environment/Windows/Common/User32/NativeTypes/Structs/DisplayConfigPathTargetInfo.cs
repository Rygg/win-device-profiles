using System.Runtime.InteropServices;
using DeviceProfiles.Infrastructure.Environment.Windows.Common.User32.NativeTypes.Enums;

namespace DeviceProfiles.Infrastructure.Environment.Windows.Common.User32.NativeTypes.Structs;
/// <summary>
/// The DISPLAYCONFIG_PATH_TARGET_INFO structure contains target information for a single path.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct DISPLAYCONFIG_PATH_TARGET_INFO
{
    /// <summary>
    /// The identifier of the adapter that the path is on.
    /// </summary>
    internal LUID adapterId;
    /// <summary>
    /// The target identifier on the specified adapter that this path relates to.
    /// </summary>
    internal uint id;
    /// <summary>
    /// A valid index into the mode information table that contains the target mode information for this path only when DISPLAYCONFIG_PATH_SUPPORT_VIRTUAL_MODE is not set. If target mode information is not available, the value of modeInfoIdx is DISPLAYCONFIG_PATH_MODE_IDX_INVALID.
    /// </summary>
    internal uint modeInfoIdx;
    /// <summary>
    /// The target's connector type. For a list of possible values, see the DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY enumerated type.
    /// </summary>
    internal DISPLAYCONFIG_VIDEO_OUTPUT_TECHNOLOGY outputTechnology;
    /// <summary>
    /// A value that specifies the rotation of the target. For a list of possible values, see the DISPLAYCONFIG_ROTATION enumerated type.
    /// </summary>
    internal DISPLAYCONFIG_ROTATION rotation;
    /// <summary>
    /// A value that specifies how the source image is scaled to the target. For a list of possible values, see the DISPLAYCONFIG_SCALING enumerated type. For more information about scaling, see Scaling the Desktop Image.
    /// </summary>
    internal DISPLAYCONFIG_SCALING scaling;
    /// <summary>
    /// A DISPLAYCONFIG_RATIONAL structure that specifies the refresh rate of the target. If the caller specifies target mode information, the operating system will instead use the refresh rate that is stored in the vSyncFreq member of the DISPLAYCONFIG_VIDEO_SIGNAL_INFO structure. In this case, the caller specifies this value in the targetVideoSignalInfo member of the DISPLAYCONFIG_TARGET_MODE structure. A refresh rate with both the numerator and denominator set to zero indicates that the caller does not specify a refresh rate and the operating system should use the most optimal refresh rate available. For this case, in a call to the SetDisplayConfig function, the caller must set the scanLineOrdering member to the DISPLAYCONFIG_SCANLINE_ORDERING_UNSPECIFIED value; otherwise, SetDisplayConfig fails.
    /// </summary>
    internal DISPLAYCONFIG_RATIONAL refreshRate;
    /// <summary>
    /// A value that specifies the scan-line ordering of the output on the target. For a list of possible values, see the DISPLAYCONFIG_SCANLINE_ORDERING enumerated type. If the caller specifies target mode information, the operating system will instead use the scan-line ordering that is stored in the scanLineOrdering member of the DISPLAYCONFIG_VIDEO_SIGNAL_INFO structure. In this case, the caller specifies this value in the targetVideoSignalInfo member of the DISPLAYCONFIG_TARGET_MODE structure.
    /// </summary>
    internal DISPLAYCONFIG_SCANLINE_ORDERING scanLineOrdering;
    /// <summary>
    /// A Boolean value that specifies whether the target is available. TRUE indicates that the target is available. Because the asynchronous nature of display topology changes when a monitor is removed, a path might still be marked as active even though the monitor has been removed.In such a case, targetAvailable could be FALSE for an active path.This is typically a transient situation that will change after the operating system takes action on the monitor removal.
    /// </summary>
    internal bool targetAvailable;
    /// <summary>
    /// A bitwise OR of flag values that indicates the status of the target. The following values are supported:
    /// </summary>
    internal DISPLAYCONFIG_TARGET_FLAGS statusFlags;
}
