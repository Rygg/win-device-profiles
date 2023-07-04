using System.Runtime.InteropServices;
using DeviceProfiles.Infrastructure.Environment.Windows.Common.User32.NativeTypes.Enums;

namespace DeviceProfiles.Infrastructure.Environment.Windows.Common.User32.NativeTypes.Structs;

/// <summary>
/// The DISPLAY_DEVICE structure receives information about the display device specified by the iDevNum parameter of the EnumDisplayDevices function.
/// </summary>
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
internal struct DISPLAY_DEVICE
{
    /// <summary>
    /// Size, in bytes, of the DISPLAY_DEVICE structure. This must be initialized prior to calling EnumDisplayDevices.
    /// </summary>
    [MarshalAs(UnmanagedType.U4)]
    internal int cb;
    /// <summary>
    /// An array of characters identifying the device name. This is either the adapter device or the monitor device.
    /// </summary>
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    internal string DeviceName;
    /// <summary>
    /// An array of characters containing the device context string. This is either a description of the display adapter or of the display monitor.
    /// </summary>
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    internal string DeviceString;
    /// <summary>
    /// Device state flags.
    /// </summary>
    [MarshalAs(UnmanagedType.U4)]
    internal DisplayDeviceStateFlags StateFlags;
    /// <summary>
    /// Not used.
    /// </summary>
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    internal string DeviceID;
    /// <summary>
    /// Reserved.
    /// </summary>
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    internal string DeviceKey;

    /// <summary>
    /// Override for converting this to a string.
    /// </summary>
    public override string ToString()
    {
        return $"{nameof(DISPLAY_DEVICE)}| DeviceName: {DeviceName}, DeviceId: {DeviceID}, DeviceString: {DeviceString}, DeviceKey: {DeviceKey}, StateFlags: {StateFlags}";
    }
}