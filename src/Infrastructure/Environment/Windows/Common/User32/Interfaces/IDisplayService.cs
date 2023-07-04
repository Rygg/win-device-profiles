using System.ComponentModel;
using DeviceProfiles.Infrastructure.Environment.Windows.Common.User32.NativeTypes.Structs;

namespace DeviceProfiles.Infrastructure.Environment.Windows.Common.User32.Interfaces;

internal interface IDisplayService
{
    /// <summary>
    /// Method uses the <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-changedisplaysettingsexa">ChangeDisplaySettingsEx</see> function
    /// to change the selected display and device mode as the new primary display. <br/>
    /// <remarks>
    /// Does not apply the changes.
    /// </remarks> 
    /// </summary>
    /// <param name="displayDevice">DisplayDevice parameter</param>
    /// <param name="deviceMode">DeviceMode parameter.</param>
    public void SetStandardDeviceAsPrimaryDisplay(DISPLAY_DEVICE displayDevice, ref DEVMODE deviceMode);

    /// <summary>
    /// Method uses the <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-changedisplaysettingsexa">ChangeDisplaySettingsEx</see> function
    /// to change the selected display and device mode to the registry. <br/>
    /// </summary>
    /// <remarks>
    /// Does not apply the changes.
    /// </remarks>
    /// <param name="displayDevice">DisplayDevice parameter</param>
    /// <param name="deviceMode">DeviceMode parameter.</param>
    public void SetStandardDeviceDeviceMode(DISPLAY_DEVICE displayDevice, ref DEVMODE deviceMode);

    /// <summary>
    /// Method uses the <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-changedisplaysettingsexa">ChangeDisplaySettingsEx</see> function
    /// to change the selected display and device mode to the new refresh mode. <br/>
    /// </summary>
    /// <remarks>
    /// Does not apply the changes.
    /// </remarks>
    /// <param name="displayDevice">DisplayDevice parameter</param>
    /// <param name="deviceMode">DeviceMode parameter.</param>
    /// <param name="newRefreshRate">New refresh rate to set.</param>
    /// <returns>True if the operation was successful. False is already set.</returns>
    /// <exception cref="Win32Exception">Something went wrong with the API calls.</exception>
    /// <exception cref="InvalidOperationException">Selected RefreshRate is not supported.</exception>
    public bool SetStandardDeviceRefreshRate(DISPLAY_DEVICE displayDevice, ref DEVMODE deviceMode, int newRefreshRate);

    /// <summary>
    /// Method applies the stored changes for the standard device methods.
    /// </summary>
    public void ApplyStandardDeviceChanges();

    /// <summary>
    /// Uses the <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-enumdisplaydevicesa">EnumDisplaySettings</see> function to obtain information about the display devices.
    /// </summary>
    /// <param name="displayId">Display identifier</param>
    /// <returns>Display information for the given identifer, or null if no device is found for the specified id.</returns>
    public DISPLAY_DEVICE? GetDisplayDevice(uint displayId);

    /// <summary>
    /// Uses the <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-enumdisplaysettingsa">EnumDisplaySettings</see> function to retrieve information about one of the graphics modes for a display device. 
    /// </summary>
    /// <param name="device">DisplayDevice to query the information on.</param>
    /// <returns>Returned information for this device.</returns>
    /// <exception cref="Win32Exception">Something went wrong with the API call.</exception>
    public DEVMODE GetDisplayDeviceMode(DISPLAY_DEVICE device);

    /// <summary>
    /// Uses <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getdisplayconfigbuffersizes">GetDisplayConfigBufferSizes</see> and
    /// <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-querydisplayconfig">QueryDisplayConfig</see> functions
    /// to retrieve information about all possible display paths for all display devices.
    /// </summary>
    /// <returns>A collection of display configuration path information.</returns>
    /// <exception cref="Win32Exception">Something went wrong with the API calls.</exception>
    public IEnumerable<DISPLAYCONFIG_PATH_INFO> GetDisplayConfigPathInformation();

    /// <summary>
    /// Uses the <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-displayconfiggetdeviceinfo">DisplayConfigGetDeviceInfo</see> function retrieves display configuration information about the device.
    /// </summary>
    /// <param name="path">Path information to query</param>
    /// <returns>Returns the WinAPI structure containing advanced color information.</returns>
    /// <exception cref="Win32Exception">Something went wrong with the API call.</exception>
    public DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO GetDisplayConfigurationAdvancedColorInformation(DISPLAYCONFIG_PATH_INFO path);

    /// <summary>
    /// Uses the <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-displayconfiggetdeviceinfo">DisplayConfigGetDeviceInfo</see> function retrieves display configuration information about the device.
    /// </summary>
    /// <param name="path">Path information to query</param>
    /// <returns>Returns the WinAPI structure containing advanced source device information.</returns>
    /// <exception cref="Win32Exception">Something went wrong with the API call.</exception>
    public DISPLAYCONFIG_SOURCE_DEVICE_NAME GetDisplayConfigurationSourceDeviceInformation(DISPLAYCONFIG_PATH_INFO path);

    /// <summary>
    /// Uses the <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-displayconfiggetdeviceinfo">DisplayConfigGetDeviceInfo</see> function retrieves display configuration information about the device.
    /// </summary>
    /// <param name="path">Path information to query</param>
    /// <returns>Returns the WinAPI structure containing advanced target device information.</returns>
    /// <exception cref="Win32Exception">Something went wrong with the API call.</exception>
    public DISPLAYCONFIG_TARGET_DEVICE_NAME GetDisplayConfigurationTargetDeviceInformation(DISPLAYCONFIG_PATH_INFO path);

    /// <summary>
    /// Uses the <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-displayconfigsetdeviceinfo">DisplayConfigSetDeviceInfo</see> function sets the properties of a target.
    /// </summary>
    /// <param name="deviceInfoHeader"></param>
    /// <param name="newState">New state for the advanced color mode. True for enabled, False for disabled.</param>
    public void SetDisplayConfigurationAdvancedColorInformation(DISPLAYCONFIG_DEVICE_INFO_HEADER deviceInfoHeader, bool newState);
}