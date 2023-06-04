namespace Infrastructure.Environment.Windows.Common.User32.NativeTypes.Enums;

/// <summary>
/// A bitwise OR of flag values that indicates the advanced color info. The following values are supported:
/// </summary>
[Flags]
internal enum DISPLAYCONFIG_ADVANCED_COLOR_INFO_VALUE_FLAGS : uint
{
    /// <summary>
    /// No support for advanced color.
    /// </summary>
    AdvancedColorNotSupported = 0x0,
    /// <summary>
    /// A type of advanced color is supported
    /// </summary>
    AdvancedColorSupported = 0x1,
    /// <summary>
    /// A type of advanced color is enabled
    /// </summary>
    AdvancedColorEnabled = 0x2,
    /// <summary>
    /// Wide color gamut is enabled
    /// </summary>
    WideColorEnforced = 0x4,
    /// <summary>
    /// Advanced color is force disabled due to system/OS policy
    /// </summary>
    AdvancedColorForceDisabled = 0x8,
}
