namespace DeviceProfiles.Infrastructure.Environment.Windows.Common.User32.NativeTypes.Enums;

/// <summary>
/// The DISPLAYCONFIG_SCANLINE_ORDERING enumeration specifies the method that the display uses to create an image on a screen.
/// </summary>
#pragma warning disable CA1712
internal enum DISPLAYCONFIG_SCANLINE_ORDERING
{
    /// <summary>
    /// Indicates that scan-line ordering of the output is unspecified. The caller can only set the scanLineOrdering member of the DISPLAYCONFIG_PATH_TARGET_INFO structure in a call to the SetDisplayConfig function to DISPLAYCONFIG_SCANLINE_ORDERING_UNSPECIFIED if the caller also set the refresh rate denominator and numerator of the refreshRate member both to zero. In this case, SetDisplayConfig uses the best refresh rate it can find.
    /// </summary>
    DISPLAYCONFIG_SCANLINE_ORDERING_UNSPECIFIED = 0,
    /// <summary>
    /// Indicates that the output is a progressive image.
    /// </summary>
    DISPLAYCONFIG_SCANLINE_ORDERING_PROGRESSIVE = 1,
    /// <summary>
    /// Indicates that the output is an interlaced image that is created beginning with the upper field.
    /// </summary>
    DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED = 2,
    /// <summary>
    /// Indicates that the output is an interlaced image that is created beginning with the upper field.
    /// </summary>
    DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED_UPPERFIELDFIRST = DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED,
    /// <summary>
    /// Indicates that the output is an interlaced image that is created beginning with the lower field.
    /// </summary>
    DISPLAYCONFIG_SCANLINE_ORDERING_INTERLACED_LOWERFIELDFIRST = 3,
}
#pragma warning restore CA1712