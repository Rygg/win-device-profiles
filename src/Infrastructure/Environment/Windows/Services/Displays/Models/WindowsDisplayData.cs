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
    public DISPLAY_DEVICE DisplayDevice { get; init; }

    /// <summary>
    /// DEVMODE from WinAPI.
    /// </summary>
    public DEVMODE deviceMode;

    /// <summary>
    /// DISPLAYCONFIG_ADVANCED_COLOR_INFO from WinApi CCD API
    /// </summary>
    public DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO? AdvancedColorInformation { get; private set; }

    /// <summary>
    /// DISPLAYCONFIG_TARGET_DEVICE_NAME from WinAPI CCD API. Also contains target IDs.
    /// </summary>
    public DISPLAYCONFIG_TARGET_DEVICE_NAME? DisplayTargetInfo { get; private set; }

    /// <summary>
    /// Set TargetInfo to this DisplayData record.
    /// </summary>
    /// <param name="target">TargetInfo to set.</param>
    public void SetTargetInfo(DISPLAYCONFIG_TARGET_DEVICE_NAME target)
    {
        DisplayTargetInfo = target;
    }

    /// <summary>
    /// Set AdvancedColorInfo for this DisplayData record.
    /// </summary>
    /// <param name="colorInfo"></param>
    public void SetAdvancedColorInformation(DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO colorInfo)
    {
        AdvancedColorInformation = colorInfo;
    }

    /// <summary>
    /// Override to string.
    /// </summary>
    /// <returns>Object in a string format.</returns>
    public override string ToString()
    {
        return
            $"DeviceName: {DisplayDevice.DeviceName}:" + System.Environment.NewLine +
            $"Target Monitor Name: {DisplayTargetInfo?.monitorFriendlyDeviceName}" + System.Environment.NewLine +
            $"Device Mode: {deviceMode.dmPelsWidth}x{deviceMode.dmPelsHeight}@{deviceMode.dmDisplayFrequency}Hz" + System.Environment.NewLine +
            $"Device Position: ({deviceMode.dmPosition.x},{deviceMode.dmPosition.y})" + System.Environment.NewLine +
            $"Source Device: {DisplayDevice.DeviceString}" + System.Environment.NewLine +
            $"Device State Flags: {DisplayDevice.StateFlags}" + System.Environment.NewLine +
            $"Device Registry Key: {DisplayDevice.DeviceKey}" + System.Environment.NewLine +
            $"Device Color Encoding: {AdvancedColorInformation?.colorEncoding}, {AdvancedColorInformation?.bitsPerColorChannel} bits per channel" + System.Environment.NewLine +
            $"Advanced Color State: {AdvancedColorInformation?.value}";
    }
}
