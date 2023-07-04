namespace DeviceProfiles.Infrastructure.Environment.Windows.Common.User32.NativeTypes.Enums;


/// <summary>
/// The DISPLAYCONFIG_COLOR_ENCODING enumeration specifies the color encoding of the display.
/// </summary>
#pragma warning disable CA1712
internal enum DISPLAYCONFIG_COLOR_ENCODING
{
    /// <summary>
    /// RGB
    /// </summary>
    DISPLAYCONFIG_COLOR_ENCODING_RGB = 0,
    /// <summary>
    /// YCbCr444
    /// </summary>
    DISPLAYCONFIG_COLOR_ENCODING_YCBCR444 = 1,
    /// <summary>
    /// YCbCR422
    /// </summary>
    DISPLAYCONFIG_COLOR_ENCODING_YCBCR422 = 2,
    /// <summary>
    /// YCbCR420
    /// </summary>
    DISPLAYCONFIG_COLOR_ENCODING_YCBCR420 = 3,
    DISPLAYCONFIG_COLOR_ENCODING_INTENSITY = 4,
}
#pragma warning restore CA1712