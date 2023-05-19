using System.ComponentModel;
using System.Runtime.InteropServices;
using Application.Common.Interfaces;
using Domain.Models;
using Infrastructure.Environment.Windows.Common.User32;
using Infrastructure.Environment.Windows.Common.User32.NativeTypes.Enums;
using Infrastructure.Environment.Windows.Common.User32.NativeTypes.Structs;
using Infrastructure.Environment.Windows.Services.Displays.Models;
using Infrastructure.Extensions;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Environment.Windows.Services.Displays;

/// <summary>
/// Service for controlling all things related to Windows environment Display controlling.
/// </summary>
public sealed class DisplayDeviceService : IDisplayDeviceController
{
    private readonly ILogger<DisplayDeviceService> _logger;

    public DisplayDeviceService(ILogger<DisplayDeviceService> logger)
    {
        _logger = logger;
    }

    public Task<bool> ChangeDisplaySettings(DeviceProfile profile, CancellationToken cancellationToken)
    {
        return Task.FromResult(false);
    }

    /// <summary>
    /// Gets retrieved display information as a string.
    /// </summary>
    /// <param name="cancellationToken">CancellationToken to cancel the operation.</param>
    /// <returns>A formatted string containing the current display information.</returns>
    public Task<string> GetCurrentDisplayInformationString(CancellationToken cancellationToken)
    {
        var currentDisplays = RetrieveSystemDisplayInformation(cancellationToken);
        var str = string.Empty;
        foreach (var (id, display) in currentDisplays)
        {
            str += $"DisplayId {id}: {System.Environment.NewLine}{display}" + System.Environment.NewLine + System.Environment.NewLine;
        }
        return Task.FromResult(str);
    }

    /// <summary>
    /// Retrieve all display information from the system.
    /// </summary>
    /// <param name="cancellationToken">CancellationToken for the operation.</param>
    /// <returns>Dictionary containing the environment DisplayIds and a model containing all the display information.</returns>
    /// <exception cref="Win32Exception">Something went wrong with the native library communication.</exception>
    private IDictionary<uint, WindowsDisplayData> RetrieveSystemDisplayInformation(CancellationToken cancellationToken)
    {
        var displays = new Dictionary<uint,WindowsDisplayData>();
        _logger.RetrievingDisplayDevices();
        uint displayId = 0;
        while (!cancellationToken.IsCancellationRequested)
        {
            var device = new DISPLAY_DEVICE();
            device.cb = Marshal.SizeOf(device);
            if (!Display.EnumDisplayDevices(null, displayId, ref device, 0)) // Loop uints until EnumDisplayDevices returns false. (No display device at this id).
            {
                break; // Break out.
            }

            _logger.DisplayRetrieved(displayId, device.ToString());

            if (!device.StateFlags.HasFlag(DisplayDeviceStateFlags.AttachedToDesktop))
            {
                displayId++;
                _logger.DeviceNotAttachedToDesktop();
                continue;
            }
            _logger.RetrievingDeviceModes(displayId);
            var deviceMode = new DEVMODE();
            if (!Display.EnumDisplaySettings(device.DeviceName, -1, ref deviceMode))
            {
                throw new Win32Exception($"DeviceMode not located for displayId {displayId}."); // Every device should have a device mode.
            }

            _logger.DeviceModeRetrieved(displayId, deviceMode.ToString());
            displays.Add(displayId, new WindowsDisplayData
            {
                DisplayDevice = device,
                DeviceMode = deviceMode
            });

            displayId++;
        }

        // Update advanced additional information to the retrieved displays.
        UpdateRetrievedDisplaysWithAdvancedInformation(displays); 
        
        foreach (var (id, display) in displays)
        {
            _logger.RetrievedDisplayInformation(id, display.ToString());
        }

        _logger.DisplaysRefreshed();
        return displays;
    }

    /// <summary>
    /// Method uses additional CCD API to retrieve additional information for the retrieved display devices.
    /// </summary>
    /// <exception cref="Win32Exception">Something went wrong in the Native methods.</exception>
    /// <exception cref="InvalidOperationException"></exception>
    private void UpdateRetrievedDisplaysWithAdvancedInformation(IDictionary<uint,WindowsDisplayData> displays)
    {
        _logger.RetrievingAdvancedDisplayInformation();
        if (!displays.Any())
        {
            return;
        }
        // Get buffer sizes for the active displays.
        var err = Display.GetDisplayConfigBufferSizes(QDC.QDC_ONLY_ACTIVE_PATHS, out var pathCount, out var modeCount);
        if (err != ResultErrorCode.ERROR_SUCCESS)
        {
            throw new Win32Exception((int)err);
        }
        var paths = new DISPLAYCONFIG_PATH_INFO[pathCount]; // Create arrays to hold the info.
        var modes = new DISPLAYCONFIG_MODE_INFO[modeCount];

        // Get display configs from CCD API:
        err = Display.QueryDisplayConfig(QDC.QDC_ONLY_ACTIVE_PATHS, ref pathCount, paths, ref modeCount, modes, IntPtr.Zero);
        if (err != ResultErrorCode.ERROR_SUCCESS)
        {
            throw new Win32Exception((int)err);
        }

        // Loop through the active display paths:
        foreach (var path in paths)
        {
            // Check if the display exists.
            if (!displays.ContainsKey(path.sourceInfo.id))
            {
                _logger.RetrievedDeviceSourceNotFound();
                continue;
            }
            // Get the source information. This is to further check mapping so that each device data is mapped correctly.
            var sourceInfo = GetDisplayConfigSourceDeviceName(path);

            // Check if the device names match.
            if (displays[path.sourceInfo.id].DisplayDevice.DeviceName != sourceInfo.viewGdiDeviceName)
            {
                throw new InvalidOperationException($"DeviceNames did not match for DisplayId: {path.sourceInfo.id}");
            }
            // Get the target device name.
            var targetInfo = GetDisplayConfigTargetDeviceName(path);
            displays[path.sourceInfo.id].DisplayTargetInfo = targetInfo; // Save the device name to memory.

            // Get advanced color info (HDR).
            var colorInfo = GetDisplayConfigAdvancedColorInfo(path);
            displays[path.sourceInfo.id].AdvancedColorInformation = colorInfo; // Save to memory.
        }
    }

    /// <summary>
    /// Get DisplayConfig Source Device Name from the Native Methods.
    /// </summary>
    /// <param name="path">Path information to query.</param>
    /// <returns>a native struct.</returns>
    /// <exception cref="Win32Exception">Error occurred in native query.</exception>
    private static DISPLAYCONFIG_SOURCE_DEVICE_NAME GetDisplayConfigSourceDeviceName(DISPLAYCONFIG_PATH_INFO path)
    {
        var sourceInfo = new DISPLAYCONFIG_SOURCE_DEVICE_NAME();
        sourceInfo.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_SOURCE_NAME;
        sourceInfo.header.size = Marshal.SizeOf<DISPLAYCONFIG_SOURCE_DEVICE_NAME>();
        sourceInfo.header.adapterId = path.sourceInfo.adapterId;
        sourceInfo.header.id = path.sourceInfo.id;
        var err = Display.DisplayConfigGetDeviceInfo(ref sourceInfo); // Call for info.
        if (err != ResultErrorCode.ERROR_SUCCESS)
        {
            throw new Win32Exception((int)err, $"Error occurred in {nameof(GetDisplayConfigSourceDeviceName)}");
        }

        return sourceInfo;
    }

    /// <summary>
    /// Get DisplayConfig Target Device Name from the Native Methods.
    /// </summary>
    /// <param name="path">Path information to query.</param>
    /// <returns>a native struct.</returns>
    /// <exception cref="Win32Exception">Error occurred in the native query.</exception>
    private static DISPLAYCONFIG_TARGET_DEVICE_NAME GetDisplayConfigTargetDeviceName(DISPLAYCONFIG_PATH_INFO path)
    {
        var targetInfo = new DISPLAYCONFIG_TARGET_DEVICE_NAME();
        targetInfo.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME;
        targetInfo.header.size = Marshal.SizeOf<DISPLAYCONFIG_TARGET_DEVICE_NAME>();
        targetInfo.header.adapterId = path.targetInfo.adapterId;
        targetInfo.header.id = path.targetInfo.id;
        var err = Display.DisplayConfigGetDeviceInfo(ref targetInfo); // Request target device information:
        if (err != ResultErrorCode.ERROR_SUCCESS)
        {
            throw new Win32Exception((int)err, $"Error occurred in {nameof(GetDisplayConfigTargetDeviceName)}");
        }

        return targetInfo;
    }
    /// <summary>
    /// Get DisplayConfig Advanced Color Info from the Native methods.
    /// </summary>
    /// <param name="path">Path information to query.</param>
    /// <returns>a native struct</returns>
    /// <exception cref="Win32Exception">Error occurred in the native query</exception>
    private static DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO GetDisplayConfigAdvancedColorInfo(DISPLAYCONFIG_PATH_INFO path)
    {
        var colorInfo = new DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO();
        colorInfo.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_ADVANCED_COLOR_INFO;
        colorInfo.header.size = Marshal.SizeOf<DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO>();
        colorInfo.header.adapterId = path.targetInfo.adapterId;
        colorInfo.header.id = path.targetInfo.id;
        var err = Display.DisplayConfigGetDeviceInfo(ref colorInfo);
        if (err != ResultErrorCode.ERROR_SUCCESS)
        {
            throw new Win32Exception((int)err, $"Error occurred in {nameof(GetDisplayConfigAdvancedColorInfo)}");
        }

        return colorInfo;
    }
}