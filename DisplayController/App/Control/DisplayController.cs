using DisplayController.App.Control.Types;
using NLog;
using System;
using System.Collections.Generic;
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
                Log.Debug("DisplayId: " + displayId);
                Log.Debug("Device.ToString(): " + device);
                Log.Debug("DeviceName: " + device.DeviceName);
                Log.Debug("DeviceID: " + device.DeviceID);
                Log.Debug("DeviceString: " + device.DeviceString);
                Log.Debug("DeviceKey: " + device.DeviceKey);
                Log.Debug("StateFlags: " + device.StateFlags);

                if (!device.StateFlags.HasFlag(DisplayDeviceStateFlags.AttachedToDesktop))
                {
                    displayId++;
                    Log.Debug("Device is not attached to the desktop. Ignoring");
                    continue;
                }
                Log.Trace("Retrieving DeviceMode.");
                var deviceMode = new DEVMODE();
                NativeMethods.EnumDisplaySettings(device.DeviceName, -1, ref deviceMode);
                Log.Debug($"DeviceMode: {deviceMode.dmPelsWidth}x{deviceMode.dmPelsHeight}@{deviceMode.dmDisplayFrequency}Hz");
                _displays.Add(displayId, new DisplayData(device, deviceMode));

                displayId++;
            }
            Log.Info("DisplayDevices refreshed.");
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
            if (newPrimary.Device.StateFlags.HasFlag(DisplayDeviceStateFlags.PrimaryDevice))
            {
                Log.Info("Display is already primary display. Returning");
                return;
            }

            var offsetX = newPrimary.DeviceMode.dmPosition.x; // Get old position.
            int offsetY = newPrimary.DeviceMode.dmPosition.y;
            newPrimary.DeviceMode.dmPosition.x = 0; // Set new as 0,0 (Primary is always 0,0)
            newPrimary.DeviceMode.dmPosition.y = 0;

            // Change values to registry. Don't take effect yet.
            NativeMethods.ChangeDisplaySettingsEx(newPrimary.Device.DeviceName, ref newPrimary.DeviceMode, (IntPtr)null, (ChangeDisplaySettingsFlags.CDS_SET_PRIMARY | ChangeDisplaySettingsFlags.CDS_UPDATEREGISTRY | ChangeDisplaySettingsFlags.CDS_NORESET), IntPtr.Zero);
            Log.Debug($"Updated registry settings for monitor {displayId}");
            // Update the offsets of the rest of the displays:
            var otherDisplays = _displays.Where(d => d.Key != displayId);

            foreach (var display in otherDisplays)
            {
                display.Value.DeviceMode.dmPosition.x -= offsetX; // Substract old primary display offset to get correct new screen position.
                display.Value.DeviceMode.dmPosition.y -= offsetY;
                // Chaange values to registry, don't apply changes yet.
                NativeMethods.ChangeDisplaySettingsEx(display.Value.Device.DeviceName, ref display.Value.DeviceMode, (IntPtr)null, (ChangeDisplaySettingsFlags.CDS_UPDATEREGISTRY | ChangeDisplaySettingsFlags.CDS_NORESET), IntPtr.Zero);
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
