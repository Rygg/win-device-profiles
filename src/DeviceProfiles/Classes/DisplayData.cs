using System;
using WinApi.User32.Display.NativeTypes;

namespace DeviceProfiles.Classes
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
        public DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO? AdvancedColorInformation;
        /// <summary>
        /// DISPLAYCONFIG_TARGET_DEVICE_NAME from WinAPI CCD API. Also contains target IDs.
        /// </summary>
        public DISPLAYCONFIG_TARGET_DEVICE_NAME? DisplayTargetInfo;
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="deviceMode"></param>
        public DisplayData(DISPLAY_DEVICE device, DEVMODE deviceMode)
        {
            DisplayDevice = device;
            DeviceMode = deviceMode;
        }
        
        /// <summary>
        /// Override to string.
        /// </summary>
        /// <returns>Object in a string format.</returns>
        public override string ToString()
        {
            return
                $"DeviceName: {DisplayDevice.DeviceName}:" + Environment.NewLine +
                $"Target Monitor Name: {DisplayTargetInfo?.monitorFriendlyDeviceName}" + Environment.NewLine +
                $"Device Mode: {DeviceMode.dmPelsWidth}x{DeviceMode.dmPelsHeight}@{DeviceMode.dmDisplayFrequency}Hz" + Environment.NewLine +
                $"Device Position: ({DeviceMode.dmPosition.x},{DeviceMode.dmPosition.y})" + Environment.NewLine + 
                $"Source Device: {DisplayDevice.DeviceString}" + Environment.NewLine +
                $"Device State Flags: {DisplayDevice.StateFlags}" + Environment.NewLine +
                $"Device Registry Key: {DisplayDevice.DeviceKey}" + Environment.NewLine +
                $"Device Color Encoding: {AdvancedColorInformation?.colorEncoding}, {AdvancedColorInformation?.bitsPerColorChannel} bits per channel" + Environment.NewLine +
                $"Advanced Color State: {AdvancedColorInformation?.value}";
        }
    }
}
