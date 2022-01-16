﻿using DisplayController.App.Control.Types;
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
                Log.Trace("Device.ToString(): " + device);
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
                _displays[path.sourceInfo.id].MonitorFriendlyName = targetInfo.monitorFriendlyDeviceName; // Set to the private variable.

                // Get advanced color info (HDR).
                var colorInfo = new DISPLAYCONFIG_ADVANCED_COLOR_INFO();
                colorInfo.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_ADVANCED_COLOR_INFO;
                colorInfo.header.size = Marshal.SizeOf<DISPLAYCONFIG_ADVANCED_COLOR_INFO>();
                colorInfo.header.adapterId = path.targetInfo.adapterId;
                colorInfo.header.id = path.targetInfo.id;
                err = NativeMethods.DisplayConfigGetDeviceInfo(ref colorInfo);
                if (err != ResultErrorCode.ERROR_SUCCESS)
                {
                    throw new Win32Exception((int)err);
                }
                Log.Trace("Color encoding: " + colorInfo.colorEncoding);
                Log.Trace("Bits: " + colorInfo.bitsPerColorChannel);
                Log.Trace("Advanced Color Info Flags: " + colorInfo.stateFlags);

                _displays[path.sourceInfo.id].AdvancedColorInformation = colorInfo; // Save to memory.
            }
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
            Log.Debug("Applying settings");
            // Apply settings:
            NativeMethods.ChangeDisplaySettingsEx(null, IntPtr.Zero, (IntPtr)null, ChangeDisplaySettingsFlags.CDS_NONE, (IntPtr)null);
            Log.Info("Applied");
            // For future long term service functionality:
            Log.Debug("Updating displays.");
            RefreshDisplayDevices(); // Update current display devices.
        }
    }
}
