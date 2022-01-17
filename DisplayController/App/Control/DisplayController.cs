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
        private readonly Dictionary<uint, DisplayData> _displays = new Dictionary<uint, DisplayData>();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DisplayController()
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
            foreach(var display in _displays)
            {
                Log.Debug($"DisplayId: {display.Key}, Data: {Environment.NewLine + display.Value + Environment.NewLine}");
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
        public string GetRetrievedDisplayInformationString()
        {
            var str = string.Empty;
            foreach(var display in _displays)
            {
                str += $"DisplayId {display.Key}: {Environment.NewLine}{display.Value}" + Environment.NewLine + Environment.NewLine;
            }
            return str;
        }

        /// <summary>
        /// Method sets the parameter as the new primary display.
        /// </summary>
        /// <param name="displayId">Id of the new primary display.</param>
        public void SetPrimaryDisplay(uint displayId)
        {
            RefreshDisplayDevices(); // Refresh display devices for accurate data.
            Log.Info($"Setting Display {displayId} as the primary monitor.");
            if (!_displays.ContainsKey(displayId))
            {
                Log.Warn("DisplayId not found. Returning.");
                return;
            }

            var newPrimary = _displays[displayId];
            if (newPrimary.DisplayDevice.StateFlags.HasFlag(DisplayDeviceStateFlags.PrimaryDevice))
            {
                Log.Info("Display is already primary display. Returning");
                return;
            }

            var offsetX = newPrimary.DeviceMode.dmPosition.x; // Get old position.
            int offsetY = newPrimary.DeviceMode.dmPosition.y;
            newPrimary.DeviceMode.dmPosition.x = 0; // Set new as 0,0 (Primary is always 0,0)
            newPrimary.DeviceMode.dmPosition.y = 0;

            // Change values to registry. Don't take effect yet.
            NativeMethods.ChangeDisplaySettingsEx(newPrimary.DisplayDevice.DeviceName, ref newPrimary.DeviceMode, (IntPtr)null, (ChangeDisplaySettingsFlags.CDS_SET_PRIMARY | ChangeDisplaySettingsFlags.CDS_UPDATEREGISTRY | ChangeDisplaySettingsFlags.CDS_NORESET), IntPtr.Zero);
            Log.Debug($"Updated registry settings for monitor {displayId}");
            // Update the offsets of the rest of the displays:
            var otherDisplays = _displays.Where(d => d.Key != displayId);

            foreach (var display in otherDisplays)
            {
                display.Value.DeviceMode.dmPosition.x -= offsetX; // Substract old primary display offset to get correct new screen position.
                display.Value.DeviceMode.dmPosition.y -= offsetY;
                // Chaange values to registry, don't apply changes yet.
                NativeMethods.ChangeDisplaySettingsEx(display.Value.DisplayDevice.DeviceName, ref display.Value.DeviceMode, (IntPtr)null, (ChangeDisplaySettingsFlags.CDS_UPDATEREGISTRY | ChangeDisplaySettingsFlags.CDS_NORESET), IntPtr.Zero);
                Log.Debug($"Updated registry settings for monitor {display.Key}");
            }

            ApplyDisplayDeviceSettings(); // Apply device settings.

            RefreshDisplayDevices(); // Update current display devices.
        }

        /// <summary>
        /// Method sets the parameter as the new primary display.
        /// </summary>
        /// <param name="displayId">Id of the new primary display.</param>
        public void SetDisplayRefreshRate(uint displayId, int newRefreshRate)
        {
            RefreshDisplayDevices(); // Refresh display devices for accurate data.
            Log.Info($"Updating display {displayId} refresh rate to {newRefreshRate}.");
            if (!_displays.ContainsKey(displayId))
            {
                Log.Warn("DisplayId not found. Returning.");
                return;
            }
            var display = _displays[displayId];
            var oldRefreshRate = display.DeviceMode.dmDisplayFrequency;
            if (oldRefreshRate == newRefreshRate)
            {
                Log.Info("Display refresh rate is already set to the desired value. Returning.");
                return;
            }
            Log.Debug("Testing monitor support for new refresh rate.");
            display.DeviceMode.dmDisplayFrequency = newRefreshRate;
            // Test can the refresh mode be set for this display:
            var testResult = NativeMethods.ChangeDisplaySettingsEx(display.DisplayDevice.DeviceName, ref display.DeviceMode, (IntPtr)null, ChangeDisplaySettingsFlags.CDS_TEST, IntPtr.Zero);
            if(testResult != DISP_CHANGE.Successful)
            {
                Log.Error("Selected refresh rate is not supported for this monitor.");
                display.DeviceMode.dmDisplayFrequency = oldRefreshRate;
                return;
            }
            Log.Info("New refresh rate is supported by the monitor.");
            // Valid refresh rate. Update to registry.
            var result = NativeMethods.ChangeDisplaySettingsEx(display.DisplayDevice.DeviceName, ref display.DeviceMode, (IntPtr)null, (ChangeDisplaySettingsFlags.CDS_UPDATEREGISTRY | ChangeDisplaySettingsFlags.CDS_NORESET), IntPtr.Zero);
            if(result != DISP_CHANGE.Successful)
            {
                Log.Error("Updating refresh rate failed.");
                throw new Win32Exception("Updating monitor refresh rate failed.");
            }
            Log.Debug($"Updated registry settings for monitor {displayId}");
            
            ApplyDisplayDeviceSettings(); // Apply device settings.

            RefreshDisplayDevices(); // Update current display devices.
        }

        private void ApplyDisplayDeviceSettings()
        {
            Log.Debug("Applying settings");
            // Apply settings:
            NativeMethods.ChangeDisplaySettingsEx(null, IntPtr.Zero, (IntPtr)null, ChangeDisplaySettingsFlags.CDS_NONE, (IntPtr)null);
            Log.Info("Applied");
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
            RefreshDisplayDevices(); // Refresh display devices for accurate data.
            Log.Info($"Setting advanced color state for display {displayId}. New state: {newState}");
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
            if (advancedColorStateEnabled && newState)
            {
                Log.Info("Advanced color state is already enabled.");
                return true;
            }
            if(!advancedColorStateEnabled && !newState)
            {
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
            Log.Debug("DeviceInfo set.");
            Log.Debug("Updating displays to validate the result.");
            RefreshDisplayDevices(); // Refresh the display devices.
            var advancedColorEnabled = _displays[displayId].AdvancedColorInformation.Value.value.HasFlag(DISPLAYCONFIG_ADVANCED_COLOR_INFO_VALUE_FLAGS.AdvancedColorEnabled);
            if (newState == advancedColorEnabled)
            {
                Log.Info("Advanced color state changed.");
                return true;
            }
            else
            {
                Log.Error("Changing advanced color state failed.");
                return false;
            }
        }
    }
}
