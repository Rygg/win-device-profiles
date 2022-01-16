using System;
using System.Runtime.InteropServices;
using WinApi.User32.Display.NativeTypes;

namespace WinApi.User32.Display
{
    /// <summary>
    /// Import display methods from user32.dll
    /// </summary>
    public static class NativeMethods
    {
        /// <summary>
        /// The ChangeDisplaySettingsEx function changes the settings of the specified display device to the specified graphics mode.
        /// </summary>
        /// <param name="lpszDeviceName">A pointer to a null-terminated string that specifies the display device whose graphics mode will change. Only display device names as returned by EnumDisplayDevices are valid. 
        /// See EnumDisplayDevices for further information on the names associated with these display devices. <br/> The lpszDeviceName parameter can be NULL. A NULL value specifies the default display device. The default device can be determined by calling EnumDisplayDevices and checking for the DISPLAY_DEVICE_PRIMARY_DEVICE flag.</param>
        /// <param name="lpDevMode">A pointer to a DEVMODE structure that describes the new graphics mode. If lpDevMode is NULL, all the values currently in the registry will be used for the display setting. Passing NULL for the lpDevMode parameter and 0 for the dwFlags parameter is the easiest way to return to the default mode after a dynamic mode change.</param>
        /// <param name="hwnd">Reserved; must be NULL.</param>
        /// <param name="dwflags">Indicates how the graphics mode should be changed. This parameter can be one of the following values.</param>
        /// <param name="lParam">If dwFlags is CDS_VIDEOPARAMETERS, lParam is a pointer to a VIDEOPARAMETERS structure. Otherwise lParam must be NULL.</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern DISP_CHANGE ChangeDisplaySettingsEx(string lpszDeviceName, ref DEVMODE lpDevMode, IntPtr hwnd, ChangeDisplaySettingsFlags dwflags, IntPtr lParam);

        /// <summary>
        /// The ChangeDisplaySettingsEx function changes the settings of the specified display device to the specified graphics mode.
        /// Overload to support DEVMODE as a pointer.
        /// </summary>
        /// <param name="lpszDeviceName">A pointer to a null-terminated string that specifies the display device whose graphics mode will change. Only display device names as returned by EnumDisplayDevices are valid. 
        /// See EnumDisplayDevices for further information on the names associated with these display devices. <br/> The lpszDeviceName parameter can be NULL. A NULL value specifies the default display device. The default device can be determined by calling EnumDisplayDevices and checking for the DISPLAY_DEVICE_PRIMARY_DEVICE flag.</param>
        /// <param name="lpDevMode">A pointer to a DEVMODE structure that describes the new graphics mode. If lpDevMode is NULL, all the values currently in the registry will be used for the display setting. Passing NULL for the lpDevMode parameter and 0 for the dwFlags parameter is the easiest way to return to the default mode after a dynamic mode change.</param>
        /// <param name="hwnd">Reserved; must be NULL.</param>
        /// <param name="dwflags">Indicates how the graphics mode should be changed. This parameter can be one of the following values.</param>
        /// <param name="lParam">If dwFlags is CDS_VIDEOPARAMETERS, lParam is a pointer to a VIDEOPARAMETERS structure. Otherwise lParam must be NULL.</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern DISP_CHANGE ChangeDisplaySettingsEx(string lpszDeviceName, IntPtr lpDevMode, IntPtr hwnd, ChangeDisplaySettingsFlags dwflags, IntPtr lParam);

        /// <summary>
        /// The EnumDisplayDevices function lets you obtain information about the display devices in the current session.
        /// </summary>
        /// <param name="lpDevice">A pointer to the device name. If NULL, function returns information for the display adapter(s) on the machine, based on iDevNum.</param>
        /// <param name="iDevNum">An index value that specifies the display device of interest. The operating system identifies each display device in the current session with an index value.The index values are consecutive integers, starting at 0. If the current session has three display devices, for example, they are specified by the index values 0, 1, and 2.</param>
        /// <param name="lpDisplayDevice">A pointer to a DISPLAY_DEVICE structure that receives information about the display device specified by iDevNum. Before calling EnumDisplayDevices, you must initialize the cb member of DISPLAY_DEVICE to the size, in bytes, of DISPLAY_DEVICE.</param>
        /// <param name="dwFlags">Set this flag to EDD_GET_DEVICE_INTERFACE_NAME (0x00000001) to retrieve the device interface name for GUID_DEVINTERFACE_MONITOR, which is registered by the operating system on a per monitor basis. The value is placed in the DeviceID member of the DISPLAY_DEVICE structure returned in lpDisplayDevice. 
        /// The resulting device interface name can be used with SetupAPI functions and serves as a link between GDI monitor devices and SetupAPI monitor devices.</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);
        /// <summary>
        /// The EnumDisplaySettings function retrieves information about one of the graphics modes for a display device. 
        /// To retrieve information for all the graphics modes of a display device, make a series of calls to this function.
        /// </summary>
        /// <param name="deviceName">A pointer to a null-terminated string that specifies the display device about whose graphics mode the function will obtain information. This parameter is either NULL or a DISPLAY_DEVICE.DeviceName returned from EnumDisplayDevices.A NULL value specifies the current display device on the computer on which the calling thread is running.</param>
        /// <param name="modeNum">Graphics mode indexes start at zero. To obtain information for all of a display device's graphics modes, make a series of calls to EnumDisplaySettings, as follows: Set iModeNum to zero for the first call, and increment iModeNum by one for each subsequent call. Continue calling the function until the return value is zero.
        /// <param name="devMode">A pointer to a DEVMODE structure into which the function stores information about the specified graphics mode. Before calling EnumDisplaySettings, set the dmSize member to sizeof(DEVMODE), and set the dmDriverExtra member to indicate the size, in bytes, of the additional space available to receive private driver data.</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);
    }
}
