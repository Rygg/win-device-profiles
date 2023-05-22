using System.ComponentModel;
using Infrastructure.Environment.Windows.Common.User32.Interfaces;
using Infrastructure.Environment.Windows.Common.User32.NativeTypes.Structs;
using System.Runtime.InteropServices;
using Infrastructure.Environment.Windows.Common.User32.NativeTypes.Enums;

namespace Infrastructure.UnitTests.Environment.Windows.Mocks;

internal static class DisplayServiceMock
{
    private static readonly Mock<IDisplayService> DisplayService = new();

    internal static IDisplayService GetDisplayService => DisplayService.Object;
    internal static void ClearInvocations() => DisplayService.Invocations.Clear();
    internal static void SetDefaultBehavior()
    {
        SetupGetDisplayDevice();
        SetupGetDeviceModes();
        SetupDisplayConfigPathInformation();
        SetupGetTargetDevices();
        SetupGetSourceDevices();
        SetupGetAdvancedColorState();



        SetupSetStandardDeviceRefreshRate();
    }
    
    private static void SetupGetDisplayDevice()
    {
        DisplayService
            .Setup(m => m.GetDisplayDevice(It.IsAny<uint>()))
            .Returns((DISPLAY_DEVICE?)null);
        DisplayService
            .Setup(m => m.GetDisplayDevice(0))
            .Returns(Display0);
        DisplayService
            .Setup(m => m.GetDisplayDevice(1))
            .Returns(Display1);
        DisplayService
            .Setup(m => m.GetDisplayDevice(2))
            .Returns(Display2);
        DisplayService
            .Setup(m => m.GetDisplayDevice(3))
            .Returns(Display3);
    }

    private static void SetupGetDeviceModes()
    {
        DisplayService
            .Setup(m => m.GetDisplayDeviceMode(It.IsAny<DISPLAY_DEVICE>()))
            .Throws(new Win32Exception("Something went wrong."));
        DisplayService
            .Setup(m => m.GetDisplayDeviceMode(Display0))
            .Returns(DevMode0);
        DisplayService
            .Setup(m => m.GetDisplayDeviceMode(Display1))
            .Returns(DevMode1);
        DisplayService
            .Setup(m => m.GetDisplayDeviceMode(Display2))
            .Returns(DevMode2);
        DisplayService
            .Setup(m => m.GetDisplayDeviceMode(Display3))
            .Returns(DevMode3);
    }

    private static void SetupDisplayConfigPathInformation()
    {
        DisplayService
            .Setup(m => m.GetDisplayConfigPathInformation())
            .Returns(new List<DISPLAYCONFIG_PATH_INFO> { Path0, Path1, Path2, Path3 });
    }

    private static void SetupGetTargetDevices()
    {
        DisplayService
            .Setup(m => m.GetDisplayConfigurationTargetDeviceInformation(It.IsAny<DISPLAYCONFIG_PATH_INFO>()))
            .Throws(new Win32Exception("Something went wrong."));
        DisplayService
            .Setup(m => m.GetDisplayConfigurationTargetDeviceInformation(Path0))
            .Returns(TargetDevice0);
        DisplayService
            .Setup(m => m.GetDisplayConfigurationTargetDeviceInformation(Path1))
            .Returns(TargetDevice1);
        DisplayService
            .Setup(m => m.GetDisplayConfigurationTargetDeviceInformation(Path2))
            .Returns(TargetDevice2);
        DisplayService
            .Setup(m => m.GetDisplayConfigurationTargetDeviceInformation(Path3))
            .Returns(TargetDevice3);
    }
    private static void SetupGetSourceDevices()
    {
        DisplayService
            .Setup(m => m.GetDisplayConfigurationSourceDeviceInformation(It.IsAny<DISPLAYCONFIG_PATH_INFO>()))
            .Throws(new Win32Exception("Something went wrong."));
        DisplayService
            .Setup(m => m.GetDisplayConfigurationSourceDeviceInformation(Path0))
            .Returns(SourceDevice0);
        DisplayService
            .Setup(m => m.GetDisplayConfigurationSourceDeviceInformation(Path1))
            .Returns(SourceDevice1);
        DisplayService
            .Setup(m => m.GetDisplayConfigurationSourceDeviceInformation(Path2))
            .Returns(SourceDevice2);
        DisplayService
            .Setup(m => m.GetDisplayConfigurationSourceDeviceInformation(Path3))
            .Returns(SourceDevice3);
    }

    private static void SetupGetAdvancedColorState()
    {
        DisplayService
            .Setup(m => m.GetDisplayConfigurationAdvancedColorInformation(It.IsAny<DISPLAYCONFIG_PATH_INFO>()))
            .Throws(new Win32Exception("Something went wrong."));
        DisplayService
            .Setup(m => m.GetDisplayConfigurationAdvancedColorInformation(Path0))
            .Returns(ColorInfo0);
        DisplayService
            .Setup(m => m.GetDisplayConfigurationAdvancedColorInformation(Path1))
            .Returns(ColorInfo1);
        DisplayService
            .Setup(m => m.GetDisplayConfigurationAdvancedColorInformation(Path2))
            .Returns(ColorInfo2);
        DisplayService
            .Setup(m => m.GetDisplayConfigurationAdvancedColorInformation(Path3))
            .Returns(ColorInfo3);
    }

    private static void SetupSetStandardDeviceRefreshRate()
    {
        void UpdateRefreshRate(DISPLAY_DEVICE device, ref DEVMODE devMode, int refreshRate)
        {
            devMode.dmDisplayFrequency = refreshRate;
        }

        DisplayService
            .Setup(m => m.SetStandardDeviceRefreshRate(
                It.IsAny<DISPLAY_DEVICE>(),
                ref It.Ref<DEVMODE>.IsAny,
                It.IsAny<int>())
            )
            .Callback(UpdateRefreshRate)
            .Returns(true);
    }
    // Devices:
    private static readonly DISPLAY_DEVICE Display0 = new()
    {
        cb = Marshal.SizeOf(new DISPLAY_DEVICE()),
        DeviceID = "0",
        DeviceKey = "DevKey1",
        DeviceName = "TestDevice0",
        DeviceString = "TestDeviceString0",
        StateFlags = DisplayDeviceStateFlags.AttachedToDesktop | DisplayDeviceStateFlags.PrimaryDevice
    };

    internal static readonly DISPLAY_DEVICE Display1 = new()
    {
        cb = Marshal.SizeOf(new DISPLAY_DEVICE()),
        DeviceID = "1",
        DeviceKey = "DevKey1",
        DeviceName = "TestDevice1",
        DeviceString = "TestDeviceString1",
        StateFlags = DisplayDeviceStateFlags.AttachedToDesktop
    };
    internal static readonly DISPLAY_DEVICE Display2 = new()
    {
        cb = Marshal.SizeOf(new DISPLAY_DEVICE()),
        DeviceID = "2",
        DeviceKey = "DevKey2",
        DeviceName = "TestDevice2",
        DeviceString = "TestDeviceString2",
        StateFlags = DisplayDeviceStateFlags.AttachedToDesktop
    };
    internal static readonly DISPLAY_DEVICE Display3 = new()
    {
        cb = Marshal.SizeOf(new DISPLAY_DEVICE()),
        DeviceID = "3",
        DeviceKey = "DevKey3",
        DeviceName = "TestDevice3",
        DeviceString = "TestDeviceString3",
    };
    // Modes:
    internal static readonly DEVMODE DevMode0 = new()
    {
        dmPosition = new POINTL
        {
            x = 0, y = 0,
        },
        dmDisplayFrequency = 144,
        dmPelsWidth = 1920,
        dmPelsHeight = 1080,
    };
    internal static readonly DEVMODE DevMode1 = new()
    {
        dmPosition = new POINTL
        {
            x = 1920,
            y = 0,
        },
        dmDisplayFrequency = 60,
        dmPelsWidth = 1920,
        dmPelsHeight = 1080,
    };
    internal static readonly DEVMODE DevMode2 = new()
    {
        dmPosition = new POINTL
        {
            x = 1920,
            y = 1080,
        },
        dmDisplayFrequency = 120,
        dmPelsWidth = 3840,
        dmPelsHeight = 2160,
    };
    internal static readonly DEVMODE DevMode3 = new()
    {
        dmPosition = new POINTL
        {
            x = -1920,
            y = 0,
        },
        dmDisplayFrequency = 60,
        dmPelsWidth = 1920,
        dmPelsHeight = 1080,
    };
    // Paths:
    internal static readonly DISPLAYCONFIG_PATH_INFO Path0 = new()
    {
        sourceInfo = new DISPLAYCONFIG_PATH_SOURCE_INFO
        {
            id = 0,
        },
        targetInfo = new DISPLAYCONFIG_PATH_TARGET_INFO
        {
            id = 0
        }
    };
    internal static readonly DISPLAYCONFIG_PATH_INFO Path1 = new()
    {
        sourceInfo = new DISPLAYCONFIG_PATH_SOURCE_INFO
        {
            id = 1,
        },
        targetInfo = new DISPLAYCONFIG_PATH_TARGET_INFO
        {
            id = 1
        }
    };
    internal static readonly DISPLAYCONFIG_PATH_INFO Path2 = new()
    {
        sourceInfo = new DISPLAYCONFIG_PATH_SOURCE_INFO
        {
            id = 2,
        },
        targetInfo = new DISPLAYCONFIG_PATH_TARGET_INFO
        {
            id = 2
        }
    };
    internal static readonly DISPLAYCONFIG_PATH_INFO Path3 = new()
    {
        sourceInfo = new DISPLAYCONFIG_PATH_SOURCE_INFO
        {
            id = 3,
        },
        targetInfo = new DISPLAYCONFIG_PATH_TARGET_INFO
        {
            id = 3
        }
    };
    // Targets:
    internal static readonly DISPLAYCONFIG_TARGET_DEVICE_NAME TargetDevice0 = new()
    {
        monitorFriendlyDeviceName = "Monitor0",
    };
    internal static readonly DISPLAYCONFIG_TARGET_DEVICE_NAME TargetDevice1 = new()
    {
        monitorFriendlyDeviceName = "Monitor1",
    };
    internal static readonly DISPLAYCONFIG_TARGET_DEVICE_NAME TargetDevice2 = new()
    {
        monitorFriendlyDeviceName = "Monitor2",
    };
    internal static readonly DISPLAYCONFIG_TARGET_DEVICE_NAME TargetDevice3 = new()
    {
        monitorFriendlyDeviceName = "Monitor3",
    };
    // Sources:
    internal static readonly DISPLAYCONFIG_SOURCE_DEVICE_NAME SourceDevice0 = new()
    {
        viewGdiDeviceName = "TestDevice0"
    };
    internal static readonly DISPLAYCONFIG_SOURCE_DEVICE_NAME SourceDevice1 = new()
    {
        viewGdiDeviceName = "TestDevice1"
    };
    internal static readonly DISPLAYCONFIG_SOURCE_DEVICE_NAME SourceDevice2 = new()
    {
        viewGdiDeviceName = "TestDevice2"
    };
    internal static readonly DISPLAYCONFIG_SOURCE_DEVICE_NAME SourceDevice3 = new()
    {
        viewGdiDeviceName = "TestDevice3"
    };

    internal static readonly DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO ColorInfo0 = new()
    {
        bitsPerColorChannel = 8,
        colorEncoding = DISPLAYCONFIG_COLOR_ENCODING.DISPLAYCONFIG_COLOR_ENCODING_RGB,
        header = new DISPLAYCONFIG_DEVICE_INFO_HEADER(),
        value = DISPLAYCONFIG_ADVANCED_COLOR_INFO_VALUE_FLAGS.AdvancedColorEnabled
    };
    internal static readonly DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO ColorInfo1 = new()
    {
        bitsPerColorChannel = 8,
        colorEncoding = DISPLAYCONFIG_COLOR_ENCODING.DISPLAYCONFIG_COLOR_ENCODING_RGB,
        header = new DISPLAYCONFIG_DEVICE_INFO_HEADER(),
        value = DISPLAYCONFIG_ADVANCED_COLOR_INFO_VALUE_FLAGS.AdvancedColorNotSupported
    };
    internal static readonly DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO ColorInfo2 = new()
    {
        bitsPerColorChannel = 8,
        colorEncoding = DISPLAYCONFIG_COLOR_ENCODING.DISPLAYCONFIG_COLOR_ENCODING_RGB,
        header = new DISPLAYCONFIG_DEVICE_INFO_HEADER(),
        value = DISPLAYCONFIG_ADVANCED_COLOR_INFO_VALUE_FLAGS.AdvancedColorSupported
    };
    internal static readonly DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO ColorInfo3 = new()
    {
        bitsPerColorChannel = 8,
        colorEncoding = DISPLAYCONFIG_COLOR_ENCODING.DISPLAYCONFIG_COLOR_ENCODING_YCBCR420,
        header = new DISPLAYCONFIG_DEVICE_INFO_HEADER(),
        value = DISPLAYCONFIG_ADVANCED_COLOR_INFO_VALUE_FLAGS.AdvancedColorNotSupported
    };
}