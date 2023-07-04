namespace DeviceProfiles.Infrastructure.Environment.Windows.Common.User32.NativeTypes.Enums;

/// <summary>
/// A bitwise OR of flag values that indicates the status of the source. The following values are supported:
/// </summary>
[Flags]
internal enum DISPLAYCONFIG_SOURCE_FLAGS
{
    /// <summary>
    /// This source is in use by at least one active path.
    /// </summary>
    DISPLAYCONFIG_SOURCE_IN_USE = 0x00000001,
}