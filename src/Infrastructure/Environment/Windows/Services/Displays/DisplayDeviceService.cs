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

    /// <summary>
    /// Changes the current system display settings to the given DeviceProfile.
    /// </summary>
    /// <param name="profile">DeviceProfile to be set as active.</param>
    /// <param name="cancellationToken">CancellationToken to cancel the operation.</param>
    /// <returns>True if the operation was successful. Otherwise false.</returns>
    public Task<bool> ChangeDisplaySettings(DeviceProfile profile, CancellationToken cancellationToken)
    {
        if (profile == null)
        {
            throw new ArgumentNullException(nameof(profile));
        }

        var displays = RetrieveSystemDisplayInformation(cancellationToken);
        var displaysForStandardSettings = profile.DisplaySettings.Where(ds => ds.PrimaryDisplay != null || ds.RefreshRate != null && ds.RefreshRate != 0);
        var displaysForAdvancedColorState = profile.DisplaySettings.Where(ds => ds.EnableHdr != null);

        SetStandardDisplaySettings(displaysForStandardSettings, displays, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromResult(false);
        }
        SetAdvancedColorDisplaySettings(displaysForAdvancedColorState, displays);
        return Task.FromResult(true);
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

    /// <summary>
    /// Method updating Display Settings using the Standard Win API.
    /// </summary>
    /// <param name="displaySettings">Array of display settings to update.</param>
    /// <param name="currentDisplays">Current displays in the system.</param>
    /// <param name="cancellationToken">CancellationToken to cancel the operation.</param>
    private void SetStandardDisplaySettings(IEnumerable<DisplaySettings> displaySettings, IDictionary<uint, WindowsDisplayData> currentDisplays, CancellationToken cancellationToken)
    {
        var anyRegistryChanges = false;
        var primaryDisplaySet = false;
        foreach (var display in displaySettings) // Set all the required configuration values to the registry.
        {
            if (display.PrimaryDisplay == true)
            {
                if (!primaryDisplaySet)
                {
                    _logger.SettingDisplayAsPrimary(display.DisplayId);
                    if (SetPrimaryDisplayToRegistry(display.DisplayId, currentDisplays))
                    {
                        anyRegistryChanges = true;
                    }
                    primaryDisplaySet = true;
                }
                else
                {
                    _logger.PrimaryDisplayAlreadySet(display.DisplayId);
                }
            }
            if (display.RefreshRate != null && display.RefreshRate != 0)
            {
                _logger.SettingDisplayRefreshRate(display.DisplayId, display.RefreshRate);
                if (SetDisplayRefreshRateToRegistry(display.DisplayId, display.RefreshRate.Value, currentDisplays))
                {
                    anyRegistryChanges = true;
                }
            }
        }
        if (anyRegistryChanges && !cancellationToken.IsCancellationRequested) // No need to apply unless changes were made. Check for cancellation also.
        {
            Display.ChangeDisplaySettingsEx(null, IntPtr.Zero, (IntPtr)null, ChangeDisplaySettingsFlags.CDS_NONE, (IntPtr)null); // Apply settings:
        }
    }

    /// <summary>
    /// Method sets the parameter as the new primary display to the registry, does not apply changes.
    /// </summary>
    /// <param name="displayId">Id of the new primary display.</param>
    /// <param name="currentDisplays">Currently retrieved displays</param>
    /// <returns>Returns true if any changes were made to the registry.</returns>
    private bool SetPrimaryDisplayToRegistry(uint displayId, IDictionary<uint, WindowsDisplayData> currentDisplays)
    {
        if (!currentDisplays.ContainsKey(displayId))
        {
            _logger.DisplayNotFound(displayId);
            return false;
        }

        var newPrimary = currentDisplays[displayId];
        if (newPrimary.DisplayDevice.StateFlags.HasFlag(DisplayDeviceStateFlags.PrimaryDevice))
        {
            _logger.DisplayWasAlreadyPrimary(displayId);
            return false;
        }

        var offsetX = newPrimary.DeviceMode.dmPosition.x; // Get old position.
        var offsetY = newPrimary.DeviceMode.dmPosition.y;
        newPrimary.DeviceMode.dmPosition.x = 0; // Set new as 0,0 (Primary is always 0,0)
        newPrimary.DeviceMode.dmPosition.y = 0;

        // Change values to registry. Don't take effect yet.
        Display.ChangeDisplaySettingsEx(newPrimary.DisplayDevice.DeviceName, ref newPrimary.DeviceMode, (IntPtr)null, (ChangeDisplaySettingsFlags.CDS_SET_PRIMARY | ChangeDisplaySettingsFlags.CDS_UPDATEREGISTRY | ChangeDisplaySettingsFlags.CDS_NORESET), IntPtr.Zero);
        _logger.UpdatedRegistrySettings(displayId);
        // Update the offsets of the rest of the displays:
        var otherDisplays = currentDisplays.Where(d => d.Key != displayId);

        foreach (var (id, display) in otherDisplays)
        {
            display.DeviceMode.dmPosition.x -= offsetX; // Subtract old primary display offset to get correct new screen position.
            display.DeviceMode.dmPosition.y -= offsetY;
            // Change values to registry, don't apply changes yet.
            Display.ChangeDisplaySettingsEx(display.DisplayDevice.DeviceName, ref display.DeviceMode, (IntPtr)null, (ChangeDisplaySettingsFlags.CDS_UPDATEREGISTRY | ChangeDisplaySettingsFlags.CDS_NORESET), IntPtr.Zero);
            _logger.UpdatedRegistrySettings(id);
        }

        return true;
    }

    /// <summary>
    /// Method sets the displays refresh rate to the new value, if it is supported.
    /// </summary>
    /// <param name="displayId">Id of the new primary display.</param>
    /// <param name="newRefreshRate">New refresh rate for the display.</param>
    /// <param name="currentDisplays">Current displays in the system.</param>
    /// <returns>Returns a boolean indicating whether changes were made to the registry</returns>
    private bool SetDisplayRefreshRateToRegistry(uint displayId, int newRefreshRate, IDictionary<uint, WindowsDisplayData> currentDisplays)
    {
        if (!currentDisplays.ContainsKey(displayId))
        {
            _logger.DisplayNotFound(displayId);
            return false;
        }
        var display = currentDisplays[displayId];
        var oldRefreshRate = display.DeviceMode.dmDisplayFrequency;
        if (oldRefreshRate == newRefreshRate)
        {
            _logger.DisplayRefreshRateWasAlreadySet();
            return false;
        }

        // Test can the refresh frequency be set for this display:
        display.DeviceMode.dmDisplayFrequency = newRefreshRate;
        
        var testResult = Display.ChangeDisplaySettingsEx(display.DisplayDevice.DeviceName, ref display.DeviceMode, (IntPtr)null, ChangeDisplaySettingsFlags.CDS_TEST, IntPtr.Zero);
        if (testResult != DISP_CHANGE.Successful)
        {
            _logger.RefreshRateNotSupported(displayId, newRefreshRate);
            display.DeviceMode.dmDisplayFrequency = oldRefreshRate;
            return false;
        }
        // Valid refresh rate. Update to registry.
        var result = Display.ChangeDisplaySettingsEx(display.DisplayDevice.DeviceName, ref display.DeviceMode, (IntPtr)null, (ChangeDisplaySettingsFlags.CDS_UPDATEREGISTRY | ChangeDisplaySettingsFlags.CDS_NORESET), IntPtr.Zero);
        if (result != DISP_CHANGE.Successful)
        {
            throw new Win32Exception((int)result, $"Updating monitor {displayId} refresh rate failed.");
        }
        _logger.UpdatedRegistrySettings(displayId);
        return true;
    }


    /// <summary>
    /// Method updating Advanced Display Settings using the advanced Win API.
    /// </summary>
    /// <param name="displaySettings">Array of display settings to update.</param>
    /// <param name="currentDisplays">Current displays in the system</param>
    private void SetAdvancedColorDisplaySettings(IEnumerable<DisplaySettings> displaySettings, IDictionary<uint, WindowsDisplayData> currentDisplays)
    {
        foreach (var display in displaySettings) // Update advanced color state values for required displays.
        {
            _logger.UpdatingAdvancedColorState(display.DisplayId, display.EnableHdr!.Value); // Suppress nullable, should have been validated before.
            if (SetDisplayAdvancedColorState(display.DisplayId, display.EnableHdr!.Value, currentDisplays)) // Suppress nullable, should have been validated before.
            {
                _logger.AdvancedColorStateUpdated(display.DisplayId);
            }
        }
    }

    /// <summary>
    /// Method sets the advanced color state of the given monitor to the desired value.
    /// </summary>
    /// <param name="displayId">DisplayId to be operated.</param>
    /// <param name="newState">New state of the advanced color mode.</param>
    /// <param name="currentDisplays">Current system displays</param>
    /// <returns>A boolean value indicating the success of the operation. Returns true if the display is set to or already is in the the desired state.</returns>
    /// <exception cref="Win32Exception">Exception is thrown if the WinAPI call fails.</exception>
    private bool SetDisplayAdvancedColorState(uint displayId, bool newState, IDictionary<uint, WindowsDisplayData> currentDisplays)
    {
        if (!currentDisplays.ContainsKey(displayId))
        {
            _logger.DisplayNotFound(displayId);
            return false;
        }
        var displayData = currentDisplays[displayId];
        if (displayData.AdvancedColorInformation == null || !displayData.AdvancedColorInformation.Value.value.HasFlag(DISPLAYCONFIG_ADVANCED_COLOR_INFO_VALUE_FLAGS.AdvancedColorSupported))
        {
            _logger.CannotSetAdvancedColorMode(displayId);
            return false;
        }
        var advancedColorStateEnabled = displayData.AdvancedColorInformation.Value.value.HasFlag(DISPLAYCONFIG_ADVANCED_COLOR_INFO_VALUE_FLAGS.AdvancedColorEnabled);
        if (advancedColorStateEnabled == newState)
        {
            _logger.AdvancedColorStateAlreadySet(displayId);
            return true;
        }

        SetDisplayConfigAdvancedColorState(newState, displayData); // Set the state as everything is valid.
        return true;
    }

    /// <summary>
    /// Set DisplayConfig Advanced Color Info using the Native methods.
    /// </summary>
    /// <param name="newState">New state for Advanced Color Mode</param>
    /// <param name="displayData">DisplayData to set the state for.</param>
    /// <exception cref="Win32Exception">Error occurred in the native query</exception>
    private static void SetDisplayConfigAdvancedColorState(bool newState, WindowsDisplayData displayData)
    {
        var newColorState = new DISPLAYCONFIG_SET_ADVANCED_COLOR_STATE
        {
            // Suppress nullable warning, it should have been validated before.
            header = displayData.AdvancedColorInformation!.Value.header with
            {
                type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_SET_ADVANCED_COLOR_STATE,
                size = Marshal.SizeOf<DISPLAYCONFIG_SET_ADVANCED_COLOR_STATE>()
            },
            value = newState ? DisplayConfigSetAdvancedColorStateValue.Enable : DisplayConfigSetAdvancedColorStateValue.Disable,
        };
        var err = Display.DisplayConfigSetDeviceInfo(ref newColorState);
        if (err != ResultErrorCode.ERROR_SUCCESS)
        {
            throw new Win32Exception((int)err, $"Error occurred while updating AdvancedColorMode for display {displayData.DisplayDevice.DeviceID}.");
        }
    }
}