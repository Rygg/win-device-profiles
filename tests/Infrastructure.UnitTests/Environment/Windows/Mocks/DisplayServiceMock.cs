using System.ComponentModel;
using Infrastructure.Environment.Windows.Common.User32.Interfaces;
using Infrastructure.Environment.Windows.Common.User32.NativeTypes.Structs;
using System.Runtime.InteropServices;
using Infrastructure.Environment.Windows.Common.User32.NativeTypes.Enums;

namespace Infrastructure.UnitTests.Environment.Windows.Mocks;

internal sealed class DisplayServiceMock
{
    internal Mock<IDisplayService> Mock { get; } = new();

    internal IDisplayService GetDisplayService => Mock.Object;
    internal void SetDefaultBehavior()
    {
        SetupGetDisplayDevice();
        SetupGetDeviceModes();
        SetupDisplayConfigPathInformation();
        SetupGetTargetDevices();
        SetupGetSourceDevices();
        SetupGetAdvancedColorState();

        SetupSetStandardDeviceRefreshRate();

        SetupSetDisplayConfigurationAdvancedColorInformation();
    }
    
    private void SetupGetDisplayDevice()
    {
        Mock
            .Setup(m => m.GetDisplayDevice(It.IsAny<uint>()))
            .Returns((DISPLAY_DEVICE?)null);
        Mock
            .Setup(m => m.GetDisplayDevice(0))
            .Returns(display0);
        Mock
            .Setup(m => m.GetDisplayDevice(1))
            .Returns(display1);
        Mock
            .Setup(m => m.GetDisplayDevice(2))
            .Returns(display2);
        Mock
            .Setup(m => m.GetDisplayDevice(3))
            .Returns(display3);
    }

    private void SetupGetDeviceModes()
    {
        Mock
            .Setup(m => m.GetDisplayDeviceMode(It.IsAny<DISPLAY_DEVICE>()))
            .Throws(new Win32Exception("Something went wrong."));
        Mock
            .Setup(m => m.GetDisplayDeviceMode(display0))
            .Returns(devMode0);
        Mock
            .Setup(m => m.GetDisplayDeviceMode(display1))
            .Returns(devMode1);
        Mock
            .Setup(m => m.GetDisplayDeviceMode(display2))
            .Returns(devMode2);
        Mock
            .Setup(m => m.GetDisplayDeviceMode(display3))
            .Returns(devMode3);
    }

    private void SetupDisplayConfigPathInformation()
    {
        Mock
            .Setup(m => m.GetDisplayConfigPathInformation())
            .Returns(new List<DISPLAYCONFIG_PATH_INFO> { path0, path1, path2, path3 });
    }

    private void SetupGetTargetDevices()
    {
        Mock
            .Setup(m => m.GetDisplayConfigurationTargetDeviceInformation(It.IsAny<DISPLAYCONFIG_PATH_INFO>()))
            .Throws(new Win32Exception("Something went wrong."));
        Mock
            .Setup(m => m.GetDisplayConfigurationTargetDeviceInformation(path0))
            .Returns(targetDevice0);
        Mock
            .Setup(m => m.GetDisplayConfigurationTargetDeviceInformation(path1))
            .Returns(targetDevice1);
        Mock
            .Setup(m => m.GetDisplayConfigurationTargetDeviceInformation(path2))
            .Returns(targetDevice2);
        Mock
            .Setup(m => m.GetDisplayConfigurationTargetDeviceInformation(path3))
            .Returns(targetDevice3);
    }
    private void SetupGetSourceDevices()
    {
        Mock
            .Setup(m => m.GetDisplayConfigurationSourceDeviceInformation(It.IsAny<DISPLAYCONFIG_PATH_INFO>()))
            .Throws(new Win32Exception("Something went wrong."));
        Mock
            .Setup(m => m.GetDisplayConfigurationSourceDeviceInformation(path0))
            .Returns(sourceDevice0);
        Mock
            .Setup(m => m.GetDisplayConfigurationSourceDeviceInformation(path1))
            .Returns(sourceDevice1);
        Mock
            .Setup(m => m.GetDisplayConfigurationSourceDeviceInformation(path2))
            .Returns(sourceDevice2);
        Mock
            .Setup(m => m.GetDisplayConfigurationSourceDeviceInformation(path3))
            .Returns(sourceDevice3);
    }

    private void SetupGetAdvancedColorState()
    {
        colorInfo0 = colorInfo0 with { header = _colorInfoHeader0 };
        colorInfo1 = colorInfo1 with { header = _colorInfoHeader1 };
        colorInfo2 = colorInfo2 with { header = _colorInfoHeader2 };
        colorInfo3 = colorInfo3 with { header = _colorInfoHeader3 };

        Mock
            .Setup(m => m.GetDisplayConfigurationAdvancedColorInformation(It.IsAny<DISPLAYCONFIG_PATH_INFO>()))
            .Throws(new Win32Exception("Something went wrong."));
        Mock
            .Setup(m => m.GetDisplayConfigurationAdvancedColorInformation(path0))
            .Returns(colorInfo0);
        Mock
            .Setup(m => m.GetDisplayConfigurationAdvancedColorInformation(path1))
            .Returns(colorInfo1);
        Mock
            .Setup(m => m.GetDisplayConfigurationAdvancedColorInformation(path2))
            .Returns(colorInfo2);
        Mock
            .Setup(m => m.GetDisplayConfigurationAdvancedColorInformation(path3))
            .Returns(colorInfo3);
    }

    private void SetupSetStandardDeviceRefreshRate()
    {
        Mock
            .Setup(m => m.SetStandardDeviceRefreshRate(
                It.IsAny<DISPLAY_DEVICE>(),
                ref It.Ref<DEVMODE>.IsAny,
                It.IsAny<int>())
            )
            .Returns(true);
    }

    private void SetupSetDisplayConfigurationAdvancedColorInformation()
    {


        void UpdateState(DISPLAYCONFIG_DEVICE_INFO_HEADER header, bool booleanValue)
        {
            if (header.Equals(_colorInfoHeader0))
            {
                colorInfo0 = colorInfo0 with
                {
                    value = booleanValue ? DISPLAYCONFIG_ADVANCED_COLOR_INFO_VALUE_FLAGS.AdvancedColorEnabled : DISPLAYCONFIG_ADVANCED_COLOR_INFO_VALUE_FLAGS.AdvancedColorSupported
                };
                return;
            }
            if (header.Equals(_colorInfoHeader1))
            {
                colorInfo1 = colorInfo1 with
                {
                    value = DISPLAYCONFIG_ADVANCED_COLOR_INFO_VALUE_FLAGS.AdvancedColorNotSupported
                };
                return;
            }
            if (header.Equals(_colorInfoHeader1))
            {
                colorInfo2 = colorInfo2 with
                {
                    value = booleanValue ? DISPLAYCONFIG_ADVANCED_COLOR_INFO_VALUE_FLAGS.AdvancedColorEnabled : DISPLAYCONFIG_ADVANCED_COLOR_INFO_VALUE_FLAGS.AdvancedColorSupported
                };
                return;
            }
            if (header.Equals(_colorInfoHeader1))
            {
                colorInfo3 = colorInfo3 with
                {
                    value = DISPLAYCONFIG_ADVANCED_COLOR_INFO_VALUE_FLAGS.AdvancedColorNotSupported
                };

            }
        }
        Mock
            .Setup(m => m.SetDisplayConfigurationAdvancedColorInformation(
                _colorInfoHeader0,
                It.IsAny<bool>())
            )
            .Callback(UpdateState);

    }

    // Devices:
    internal readonly DISPLAY_DEVICE display0 = new()
    {
        cb = Marshal.SizeOf(new DISPLAY_DEVICE()),
        DeviceID = "0",
        DeviceKey = "DevKey1",
        DeviceName = "TestDevice0",
        DeviceString = "TestDeviceString0",
        StateFlags = DisplayDeviceStateFlags.AttachedToDesktop | DisplayDeviceStateFlags.PrimaryDevice
    };

    internal readonly DISPLAY_DEVICE display1 = new()
    {
        cb = Marshal.SizeOf(new DISPLAY_DEVICE()),
        DeviceID = "1",
        DeviceKey = "DevKey1",
        DeviceName = "TestDevice1",
        DeviceString = "TestDeviceString1",
        StateFlags = DisplayDeviceStateFlags.AttachedToDesktop
    };
    internal readonly DISPLAY_DEVICE display2 = new()
    {
        cb = Marshal.SizeOf(new DISPLAY_DEVICE()),
        DeviceID = "2",
        DeviceKey = "DevKey2",
        DeviceName = "TestDevice2",
        DeviceString = "TestDeviceString2",
        StateFlags = DisplayDeviceStateFlags.AttachedToDesktop
    };
    internal readonly DISPLAY_DEVICE display3 = new()
    {
        cb = Marshal.SizeOf(new DISPLAY_DEVICE()),
        DeviceID = "3",
        DeviceKey = "DevKey3",
        DeviceName = "TestDevice3",
        DeviceString = "TestDeviceString3",
    };
    // Modes:
    internal DEVMODE devMode0 = new()
    {
        dmPosition = new POINTL
        {
            x = 0, y = 0,
        },
        dmDisplayFrequency = 144,
        dmPelsWidth = 1920,
        dmPelsHeight = 1080,
    };
    internal DEVMODE devMode1 = new()
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
    internal DEVMODE devMode2 = new()
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
    internal DEVMODE devMode3 = new()
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
    internal readonly DISPLAYCONFIG_PATH_INFO path0 = new()
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
    internal readonly DISPLAYCONFIG_PATH_INFO path1 = new()
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
    internal readonly DISPLAYCONFIG_PATH_INFO path2 = new()
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
    internal readonly DISPLAYCONFIG_PATH_INFO path3 = new()
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
    internal readonly DISPLAYCONFIG_TARGET_DEVICE_NAME targetDevice0 = new()
    {
        monitorFriendlyDeviceName = "Monitor0",
    };
    internal readonly DISPLAYCONFIG_TARGET_DEVICE_NAME targetDevice1 = new()
    {
        monitorFriendlyDeviceName = "Monitor1",
    };
    internal readonly DISPLAYCONFIG_TARGET_DEVICE_NAME targetDevice2 = new()
    {
        monitorFriendlyDeviceName = "Monitor2",
    };
    internal readonly DISPLAYCONFIG_TARGET_DEVICE_NAME targetDevice3 = new()
    {
        monitorFriendlyDeviceName = "Monitor3",
    };
    // Sources:
    internal readonly DISPLAYCONFIG_SOURCE_DEVICE_NAME sourceDevice0 = new()
    {
        viewGdiDeviceName = "TestDevice0"
    };
    internal readonly DISPLAYCONFIG_SOURCE_DEVICE_NAME sourceDevice1 = new()
    {
        viewGdiDeviceName = "TestDevice1"
    };
    internal readonly DISPLAYCONFIG_SOURCE_DEVICE_NAME sourceDevice2 = new()
    {
        viewGdiDeviceName = "TestDevice2"
    };
    internal readonly DISPLAYCONFIG_SOURCE_DEVICE_NAME sourceDevice3 = new()
    {
        viewGdiDeviceName = "TestDevice3"
    };

    internal DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO colorInfo0 = new()
    {
        bitsPerColorChannel = 8,
        colorEncoding = DISPLAYCONFIG_COLOR_ENCODING.DISPLAYCONFIG_COLOR_ENCODING_RGB,
        value = DISPLAYCONFIG_ADVANCED_COLOR_INFO_VALUE_FLAGS.AdvancedColorEnabled
    };
    internal DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO colorInfo1 = new()
    {
        bitsPerColorChannel = 8,
        colorEncoding = DISPLAYCONFIG_COLOR_ENCODING.DISPLAYCONFIG_COLOR_ENCODING_RGB,
        value = DISPLAYCONFIG_ADVANCED_COLOR_INFO_VALUE_FLAGS.AdvancedColorNotSupported
    };
    internal DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO colorInfo2 = new()
    {
        bitsPerColorChannel = 8,
        colorEncoding = DISPLAYCONFIG_COLOR_ENCODING.DISPLAYCONFIG_COLOR_ENCODING_RGB,
        value = DISPLAYCONFIG_ADVANCED_COLOR_INFO_VALUE_FLAGS.AdvancedColorSupported
    };
    internal DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO colorInfo3 = new()
    {
        bitsPerColorChannel = 8,
        colorEncoding = DISPLAYCONFIG_COLOR_ENCODING.DISPLAYCONFIG_COLOR_ENCODING_YCBCR420,
        value = DISPLAYCONFIG_ADVANCED_COLOR_INFO_VALUE_FLAGS.AdvancedColorNotSupported
    };

    private readonly DISPLAYCONFIG_DEVICE_INFO_HEADER _colorInfoHeader0 = new();
    private readonly DISPLAYCONFIG_DEVICE_INFO_HEADER _colorInfoHeader1 = new();
    private readonly DISPLAYCONFIG_DEVICE_INFO_HEADER _colorInfoHeader2 = new();
    private readonly DISPLAYCONFIG_DEVICE_INFO_HEADER _colorInfoHeader3 = new();
}