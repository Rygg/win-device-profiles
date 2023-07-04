namespace DeviceProfiles.Infrastructure.Environment.Windows.Common.User32.NativeTypes.Enums;
/// <summary>
/// The DISPLAYCONFIG_MODE_INFO_TYPE enumeration specifies that the information that is contained within the DISPLAYCONFIG_MODE_INFO structure is either source or target mode.
/// </summary>
#pragma warning disable CA1712
internal enum DISPLAYCONFIG_MODE_INFO_TYPE
{
    /// <summary>
    /// Indicates that the DISPLAYCONFIG_MODE_INFO structure contains source mode information.
    /// </summary>
    DISPLAYCONFIG_MODE_INFO_TYPE_SOURCE = 1,
    /// <summary>
    /// DISPLAYCONFIG_MODE_INFO_TYPE
    /// </summary>
    DISPLAYCONFIG_MODE_INFO_TYPE_TARGET = 2,
    /// <summary>
    /// Indicates that the DISPLAYCONFIG_MODE_INFO structure contains a valid DISPLAYCONFIG_DESKTOP_IMAGE_INFO structure. Supported starting in Windows 10.
    /// </summary>
    DISPLAYCONFIG_MODE_INFO_TYPE_DESKTOP_IMAGE = 3,
}
#pragma warning restore CA1712