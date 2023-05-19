using Infrastructure.Environment.Windows.Common.User32.NativeTypes.Structs;

namespace Infrastructure.Environment.Windows.Services.Displays.Models;

/// <summary>
/// Record for containing windows display data.
/// </summary>
internal sealed record WindowsDisplayData
{
    /// <summary>
    /// DISPLAY_DEVICE from WinAPI.
    /// </summary>
    public DISPLAY_DEVICE DisplayDevice;

    /// <summary>
    /// DEVMODE from WinAPI.
    /// </summary>
    public DEVMODE DeviceMode;

    /// <summary>
    /// DISPLAYCONFIG_ADVANCED_COLOR_INFO from WinApi CCD API
    /// </summary>
    public DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO? AdvancedColorInformation;

    /// <summary>
    /// DISPLAYCONFIG_TARGET_DEVICE_NAME from WinAPI CCD API. Also contains target IDs.
    /// </summary>
    public DISPLAYCONFIG_TARGET_DEVICE_NAME? DisplayTargetInfo;


    /// <summary>
    /// Override to string.
    /// </summary>
    /// <returns>Object in a string format.</returns>
    public override string ToString()
    {
        return
            $"DeviceName: {DisplayDevice.DeviceName}:" + System.Environment.NewLine +
            $"Target Monitor Name: {DisplayTargetInfo?.monitorFriendlyDeviceName}" + System.Environment.NewLine +
            $"Device Mode: {DeviceMode.dmPelsWidth}x{DeviceMode.dmPelsHeight}@{DeviceMode.dmDisplayFrequency}Hz" + System.Environment.NewLine +
            $"Device Position: ({DeviceMode.dmPosition.x},{DeviceMode.dmPosition.y})" + System.Environment.NewLine +
            $"Source Device: {DisplayDevice.DeviceString}" + System.Environment.NewLine +
            $"Device State Flags: {DisplayDevice.StateFlags}" + System.Environment.NewLine +
            $"Device Registry Key: {DisplayDevice.DeviceKey}" + System.Environment.NewLine +
            $"Device Color Encoding: {AdvancedColorInformation?.colorEncoding}, {AdvancedColorInformation?.bitsPerColorChannel} bits per channel" + System.Environment.NewLine +
            $"Advanced Color State: {AdvancedColorInformation?.value}";
    }
}
