namespace Infrastructure.Environment.Windows.Common.User32.NativeTypes.Enums;

/// <summary>
/// The DISPLAYCONFIG_ROTATION enumeration specifies the clockwise rotation of the display.
/// </summary>
#pragma warning disable CA1712
internal enum DISPLAYCONFIG_ROTATION
{
    /// <summary>
    /// Indicates that rotation is 0 degrees—landscape mode.
    /// </summary>
    DISPLAYCONFIG_ROTATION_IDENTITY = 1,
    /// <summary>
    /// Indicates that rotation is 90 degrees clockwise—portrait mode.
    /// </summary>
    DISPLAYCONFIG_ROTATION_ROTATE90 = 2,
    /// <summary>
    /// Indicates that rotation is 180 degrees clockwise—inverted landscape mode.
    /// </summary>
    DISPLAYCONFIG_ROTATION_ROTATE180 = 3,
    /// <summary>
    /// Indicates that rotation is 270 degrees clockwise—inverted portrait mode.
    /// </summary>
    DISPLAYCONFIG_ROTATION_ROTATE270 = 4,
}
#pragma warning restore CA1712