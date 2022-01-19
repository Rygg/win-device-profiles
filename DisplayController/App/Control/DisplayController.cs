using DisplayController.App.Configuration;
using DisplayController.App.Control.Types;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using WinApi.User32.Display;
using WinApi.User32.Display.NativeTypes;

namespace DisplayController.App.Control
{
    /// <summary>
    /// Class containing the functionality for controlling the displays of the system.
    /// </summary>
    internal sealed class DisplayController
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Variable containing the current displays of the system.
        /// </summary>
        private readonly Dictionary<uint, DisplayData> _displays = new();

        /// <summary>
        /// Default constructor.
        /// </summary>
        internal DisplayController()
        {
            RefreshDisplayDevices(); // Get initial display devices.
        }

        /// <summary>
        /// Method refreshes available display devices from WinApi and stores them to memory.
        /// </summary>
        private void RefreshDisplayDevices()
        {
            if (_displays.Any())
            {
                Log.Debug("Clearing old display devices.");
                _displays.Clear(); // Clear old displays.
            }
            Log.Debug("Retrieving display devices..");
            uint displayId = 0;
            while (true) // TODO: Some other condition?
            {
                var device = new DISPLAY_DEVICE();
                device.cb = Marshal.SizeOf(device);
                if (!NativeMethods.EnumDisplayDevices(null, displayId, ref device, 0)) // Loop uints until EnumDisplayDevices returns false. (No display device at this id).
                {
                    break; // Break out.
                }
                Log.Trace("DisplayId: " + displayId);
                Log.Trace("DeviceName: " + device.DeviceName);
                Log.Trace("DeviceID: " + device.DeviceID);
                Log.Trace("DeviceString: " + device.DeviceString);
                Log.Trace("DeviceKey: " + device.DeviceKey);
                Log.Trace("StateFlags: " + device.StateFlags);

                if (!device.StateFlags.HasFlag(DisplayDeviceStateFlags.AttachedToDesktop))
                {
                    displayId++;
                    Log.Debug("Device is not attached to the desktop. Ignoring");
                    continue;
                }
                Log.Debug("Retrieving DeviceMode.");
                var deviceMode = new DEVMODE();
                if(!NativeMethods.EnumDisplaySettings(device.DeviceName, -1, ref deviceMode))
                {
                    throw new Win32Exception("DeviceMode not located."); // Every device should have a device mode.
                }
                Log.Trace($"dmPelsWidth: {deviceMode.dmPelsWidth}");
                Log.Trace($"dmPelsHeight: {deviceMode.dmPelsHeight}");
                Log.Trace($"dmDisplayFrequency: {deviceMode.dmDisplayFrequency}");
                Log.Trace($"dmPosition.x: {deviceMode.dmPosition.x}");
                Log.Trace($"dmPosition.y: {deviceMode.dmPosition.y}");
                _displays.Add(displayId, new DisplayData(device, deviceMode));

                displayId++;
            }
            UpdateDisplayConfigAdvancedInformation(); // Update advanced additional information to the retrieved displays.
            // Log data for debugging.
            Log.Debug("Retrieved displays:");
            foreach(var (id, display) in _displays)
            {
                Log.Debug($"DisplayId: {id}, Data: {Environment.NewLine + display + Environment.NewLine}");
            }
            Log.Info("Refreshed displays.");
        }

        /// <summary>
        /// Method uses additional CCD API to retrieve additional information for the retrieved display devices.
        /// Method must be called after the _displays dictionary has been initialized.
        /// </summary>
        /// <exception cref="Win32Exception"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        private void UpdateDisplayConfigAdvancedInformation()
        {
            Log.Debug("Retrieving additional display information.");
            if(!_displays.Any())
            {
                throw new InvalidOperationException("No displays retrieved. Invalid operation.");
            }
            // Get buffer sizes for the active displays.
            var err = NativeMethods.GetDisplayConfigBufferSizes(QDC.QDC_ONLY_ACTIVE_PATHS, out var pathCount, out var modeCount);
            if (err != ResultErrorCode.ERROR_SUCCESS)
            {
                throw new Win32Exception((int)err);
            }

            var paths = new DISPLAYCONFIG_PATH_INFO[pathCount]; // Create arrays to hold the info.
            var modes = new DISPLAYCONFIG_MODE_INFO[modeCount]; 
            // Get display configs from CCD API:
            err = NativeMethods.QueryDisplayConfig(QDC.QDC_ONLY_ACTIVE_PATHS, ref pathCount, paths, ref modeCount, modes, IntPtr.Zero);
            if (err != ResultErrorCode.ERROR_SUCCESS)
            {
                throw new Win32Exception((int)err);
            }

            // Loop through the active display paths:
            foreach (var path in paths)
            {
                // Check if the display exists.
                if(!_displays.ContainsKey(path.sourceInfo.id))
                {
                    Log.Warn("Retrieved device source was not located in DisplayData. Skipping the device.");
                    continue;
                }

                // Get the source information. This is to further check mapping so that each device data is mapped correctly.
                var sourceInfo = new DISPLAYCONFIG_SOURCE_DEVICE_NAME();
                sourceInfo.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_SOURCE_NAME;
                sourceInfo.header.size = Marshal.SizeOf<DISPLAYCONFIG_SOURCE_DEVICE_NAME>();
                sourceInfo.header.adapterId = path.sourceInfo.adapterId;
                sourceInfo.header.id = path.sourceInfo.id;
                err = NativeMethods.DisplayConfigGetDeviceInfo(ref sourceInfo); // Call for info.
                if (err != ResultErrorCode.ERROR_SUCCESS)
                {
                    throw new Win32Exception((int)err);
                }

                // Check if the device names match.
                if(_displays[path.sourceInfo.id].DisplayDevice.DeviceName != sourceInfo.viewGdiDeviceName)
                {
                    Log.Error("DeviceNames did not match.");
                    throw new InvalidOperationException("DeviceNames did not match");
                }

                // Get the target device name.
                var targetInfo = new DISPLAYCONFIG_TARGET_DEVICE_NAME();
                targetInfo.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME;
                targetInfo.header.size = Marshal.SizeOf<DISPLAYCONFIG_TARGET_DEVICE_NAME>();
                targetInfo.header.adapterId = path.targetInfo.adapterId;
                targetInfo.header.id = path.targetInfo.id;
                err = NativeMethods.DisplayConfigGetDeviceInfo(ref targetInfo); // Request target device information:
                if (err != ResultErrorCode.ERROR_SUCCESS)
                {
                    throw new Win32Exception((int)err);
                }

                Log.Trace("Monitor Friendly Name: " + targetInfo.monitorFriendlyDeviceName);
                _displays[path.sourceInfo.id].DisplayTargetInfo = targetInfo; // save to memory.

                // Get advanced color info (HDR).
                var colorInfo = new DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO();
                colorInfo.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_ADVANCED_COLOR_INFO;
                colorInfo.header.size = Marshal.SizeOf<DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO>();
                colorInfo.header.adapterId = path.targetInfo.adapterId;
                colorInfo.header.id = path.targetInfo.id;
                err = NativeMethods.DisplayConfigGetDeviceInfo(ref colorInfo);
                if (err != ResultErrorCode.ERROR_SUCCESS)
                {
                    throw new Win32Exception((int)err);
                }
                Log.Trace("Color encoding: " + colorInfo.colorEncoding);
                Log.Trace("Bits: " + colorInfo.bitsPerColorChannel);
                Log.Trace("Advanced Color Info Flags: " + colorInfo.value);

                _displays[path.sourceInfo.id].AdvancedColorInformation = colorInfo; // Save to memory.
            }
        }

        /// <summary>
        /// Gets retrieved display information as a string.
        /// </summary>
        /// <returns></returns>
        internal string GetRetrievedDisplayInformationString()
        {
            var str = string.Empty;
            foreach(var (id, display) in _displays)
            {
                str += $"DisplayId {id}: {Environment.NewLine}{display}" + Environment.NewLine + Environment.NewLine;
            }
            return str;
        }

        /// <summary>
        /// Method applies the display profile given as a parameter.
        /// </summary>
        /// <param name="displaySettings">DisplaySettings of the profile.</param>
        /// <returns>A task representing the operation.</returns>
        internal void SetDisplayProfile(ProfileDisplaySetting[] displaySettings)
        {
            Log.Trace($"Entering {nameof(SetDisplayProfile)}");

            // Filter displays to the required api methods.
            var displaysForChangeDisplaySettingsEx = displaySettings.Where(ds => ds.PrimaryDisplay != null || ds.RefreshRate != null && ds.RefreshRate != 0);
            var displaysForAdvancedColorState = displaySettings.Where(ds => ds.EnableHdr != null);

            Log.Debug("Refresh DisplayDevices to have the most recent current information.");
            RefreshDisplayDevices();

            var anyRegistryChanges = false;
            var primaryDisplaySet = false;
            foreach(var display in displaysForChangeDisplaySettingsEx) // Set all the required configuration values to the registry.
            {
                if(display.PrimaryDisplay == true)
                {
                    if(!primaryDisplaySet)
                    {
                        Log.Info($"Setting Display {display.DisplayId} as the new primary display.");
                        if(SetPrimaryDisplayToRegistry(display.DisplayId))
                        {
                            Log.Info("Values updated");
                            anyRegistryChanges = true;
                        }                        
                        primaryDisplaySet = true;
                    }
                    else
                    {
                        Log.Warn("Primary Display was already set in this profile. PrimaryDisplay change ignored.");
                    }
                }
                if(display.RefreshRate != null && display.RefreshRate != 0)
                {
                    Log.Info($"Setting Display {display.DisplayId} refresh rate to {display.RefreshRate}Hz");
                    if(SetDisplayRefreshRateToRegistry(display.DisplayId, display.RefreshRate.Value))
                    {
                        anyRegistryChanges = true;
                        Log.Info("Values updated");
                    }
                }
            }
            if(anyRegistryChanges) // No need to apply unless changes were made.
            {
                Log.Debug("Applying the changes");
                ApplyDisplayDeviceSettings();
            }
            
            foreach(var display in displaysForAdvancedColorState) // Update advanced color state values for required displays.
            {
                var newStateStr = display.EnableHdr == true ? "Enabled" : "Disabled";
                Log.Info($"Changing advanced color state for Display {display.DisplayId}. New state: {newStateStr}");
                if(SetDisplayAdvancedColorState(display.DisplayId, display.EnableHdr!.Value))
                {
                    Log.Info("Values updated.");
                }
            }

            Log.Info("DisplayProfile changed.");
            RefreshDisplayDevices(); // Refresh the display devices after all the changes.
        }

        /// <summary>
        /// Method sets the parameter as the new primary display to the registry, does not apply changes.
        /// </summary>
        /// <param name="displayId">Id of the new primary display.</param>
        /// <returns>Returns true if any changes were made to the registry.</returns>
        private bool SetPrimaryDisplayToRegistry(uint displayId)
        {
            if (!_displays.ContainsKey(displayId))
            {
                Log.Warn("DisplayId not found. Returning.");
                return false;
            }

            var newPrimary = _displays[displayId];
            if (newPrimary.DisplayDevice.StateFlags.HasFlag(DisplayDeviceStateFlags.PrimaryDevice))
            {
                Log.Info("Display is already primary display. Returning");
                return false;
            }

            var offsetX = newPrimary.DeviceMode.dmPosition.x; // Get old position.
            var offsetY = newPrimary.DeviceMode.dmPosition.y;
            newPrimary.DeviceMode.dmPosition.x = 0; // Set new as 0,0 (Primary is always 0,0)
            newPrimary.DeviceMode.dmPosition.y = 0;

            // Change values to registry. Don't take effect yet.
            NativeMethods.ChangeDisplaySettingsEx(newPrimary.DisplayDevice.DeviceName, ref newPrimary.DeviceMode, (IntPtr)null, (ChangeDisplaySettingsFlags.CDS_SET_PRIMARY | ChangeDisplaySettingsFlags.CDS_UPDATEREGISTRY | ChangeDisplaySettingsFlags.CDS_NORESET), IntPtr.Zero);
            Log.Debug($"Updated registry settings for monitor {displayId}");
            // Update the offsets of the rest of the displays:
            var otherDisplays = _displays.Where(d => d.Key != displayId);

            foreach (var (id, display) in otherDisplays)
            {
                display.DeviceMode.dmPosition.x -= offsetX; // Subtract old primary display offset to get correct new screen position.
                display.DeviceMode.dmPosition.y -= offsetY;
                // Change values to registry, don't apply changes yet.
                NativeMethods.ChangeDisplaySettingsEx(display.DisplayDevice.DeviceName, ref display.DeviceMode, (IntPtr)null, (ChangeDisplaySettingsFlags.CDS_UPDATEREGISTRY | ChangeDisplaySettingsFlags.CDS_NORESET), IntPtr.Zero);
                Log.Debug($"Updated registry settings for monitor {id}");
            }

            return true;
        }

        /// <summary>
        /// Method sets the displays refresh rate to the new value, if it is supported.
        /// </summary>
        /// <param name="displayId">Id of the new primary display.</param>
        /// <param name="newRefreshRate">New refresh rate for the display.</param>
        /// <returns>Returns a boolean indicating whether changes were made to the registry</returns>
        private bool SetDisplayRefreshRateToRegistry(uint displayId, int newRefreshRate)
        {
            if (!_displays.ContainsKey(displayId))
            {
                Log.Warn("DisplayId not found. Returning.");
                return false;
            }
            var display = _displays[displayId];
            var oldRefreshRate = display.DeviceMode.dmDisplayFrequency;
            if (oldRefreshRate == newRefreshRate)
            {
                Log.Info("Display refresh rate is already set to the desired value. Returning.");
                return false;
            }
            Log.Debug("Testing monitor support for new refresh rate.");
            display.DeviceMode.dmDisplayFrequency = newRefreshRate;
            // Test can the refresh mode be set for this display:
            var testResult = NativeMethods.ChangeDisplaySettingsEx(display.DisplayDevice.DeviceName, ref display.DeviceMode, (IntPtr)null, ChangeDisplaySettingsFlags.CDS_TEST, IntPtr.Zero);
            if(testResult != DISP_CHANGE.Successful)
            {
                Log.Error("Selected refresh rate is not supported for this monitor.");
                display.DeviceMode.dmDisplayFrequency = oldRefreshRate;
                return false;
            }
            Log.Debug("New refresh rate is supported by the monitor. Updating.");
            // Valid refresh rate. Update to registry.
            var result = NativeMethods.ChangeDisplaySettingsEx(display.DisplayDevice.DeviceName, ref display.DeviceMode, (IntPtr)null, (ChangeDisplaySettingsFlags.CDS_UPDATEREGISTRY | ChangeDisplaySettingsFlags.CDS_NORESET), IntPtr.Zero);
            if(result != DISP_CHANGE.Successful)
            {
                Log.Error("Updating refresh rate failed.");
                throw new Win32Exception("Updating monitor refresh rate failed.");
            }
            Log.Debug($"Updated registry settings for monitor {displayId}");
            return true;
        }

        /// <summary>
        /// Method applies the values previously updated with <see cref="NativeMethods.ChangeDisplaySettingsEx"/>.
        /// </summary>
        private static void ApplyDisplayDeviceSettings()
        {
            NativeMethods.ChangeDisplaySettingsEx(null, IntPtr.Zero, (IntPtr)null, ChangeDisplaySettingsFlags.CDS_NONE, (IntPtr)null); // Apply settings:
            Log.Info("Changes applied");
        }

        /// <summary>
        /// Method sets the advanced color state of the given monitor to the desired value.
        /// </summary>
        /// <param name="displayId">DisplayId to be operated.</param>
        /// <param name="newState">New state of the advanced color mode.</param>
        /// <returns>A boolean value indicating the success of the operation. Returns true if the display is set to or already is in the the desired state.</returns>
        /// <exception cref="Win32Exception">Exception is thrown if the WinAPI call fails.</exception>
        public bool SetDisplayAdvancedColorState(uint displayId, bool newState)
        {
            if (!_displays.ContainsKey(displayId))
            {
                Log.Error("DisplayId not found. Returning.");
                return false;
            }
            var displayData = _displays[displayId];
            if(displayData.AdvancedColorInformation == null)
            {
                Log.Error("Missing required data. Cannot change advanced color state");
                return false;
            }
            if (!displayData.AdvancedColorInformation.Value.value.HasFlag(DISPLAYCONFIG_ADVANCED_COLOR_INFO_VALUE_FLAGS.AdvancedColorSupported))
            {
                Log.Error("Selected display does not support advanced color modes.");
                return false;
            }
            var advancedColorStateEnabled = displayData.AdvancedColorInformation.Value.value.HasFlag(DISPLAYCONFIG_ADVANCED_COLOR_INFO_VALUE_FLAGS.AdvancedColorEnabled);
            switch (advancedColorStateEnabled)
            {
                case true when newState:
                    Log.Info("Advanced color state is already enabled.");
                    return true;
                case false when !newState:
                    Log.Info("Advanced color state is already disabled.");
                    return true;
            }

            // Create the object for setting the state.
            var newColorState = new DISPLAYCONFIG_SET_ADVANCED_COLOR_STATE
            {
                header = new DISPLAYCONFIG_DEVICE_INFO_HEADER {
                    type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_SET_ADVANCED_COLOR_STATE,
                    adapterId = displayData.AdvancedColorInformation.Value.header.adapterId,
                    id = displayData.AdvancedColorInformation.Value.header.id,
                    size = Marshal.SizeOf<DISPLAYCONFIG_SET_ADVANCED_COLOR_STATE>()
                },
                value = newState ? DisplayConfigSetAdvancedColorStateValue.Enable : DisplayConfigSetAdvancedColorStateValue.Disable,
            };
            Log.Trace("Calling DisplayConfigSetDeviceInfo");
            var err = NativeMethods.DisplayConfigSetDeviceInfo(ref newColorState);
            if (err != ResultErrorCode.ERROR_SUCCESS)
            {
                throw new Win32Exception((int)err);
            }
            Log.Debug("DeviceInfo set with no errors");
            return true;
        }
    }
}
