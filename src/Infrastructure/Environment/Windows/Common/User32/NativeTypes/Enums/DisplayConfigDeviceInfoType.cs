namespace DeviceProfiles.Infrastructure.Environment.Windows.Common.User32.NativeTypes.Enums;

/// <summary>
/// he DISPLAYCONFIG_DEVICE_INFO_TYPE enumeration specifies the type of display device info to configure or obtain through the DisplayConfigSetDeviceInfo or DisplayConfigGetDeviceInfo function.
/// </summary>
#pragma warning disable CA1712
internal enum DISPLAYCONFIG_DEVICE_INFO_TYPE
{
    /// <summary>
    /// Specifies the source name of the display device. If the DisplayConfigGetDeviceInfo function is successful, DisplayConfigGetDeviceInfo returns the source name in the DISPLAYCONFIG_SOURCE_DEVICE_NAME structure.
    /// </summary>
    DISPLAYCONFIG_DEVICE_INFO_GET_SOURCE_NAME = 1,
    /// <summary>
    /// Specifies information about the monitor. If the DisplayConfigGetDeviceInfo function is successful, DisplayConfigGetDeviceInfo returns info about the monitor in the DISPLAYCONFIG_TARGET_DEVICE_NAME structure.
    /// </summary>
    DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME = 2,
    /// <summary>
    /// Specifies information about the preferred mode of a monitor. If the DisplayConfigGetDeviceInfo function is successful, DisplayConfigGetDeviceInfo returns info about the preferred mode of a monitor in the DISPLAYCONFIG_TARGET_PREFERRED_MODE structure.
    /// </summary>
    DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_PREFERRED_MODE = 3,
    /// <summary>
    /// Specifies the graphics adapter name. If the DisplayConfigGetDeviceInfo function is successful, DisplayConfigGetDeviceInfo returns the adapter name in the DISPLAYCONFIG_ADAPTER_NAME structure.
    /// </summary>
    DISPLAYCONFIG_DEVICE_INFO_GET_ADAPTER_NAME = 4,
    /// <summary>
    /// Specifies how to set the monitor. If the DisplayConfigSetDeviceInfo function is successful, DisplayConfigSetDeviceInfo uses info in the DISPLAYCONFIG_SET_TARGET_PERSISTENCE structure to force the output in a boot-persistent manner.
    /// </summary>
    DISPLAYCONFIG_DEVICE_INFO_SET_TARGET_PERSISTENCE = 5,
    /// <summary>
    /// Specifies how to set the base output technology for a given target ID. If the DisplayConfigGetDeviceInfo function is successful, DisplayConfigGetDeviceInfo returns base output technology info in the DISPLAYCONFIG_TARGET_BASE_TYPE structure. Supported by WDDM 1.3 and later user-mode display drivers running on Windows 8.1 and later.
    /// </summary>
    DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_BASE_TYPE = 6,
    /// <summary>
    /// Specifies the state of virtual mode support. If the DisplayConfigGetDeviceInfo function is successful, DisplayConfigGetDeviceInfo returns virtual mode support information in the DISPLAYCONFIG_SUPPORT_VIRTUAL_RESOLUTION structure. Supported starting in Windows 10.
    /// </summary>
    DISPLAYCONFIG_DEVICE_INFO_GET_SUPPORT_VIRTUAL_RESOLUTION = 7,
    /// <summary>
    /// Specifies how to set the state of virtual mode support. If the DisplayConfigSetDeviceInfo function is successful, DisplayConfigSetDeviceInfo uses info in the DISPLAYCONFIG_SUPPORT_VIRTUAL_RESOLUTION structure to change the state of virtual mode support. Supported starting in Windows 10.
    /// </summary>
    DISPLAYCONFIG_DEVICE_INFO_SET_SUPPORT_VIRTUAL_RESOLUTION = 8,
    /// <summary>
    /// Specifies the advanced color information. If the DisplayConfigGetDeviceInfo function is successful, DisplayConfigGetDeviceInfo returns the advanced color information in the DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO structure.
    /// </summary>
    DISPLAYCONFIG_DEVICE_INFO_GET_ADVANCED_COLOR_INFO = 9,
    /// <summary>
    /// Specifies how to set the state of advanced color information. If the DisplayConfigSetDeviceInfo function is successful, DisplayConfigSetDeviceInfo uses info in the DISPLAYCONFIG_SET_ADVANCED_COLOR_STATE structure to change the state of advanced color mode.
    /// </summary>
    DISPLAYCONFIG_DEVICE_INFO_SET_ADVANCED_COLOR_STATE = 10,
    /// <summary>
    /// Specifies the current SDR white level for an HDR monitor. If the DisplayConfigGetDeviceInfo function is successful, DisplayConfigGetDeviceInfo return SDR white level info in the DISPLAYCONFIG_SDR_WHITE_LEVEL structure. Supported starting in Windows 10 Fall Creators Update (Version 1709).
    /// </summary>
    DISPLAYCONFIG_DEVICE_INFO_GET_SDR_WHITE_LEVEL = 11,
}
#pragma warning restore CA1712