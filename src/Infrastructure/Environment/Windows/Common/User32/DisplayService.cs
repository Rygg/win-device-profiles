using System.ComponentModel;
using System.Runtime.InteropServices;
using Infrastructure.Environment.Windows.Common.User32.Interfaces;
using Infrastructure.Environment.Windows.Common.User32.NativeTypes.Enums;
using Infrastructure.Environment.Windows.Common.User32.NativeTypes.Structs;

namespace Infrastructure.Environment.Windows.Common.User32;

/// <summary>
/// Import display methods from user32.dll
/// </summary>
internal sealed class DisplayService : IDisplayService
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
    public void SetStandardDeviceAsPrimaryDisplay(DISPLAY_DEVICE displayDevice, ref DEVMODE deviceMode)
    {
        // Set new primary as 0,0 (Primary is always 0,0)
        deviceMode.dmPosition.x = 0; 
        deviceMode.dmPosition.y = 0;

        // Change values to registry. Don't take effect yet.
        ChangeDisplaySettingsEx(displayDevice.DeviceName, ref deviceMode, (nint)null, (ChangeDisplaySettingsFlags.CDS_SET_PRIMARY | ChangeDisplaySettingsFlags.CDS_UPDATEREGISTRY | ChangeDisplaySettingsFlags.CDS_NORESET), nint.Zero);
    }

    /// <summary>
    /// Method uses the <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-changedisplaysettingsexa">ChangeDisplaySettingsEx</see> function
    /// to change the selected display and device mode to the registry. <br/>
    /// </summary>
    /// <remarks>
    /// Does not apply the changes.
    /// </remarks>
    /// <param name="displayDevice">DisplayDevice parameter</param>
    /// <param name="deviceMode">DeviceMode parameter.</param>
    public void SetStandardDeviceDeviceMode(DISPLAY_DEVICE displayDevice, ref DEVMODE deviceMode)
    {
        // Change values to registry. Don't take effect yet.
        ChangeDisplaySettingsEx(displayDevice.DeviceName, ref deviceMode, (nint)null, (ChangeDisplaySettingsFlags.CDS_UPDATEREGISTRY | ChangeDisplaySettingsFlags.CDS_NORESET), IntPtr.Zero);
    }
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
    /// <returns>True if the operation was successful. False if the refresh rate is not supported.</returns>
    /// <exception cref="Win32Exception">Something went wrong with the API calls.</exception>
    public bool SetStandardDeviceRefreshRate(DISPLAY_DEVICE displayDevice, ref DEVMODE deviceMode, int newRefreshRate)
    {
        var oldRefreshRate = deviceMode.dmDisplayFrequency;
        if (oldRefreshRate == newRefreshRate)
        {
            return true;
        }

        // Test can the refresh frequency be set for this display:
        deviceMode.dmDisplayFrequency = newRefreshRate;

        var testResult = ChangeDisplaySettingsEx(displayDevice.DeviceName, ref deviceMode, (nint)null, ChangeDisplaySettingsFlags.CDS_TEST, nint.Zero);
        if (testResult != DISP_CHANGE.Successful)
        {
            deviceMode.dmDisplayFrequency = oldRefreshRate; // Set old refresh rate back.
            return false;
        }
        // Valid refresh rate. Update to registry.
        var result = ChangeDisplaySettingsEx(displayDevice.DeviceName, ref deviceMode, (nint)null, (ChangeDisplaySettingsFlags.CDS_UPDATEREGISTRY | ChangeDisplaySettingsFlags.CDS_NORESET), nint.Zero);
        if (result != DISP_CHANGE.Successful)
        {
            throw new Win32Exception((int)result, $"Updating monitor {displayDevice.DeviceID} refresh rate failed.");
        }

        return true;
    }

    /// <summary>
    /// Method applies the stored changes for the standard device methods.
    /// </summary>
    public void ApplyStandardDeviceChanges()
    {
        ChangeDisplaySettingsEx(null, nint.Zero, (nint)null, ChangeDisplaySettingsFlags.CDS_NONE, (nint)null);
    }
    /// <summary>
    /// Uses the <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-enumdisplaydevicesa">EnumDisplaySettings</see> function to obtain information about the display devices.
    /// </summary>
    /// <param name="displayId">Display identifier</param>
    /// <returns>Display information for the given identifier, or null if no device is found for the specified id.</returns>
    public DISPLAY_DEVICE? GetDisplayDevice(uint displayId)
    {
        var device = new DISPLAY_DEVICE();
        device.cb = Marshal.SizeOf(device);
        if (!EnumDisplayDevices(null, displayId, ref device, 0))
        {
            return null;
        }

        return device;
    }
    /// <summary>
    /// Uses the <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-enumdisplaysettingsa">EnumDisplaySettings</see> function to retrieve information about one of the graphics modes for a display device. 
    /// </summary>
    /// <param name="device">DisplayDevice to query the information on.</param>
    /// <returns>Returned information for this device.</returns>
    /// <exception cref="Win32Exception">Something went wrong with the API call.</exception>
    public DEVMODE GetDisplayDeviceMode(DISPLAY_DEVICE device)
    {
        var deviceMode = new DEVMODE();
        if (!EnumDisplaySettings(device.DeviceName, -1, ref deviceMode))
        {
            throw new Win32Exception($"DeviceMode not located for displayId {device.DeviceID}."); // Every device should have a device mode.
        }
        return deviceMode;
    }

    /// <summary>
    /// Uses <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getdisplayconfigbuffersizes">GetDisplayConfigBufferSizes</see> and
    /// <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-querydisplayconfig">QueryDisplayConfig</see> functions
    /// to retrieve information about all possible display paths for all display devices.
    /// </summary>
    /// <returns>A collection of display configuration path information.</returns>
    /// <exception cref="Win32Exception">Something went wrong with the API calls.</exception>
    public IEnumerable<DISPLAYCONFIG_PATH_INFO> GetDisplayConfigPathInformation()
    {
        // Get buffer sizes for the active displays.
        var err = GetDisplayConfigBufferSizes(QDC.QDC_ONLY_ACTIVE_PATHS, out var pathCount, out var modeCount);
        if (err != ResultErrorCode.ERROR_SUCCESS)
        {
            throw new Win32Exception((int)err);
        }
        var paths = new DISPLAYCONFIG_PATH_INFO[pathCount]; // Create arrays to hold the info.
        var modes = new DISPLAYCONFIG_MODE_INFO[modeCount];

        // Get display configs from CCD API:
        err = QueryDisplayConfig(QDC.QDC_ONLY_ACTIVE_PATHS, ref pathCount, paths, ref modeCount, modes, IntPtr.Zero);
        if (err != ResultErrorCode.ERROR_SUCCESS)
        {
            throw new Win32Exception((int)err);
        }

        return paths;
    }

    /// <summary>
    /// Uses the <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-displayconfiggetdeviceinfo">DisplayConfigGetDeviceInfo</see> function retrieves display configuration information about the device.
    /// </summary>
    /// <param name="path">Path information to query</param>
    /// <returns>Returns the WinAPI structure containing advanced color information.</returns>
    /// <exception cref="Win32Exception">Something went wrong with the API call.</exception>
    public DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO GetDisplayConfigurationAdvancedColorInformation(DISPLAYCONFIG_PATH_INFO path)
    {
        var colorInfo = new DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO();
        colorInfo.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_ADVANCED_COLOR_INFO;
        colorInfo.header.size = Marshal.SizeOf<DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO>();
        colorInfo.header.adapterId = path.targetInfo.adapterId;
        colorInfo.header.id = path.targetInfo.id;
        var err = DisplayConfigGetDeviceInfo(ref colorInfo);
        if (err != ResultErrorCode.ERROR_SUCCESS)
        {
            throw new Win32Exception((int)err, $"Error occurred in {nameof(GetDisplayConfigurationAdvancedColorInformation)}");
        }
        return colorInfo;
    }
    /// <summary>
    /// Uses the <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-displayconfiggetdeviceinfo">DisplayConfigGetDeviceInfo</see> function retrieves display configuration information about the device.
    /// </summary>
    /// <param name="path">Path information to query</param>
    /// <returns>Returns the WinAPI structure containing advanced source device information.</returns>
    /// <exception cref="Win32Exception">Something went wrong with the API call.</exception>
    public DISPLAYCONFIG_SOURCE_DEVICE_NAME GetDisplayConfigurationSourceDeviceInformation(DISPLAYCONFIG_PATH_INFO path)
    {
        var sourceInfo = new DISPLAYCONFIG_SOURCE_DEVICE_NAME();
        sourceInfo.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_SOURCE_NAME;
        sourceInfo.header.size = Marshal.SizeOf<DISPLAYCONFIG_SOURCE_DEVICE_NAME>();
        sourceInfo.header.adapterId = path.sourceInfo.adapterId;
        sourceInfo.header.id = path.sourceInfo.id;
        var err = DisplayConfigGetDeviceInfo(ref sourceInfo); // Call for info.
        if (err != ResultErrorCode.ERROR_SUCCESS)
        {
            throw new Win32Exception((int)err, $"Error occurred in {nameof(GetDisplayConfigurationSourceDeviceInformation)}");
        }

        return sourceInfo;
    }
    /// <summary>
    /// Uses the <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-displayconfiggetdeviceinfo">DisplayConfigGetDeviceInfo</see> function retrieves display configuration information about the device.
    /// </summary>
    /// <param name="path">Path information to query</param>
    /// <returns>Returns the WinAPI structure containing advanced target device information.</returns>
    /// <exception cref="Win32Exception">Something went wrong with the API call.</exception>
    public DISPLAYCONFIG_TARGET_DEVICE_NAME GetDisplayConfigurationTargetDeviceInformation(DISPLAYCONFIG_PATH_INFO path)
    {
        var targetInfo = new DISPLAYCONFIG_TARGET_DEVICE_NAME();
        targetInfo.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME;
        targetInfo.header.size = Marshal.SizeOf<DISPLAYCONFIG_TARGET_DEVICE_NAME>();
        targetInfo.header.adapterId = path.targetInfo.adapterId;
        targetInfo.header.id = path.targetInfo.id;
        var err = DisplayConfigGetDeviceInfo(ref targetInfo); // Request target device information:
        if (err != ResultErrorCode.ERROR_SUCCESS)
        {
            throw new Win32Exception((int)err, $"Error occurred in {nameof(GetDisplayConfigurationTargetDeviceInformation)}");
        }

        return targetInfo;
    }

    /// <summary>
    /// Uses the <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-displayconfigsetdeviceinfo">DisplayConfigSetDeviceInfo</see> function sets the properties of a target.
    /// </summary>
    /// <param name="deviceInfoHeader"></param>
    /// <param name="newState">New state for the advanced color mode. True for enabled, False for disabled.</param>
    public void SetDisplayConfigurationAdvancedColorInformation(DISPLAYCONFIG_DEVICE_INFO_HEADER deviceInfoHeader, bool newState)
    {
        // Create the object for setting the state.
        var newColorState = new DISPLAYCONFIG_SET_ADVANCED_COLOR_STATE
        {
            header = deviceInfoHeader with
            {
                type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_SET_ADVANCED_COLOR_STATE, 
                size = Marshal.SizeOf<DISPLAYCONFIG_SET_ADVANCED_COLOR_STATE>()
            },
            value = newState ? DisplayConfigSetAdvancedColorStateValue.Enable : DisplayConfigSetAdvancedColorStateValue.Disable,
        };
        var err = DisplayConfigSetDeviceInfo(ref newColorState);
        if (err != ResultErrorCode.ERROR_SUCCESS)
        {
            throw new Win32Exception((int)err, $"Error occurred while updating AdvancedColorMode for display {deviceInfoHeader.id}.");
        }
    }

    #region Native Methods

    // ReSharper disable CommentTypo
    // ReSharper disable IdentifierTypo
    /// <summary>
    /// The <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-changedisplaysettingsexa">ChangeDisplaySettingsEx</see> function changes the settings of the specified display device to the specified graphics mode.
    /// </summary>
    /// <param name="lpszDeviceName">A pointer to a null-terminated string that specifies the display device whose graphics mode will change. Only display device names as returned by EnumDisplayDevices are valid. 
    /// See EnumDisplayDevices for further information on the names associated with these display devices. <br/> The lpszDeviceName parameter can be NULL. A NULL value specifies the default display device. The default device can be determined by calling EnumDisplayDevices and checking for the DISPLAY_DEVICE_PRIMARY_DEVICE flag.</param>
    /// <param name="lpDevMode">A pointer to a DEVMODE structure that describes the new graphics mode. If lpDevMode is NULL, all the values currently in the registry will be used for the display setting. Passing NULL for the lpDevMode parameter and 0 for the dwFlags parameter is the easiest way to return to the default mode after a dynamic mode change.</param>
    /// <param name="hwnd">Reserved; must be NULL.</param>
    /// <param name="dwflags">Indicates how the graphics mode should be changed. This parameter can be one of the following values.</param>
    /// <param name="lParam">If dwFlags is CDS_VIDEOPARAMETERS, lParam is a pointer to a VIDEOPARAMETERS structure. Otherwise lParam must be NULL.</param>
    /// <returns>Result enumeration</returns>
    [DllImport("user32.dll")]
    private static extern DISP_CHANGE ChangeDisplaySettingsEx(string lpszDeviceName, ref DEVMODE lpDevMode, nint hwnd, ChangeDisplaySettingsFlags dwflags, IntPtr lParam);
    /// <summary>
    /// The <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-changedisplaysettingsexa">ChangeDisplaySettingsEx</see> function changes the settings of the specified display device to the specified graphics mode.
    /// Overload to support DEVMODE as a pointer.
    /// </summary>
    /// <param name="lpszDeviceName">A pointer to a null-terminated string that specifies the display device whose graphics mode will change. Only display device names as returned by EnumDisplayDevices are valid. 
    /// See EnumDisplayDevices for further information on the names associated with these display devices. <br/> The lpszDeviceName parameter can be NULL. A NULL value specifies the default display device. The default device can be determined by calling EnumDisplayDevices and checking for the DISPLAY_DEVICE_PRIMARY_DEVICE flag.</param>
    /// <param name="lpDevMode">A pointer to a DEVMODE structure that describes the new graphics mode. If lpDevMode is NULL, all the values currently in the registry will be used for the display setting. Passing NULL for the lpDevMode parameter and 0 for the dwFlags parameter is the easiest way to return to the default mode after a dynamic mode change.</param>
    /// <param name="hwnd">Reserved; must be NULL.</param>
    /// <param name="dwflags">Indicates how the graphics mode should be changed. This parameter can be one of the following values.</param>
    /// <param name="lParam">If dwFlags is CDS_VIDEOPARAMETERS, lParam is a pointer to a VIDEOPARAMETERS structure. Otherwise lParam must be NULL.</param>
    /// <returns>Result enumeration</returns>
    [DllImport("user32.dll")]

    private static extern DISP_CHANGE ChangeDisplaySettingsEx(string? lpszDeviceName, nint lpDevMode, nint hwnd, ChangeDisplaySettingsFlags dwflags, nint lParam);
    /// <summary>
    /// The <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-enumdisplaydevicesa">EnumDisplaySettings</see> function lets you obtain information about the display devices in the current session.
    /// </summary>
    /// <param name="lpDevice">A pointer to the device name. If NULL, function returns information for the display adapter(s) on the machine, based on iDevNum.</param>
    /// <param name="iDevNum">An index value that specifies the display device of interest. The operating system identifies each display device in the current session with an index value.The index values are consecutive integers, starting at 0. If the current session has three display devices, for example, they are specified by the index values 0, 1, and 2.</param>
    /// <param name="lpDisplayDevice">A pointer to a DISPLAY_DEVICE structure that receives information about the display device specified by iDevNum. Before calling EnumDisplayDevices, you must initialize the cb member of DISPLAY_DEVICE to the size, in bytes, of DISPLAY_DEVICE.</param>
    /// <param name="dwFlags">Set this flag to EDD_GET_DEVICE_INTERFACE_NAME (0x00000001) to retrieve the device interface name for GUID_DEVINTERFACE_MONITOR, which is registered by the operating system on a per monitor basis. The value is placed in the DeviceID member of the DISPLAY_DEVICE structure returned in lpDisplayDevice. 
    /// The resulting device interface name can be used with SetupAPI functions and serves as a link between GDI monitor devices and SetupAPI monitor devices.</param>
    /// <returns>Boolean representing operation success</returns>
    [DllImport("user32.dll")]
    private static extern bool EnumDisplayDevices(string? lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);
    /// <summary>
    /// The <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-enumdisplaysettingsa">EnumDisplaySettings</see> function retrieves information about one of the graphics modes for a display device. 
    /// To retrieve information for all the graphics modes of a display device, make a series of calls to this function.
    /// </summary>
    /// <param name="deviceName">A pointer to a null-terminated string that specifies the display device about whose graphics mode the function will obtain information. This parameter is either NULL or a DISPLAY_DEVICE.DeviceName returned from EnumDisplayDevices.A NULL value specifies the current display device on the computer on which the calling thread is running.</param>
    /// <param name="modeNum">Graphics mode indexes start at zero. To obtain information for all of a display device's graphics modes, make a series of calls to EnumDisplaySettings, as follows: Set iModeNum to zero for the first call, and increment iModeNum by one for each subsequent call. Continue calling the function until the return value is zero.</param>
    /// <param name="devMode">A pointer to a DEVMODE structure into which the function stores information about the specified graphics mode. Before calling EnumDisplaySettings, set the dmSize member to sizeof(DEVMODE), and set the dmDriverExtra member to indicate the size, in bytes, of the additional space available to receive public driver data.</param>
    /// <returns>Boolean representing operation success</returns>
    [DllImport("user32.dll")]
    private static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);

    // CCD Api: https://docs.microsoft.com/en-us/windows-hardware/drivers/display/connecting-and-configuring-displays

    /// <summary>
    /// The <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getdisplayconfigbuffersizes">GetDisplayConfigBufferSizes</see> function retrieves the size of the buffers that are required to call the QueryDisplayConfig function.
    /// </summary>
    /// <param name="flags">The type of information to retrieve. The value for the Flags parameter must be one of the following values.</param>
    /// <param name="numPathArrayElements">ointer to a variable that receives the number of elements in the path information table. The pNumPathArrayElements parameter value is then used by a subsequent call to the QueryDisplayConfig function. This parameter cannot be NULL.</param>
    /// <param name="numModeInfoArrayElements">Pointer to a variable that receives the number of elements in the mode information table. The pNumModeInfoArrayElements parameter value is then used by a subsequent call to the QueryDisplayConfig function. This parameter cannot be NULL.</param>
    /// <returns>The function returns one of the following return codes.</returns>
    [DllImport("user32.dll")]
    private static extern ResultErrorCode GetDisplayConfigBufferSizes(QDC flags, out int numPathArrayElements, out int numModeInfoArrayElements);
    /// <summary>
    /// The <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-querydisplayconfig">QueryDisplayConfig</see> function retrieves information about all possible display paths for all display devices, or views, in the current setting.
    /// </summary>
    /// <param name="flags">The type of information to retrieve. The value for the Flags parameter must use one of the following values.</param>
    /// <param name="numPathArrayElements">Pointer to a variable that contains the number of elements in pPathInfoArray. This parameter cannot be NULL. If QueryDisplayConfig returns ERROR_SUCCESS, pNumPathInfoElements is updated with the number of valid entries in pPathInfoArray.</param>
    /// <param name="pathArray">Pointer to a variable that contains an array of DISPLAYCONFIG_PATH_INFO elements. Each element in pPathInfoArray describes a single path from a source to a target. The source and target mode information indexes are only valid in combination with the pmodeInfoArray tables that are returned for the API at the same time. This parameter cannot be NULL. The pPathInfoArray is always returned in path priority order. For more information about path priority order, see <see href="https://docs.microsoft.com/en-us/windows-hardware/drivers/display/path-priority-order">Path Priority Order.</see></param>
    /// <param name="numModeInfoArrayElements">Pointer to a variable that specifies the number in element of the mode information table. This parameter cannot be NULL. If QueryDisplayConfig returns ERROR_SUCCESS, pNumModeInfoArrayElements is updated with the number of valid entries in pModeInfoArray.</param>
    /// <param name="modeInfoArray">Pointer to a variable that contains an array of DISPLAYCONFIG_MODE_INFO elements. This parameter cannot be NULL.</param>
    /// <param name="currentTopologyId">Pointer to a variable that receives the identifier of the currently active topology in the CCD database. For a list of possible values, see the DISPLAYCONFIG_TOPOLOGY_ID enumerated type.
    /// The pCurrentTopologyId parameter is only set when the Flags parameter value is QDC_DATABASE_CURRENT.
    /// If the Flags parameter value is set to QDC_DATABASE_CURRENT, the pCurrentTopologyId parameter must not be NULL.If the Flags parameter value is not set to QDC_DATABASE_CURRENT, the pCurrentTopologyId parameter value must be NULL.</param>
    /// <returns>The function returns one of the following return codes.</returns>
    [DllImport("user32.dll")]
    private static extern ResultErrorCode QueryDisplayConfig(QDC flags, ref int numPathArrayElements, [In, Out] DISPLAYCONFIG_PATH_INFO[] pathArray, ref int numModeInfoArrayElements, [In, Out] DISPLAYCONFIG_MODE_INFO[] modeInfoArray, nint currentTopologyId);
    /// <summary>
    /// The <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-displayconfiggetdeviceinfo">DisplayConfigGetDeviceInfo</see> function retrieves display configuration information about the device.
    /// </summary>
    /// <param name="requestPacket">A pointer to a DISPLAYCONFIG_DEVICE_INFO_HEADER structure. This structure contains information about the request, which includes the packet type in the type member. The type and size of additional data that DisplayConfigGetDeviceInfo returns after the header structure depend on the packet type.</param>
    /// <returns>The function returns one of the following return codes.</returns>
    [DllImport("user32.dll")]
    private static extern ResultErrorCode DisplayConfigGetDeviceInfo(ref DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO requestPacket);
    /// <summary>
    /// The <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-displayconfiggetdeviceinfo">DisplayConfigGetDeviceInfo</see> function retrieves display configuration information about the device.
    /// </summary>
    /// <param name="requestPacket">A pointer to a DISPLAYCONFIG_DEVICE_INFO_HEADER structure. This structure contains information about the request, which includes the packet type in the type member. The type and size of additional data that DisplayConfigGetDeviceInfo returns after the header structure depend on the packet type.</param>
    /// <returns>The function returns one of the following return codes.</returns>
    [DllImport("user32.dll")]
    private static extern ResultErrorCode DisplayConfigGetDeviceInfo(ref DISPLAYCONFIG_SOURCE_DEVICE_NAME requestPacket);
    /// <summary>
    /// The <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-displayconfiggetdeviceinfo">DisplayConfigGetDeviceInfo</see> function retrieves display configuration information about the device.
    /// </summary>
    /// <param name="requestPacket">A pointer to a DISPLAYCONFIG_DEVICE_INFO_HEADER structure. This structure contains information about the request, which includes the packet type in the type member. The type and size of additional data that DisplayConfigGetDeviceInfo returns after the header structure depend on the packet type.</param>
    /// <returns>The function returns one of the following return codes.</returns>
    [DllImport("user32.dll")]
    private static extern ResultErrorCode DisplayConfigGetDeviceInfo(ref DISPLAYCONFIG_TARGET_DEVICE_NAME requestPacket);
    /// <summary>
    /// The <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-displayconfigsetdeviceinfo">DisplayConfigSetDeviceInfo</see> function sets the properties of a target.
    /// </summary>
    /// <param name="setPacket">A pointer to a DISPLAYCONFIG_DEVICE_INFO_HEADER structure that contains information to set for the device. The type and size of additional data that DisplayConfigSetDeviceInfo uses for the configuration comes after the header structure. This additional data depends on the packet type, as specified by the type member of DISPLAYCONFIG_DEVICE_INFO_HEADER. For example, if the caller wants to change the boot persistence, that caller allocates and fills a DISPLAYCONFIG_SET_TARGET_PERSISTENCE structure and passes a pointer to this structure in setPacket. Note that the first member of the DISPLAYCONFIG_SET_TARGET_PERSISTENCE structure is the DISPLAYCONFIG_DEVICE_INFO_HEADER.</param>
    /// <returns>The function returns one of the following return codes.</returns>
    [DllImport("user32.dll")]
    private static extern ResultErrorCode DisplayConfigSetDeviceInfo([In] ref DISPLAYCONFIG_SET_ADVANCED_COLOR_STATE setPacket);

    // ReSharper restore CommentTypo
    // ReSharper restore IdentifierTypo
    #endregion
}