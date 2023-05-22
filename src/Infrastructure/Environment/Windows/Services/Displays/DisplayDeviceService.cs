using System.ComponentModel;
using Application.Common.Interfaces;
using Domain.Models;
using Infrastructure.Environment.Windows.Common.User32.Interfaces;
using Infrastructure.Environment.Windows.Common.User32.NativeTypes.Enums;
using Infrastructure.Environment.Windows.Services.Displays.Models;
using Infrastructure.Extensions;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Environment.Windows.Services.Displays;

/// <summary>
/// Service for controlling all things related to Windows environment Display controlling.
/// </summary>
internal sealed class DisplayDeviceService : IDisplayDeviceController
{
    private readonly ILogger<DisplayDeviceService> _logger;
    private readonly IDisplayService _displayService;

    public DisplayDeviceService(ILogger<DisplayDeviceService> logger, IDisplayService displayService)
    {
        _logger = logger;
        _displayService = displayService;
    }

    /// <summary>
    /// Changes the current system display settings to the given DeviceProfile.
    /// </summary>
    /// <param name="profile">DeviceProfile to be set as active.</param>
    /// <param name="cancellationToken">CancellationToken to cancel the operation.</param>
    /// <returns>True if the operation was successful. False if no changes were made.</returns>
    public Task<bool> ChangeDisplaySettings(DeviceProfile profile, CancellationToken cancellationToken)
    {
        if (profile == null)
        {
            throw new ArgumentNullException(nameof(profile));
        }

        var displays = RetrieveSystemDisplayInformation(cancellationToken);
        var displaysForStandardSettings = profile.DisplaySettings.Where(ds => ds.PrimaryDisplay != null || ds.RefreshRate != null && ds.RefreshRate != 0);
        var displaysForAdvancedColorState = profile.DisplaySettings.Where(ds => ds.EnableHdr != null);

        var anyChanges = SetStandardDisplaySettings(displaysForStandardSettings, displays, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromResult(false);
        }
        var anyHdrChanges = SetAdvancedColorDisplaySettings(displaysForAdvancedColorState, displays);
        return Task.FromResult(anyChanges || anyHdrChanges);
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
            var device = _displayService.GetDisplayDevice(displayId);
            if (device == null) // Loop uints until no display device is found. (No display device at this id).
            {
                break; // Break out.
            }

            _logger.DisplayRetrieved(displayId, device.Value.ToString());

            if (!device.Value.StateFlags.HasFlag(DisplayDeviceStateFlags.AttachedToDesktop))
            {
                displayId++;
                _logger.DeviceNotAttachedToDesktop();
                continue;
            }
            _logger.RetrievingDeviceModes(displayId);
            var deviceMode = _displayService.GetDisplayDeviceMode(device.Value);
            
            _logger.DeviceModeRetrieved(displayId, deviceMode.ToString());
            displays.Add(displayId, new WindowsDisplayData
            {
                DisplayDevice = device.Value,
                deviceMode = deviceMode
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
        var paths = _displayService.GetDisplayConfigPathInformation();
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
            var sourceInfo = _displayService.GetDisplayConfigurationSourceDeviceInformation(path);

            // Check if the device names match.
            if (displays[path.sourceInfo.id].DisplayDevice.DeviceName != sourceInfo.viewGdiDeviceName)
            {
                throw new InvalidOperationException($"DeviceNames did not match for DisplayId: {path.sourceInfo.id}");
            }
            // Get the target device name.
            var targetInfo = _displayService.GetDisplayConfigurationTargetDeviceInformation(path);
            displays[path.sourceInfo.id].SetTargetInfo(targetInfo); // Save the device name to memory.

            // Get advanced color info (HDR).
            var colorInfo = _displayService.GetDisplayConfigurationAdvancedColorInformation(path);
            displays[path.sourceInfo.id].SetAdvancedColorInformation(colorInfo); // Save to memory.
        }
    }


    /// <summary>
    /// Method updating Display Settings using the Standard Win API.
    /// </summary>
    /// <param name="displaySettings">Array of display settings to update.</param>
    /// <param name="currentDisplays">Current displays in the system.</param>
    /// <param name="cancellationToken">CancellationToken to cancel the operation.</param>
    private bool SetStandardDisplaySettings(IEnumerable<DisplaySettings> displaySettings, IDictionary<uint, WindowsDisplayData> currentDisplays, CancellationToken cancellationToken)
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
            _displayService.ApplyStandardDeviceChanges();
            return true;
        }
        return false;
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

        // Store the old position to fix offsets of other monitors.
        var offsetX = newPrimary.deviceMode.dmPosition.x; 
        var offsetY = newPrimary.deviceMode.dmPosition.y;
        _displayService.SetStandardDeviceAsPrimaryDisplay(newPrimary.DisplayDevice, ref newPrimary.deviceMode);
        _logger.UpdatedRegistrySettings(displayId);
        // Update the offsets of the rest of the displays:
        var otherDisplays = currentDisplays.Where(d => d.Key != displayId);

        foreach (var (id, display) in otherDisplays)
        {
            display.deviceMode.dmPosition.x -= offsetX; // Subtract old primary display offset to get correct new screen position.
            display.deviceMode.dmPosition.y -= offsetY;
            _displayService.SetStandardDeviceDeviceMode(display.DisplayDevice, ref display.deviceMode);
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
        try
        {
            var result = _displayService.SetStandardDeviceRefreshRate(display.DisplayDevice, ref display.deviceMode, newRefreshRate);
            if (result == false)
            {
                return false;
            }
            _logger.UpdatedRegistrySettings(displayId);
            return true;
        }
        catch (InvalidOperationException)
        {
            _logger.RefreshRateNotSupported(displayId, newRefreshRate);
            return false;
        }
    }

    /// <summary>
    /// Method updating Advanced Display Settings using the advanced Win API.
    /// </summary>
    /// <param name="displaySettings">Array of display settings to update.</param>
    /// <param name="currentDisplays">Current displays in the system</param>
    private bool SetAdvancedColorDisplaySettings(IEnumerable<DisplaySettings> displaySettings, IDictionary<uint, WindowsDisplayData> currentDisplays)
    {
        var anyChange = false;
        foreach (var display in displaySettings) // Update advanced color state values for required displays.
        {
            _logger.UpdatingAdvancedColorState(display.DisplayId, display.EnableHdr!.Value); // Suppress nullable, should have been validated before.
            if (SetDisplayAdvancedColorState(display.DisplayId, display.EnableHdr!.Value, currentDisplays)) // Suppress nullable, should have been validated before.
            {
                _logger.AdvancedColorStateUpdated(display.DisplayId);
                anyChange = true;
            }
        }

        return anyChange;
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
        _displayService.SetDisplayConfigurationAdvancedColorInformation(displayData.AdvancedColorInformation.Value.header, newState);
        return true;
    }
}