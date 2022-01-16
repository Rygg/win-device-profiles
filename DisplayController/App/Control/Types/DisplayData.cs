using System;
using WinApi.User32.Display.NativeTypes;

namespace DisplayController.App.Control.Types
{
    /// <summary>
    /// Class for containing display data.
    /// </summary>
    internal class DisplayData
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
        public DISPLAYCONFIG_ADVANCED_COLOR_INFO AdvancedColorInformation;
        /// <summary>
        /// The friendly name of the target display device.
        /// </summary>
        public string MonitorFriendlyName;

        public DisplayData(DISPLAY_DEVICE device, DEVMODE deviceMode)
        {
            DisplayDevice = device;
            DeviceMode = deviceMode;
        }

        // For logging purposes.
        public override string ToString()
        {
            return
                $"DeviceName: {DisplayDevice.DeviceName}:" + Environment.NewLine +
                $"Target Monitor Name: {MonitorFriendlyName}" + Environment.NewLine +
                $"Device Mode: {DeviceMode.dmPelsWidth}x{DeviceMode.dmPelsHeight}@{DeviceMode.dmDisplayFrequency}Hz" + Environment.NewLine +
                $"Source Device: {DisplayDevice.DeviceString}" + Environment.NewLine +
                $"Device State Flags: {DisplayDevice.StateFlags}" + Environment.NewLine +
                $"Device Registry Key: {DisplayDevice.DeviceKey}" + Environment.NewLine +
                $"Device Color Encoding: {AdvancedColorInformation.colorEncoding}, {AdvancedColorInformation.bitsPerColorChannel} bits per channel" + Environment.NewLine +
                $"Advanced Color Information Flags: {AdvancedColorInformation.stateFlags}";
        }
    }
}
