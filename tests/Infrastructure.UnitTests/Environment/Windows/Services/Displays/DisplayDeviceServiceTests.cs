using System.ComponentModel;
using Domain.Entities;
using Domain.ValueObjects;
using Infrastructure.Environment.Windows.Common.User32.NativeTypes.Enums;
using Infrastructure.Environment.Windows.Common.User32.NativeTypes.Structs;
using Infrastructure.Environment.Windows.Services.Displays;
using Infrastructure.UnitTests.Environment.Windows.Mocks;
using Microsoft.Extensions.Logging;

namespace Infrastructure.UnitTests.Environment.Windows.Services.Displays;

[TestFixture]
public sealed class DisplayDeviceServiceTests
{
    private readonly Mock<ILogger<DisplayDeviceService>> _logger = new();

    private DisplayServiceMock _displayMock = null!;

    [SetUp]
    public void SetUp()
    {
        _displayMock = new DisplayServiceMock();
        _displayMock.SetDefaultBehavior();
    }

    [Test]
    public async Task GetCurrentDisplayInformationString_ValidData_ReturnsCorrectDataInString()
    {
        var sut = new DisplayDeviceService(_logger.Object, _displayMock.GetDisplayService);
        var str = await sut.GetCurrentDisplayInformationString(CancellationToken.None);
        str.Should().Contain(_displayMock.display0.DeviceName);
        str.Should().Contain(_displayMock.display0.DeviceString);
        str.Should().Contain(_displayMock.display0.DeviceKey);
        str.Should().Contain(_displayMock.display0.StateFlags.ToString());
        str.Should().Contain(_displayMock.targetDevice0.monitorFriendlyDeviceName);
        str.Should().Contain($"{_displayMock.devMode0.dmPelsWidth}x{_displayMock.devMode0.dmPelsHeight}@{_displayMock.devMode0.dmDisplayFrequency}Hz");
        str.Should().Contain($"{_displayMock.devMode0.dmPosition.x},{_displayMock.devMode0.dmPosition.y}");
        str.Should().Contain(_displayMock.colorInfo0.colorEncoding.ToString());
        str.Should().Contain(_displayMock.colorInfo0.bitsPerColorChannel.ToString());
        str.Should().Contain(_displayMock.colorInfo0.value.ToString());
        str.Should().Contain(_displayMock.display1.DeviceName);
        str.Should().Contain(_displayMock.display1.DeviceString);
        str.Should().Contain(_displayMock.display1.DeviceKey);
        str.Should().Contain(_displayMock.display1.StateFlags.ToString());
        str.Should().Contain(_displayMock.targetDevice1.monitorFriendlyDeviceName);
        str.Should().Contain($"{_displayMock.devMode1.dmPelsWidth}x{_displayMock.devMode1.dmPelsHeight}@{_displayMock.devMode1.dmDisplayFrequency}Hz");
        str.Should().Contain($"{_displayMock.devMode1.dmPosition.x},{_displayMock.devMode1.dmPosition.y}");
        str.Should().Contain(_displayMock.colorInfo1.colorEncoding.ToString());
        str.Should().Contain(_displayMock.colorInfo1.bitsPerColorChannel.ToString());
        str.Should().Contain(_displayMock.colorInfo1.value.ToString());
        str.Should().Contain(_displayMock.display2.DeviceName);
        str.Should().Contain(_displayMock.display2.DeviceString);
        str.Should().Contain(_displayMock.display2.DeviceKey);
        str.Should().Contain(_displayMock.display2.StateFlags.ToString());
        str.Should().Contain(_displayMock.targetDevice2.monitorFriendlyDeviceName);
        str.Should().Contain($"{_displayMock.devMode2.dmPelsWidth}x{_displayMock.devMode2.dmPelsHeight}@{_displayMock.devMode2.dmDisplayFrequency}Hz");
        str.Should().Contain($"{_displayMock.devMode2.dmPosition.x},{_displayMock.devMode2.dmPosition.y}");
        str.Should().Contain(_displayMock.colorInfo2.colorEncoding.ToString());
        str.Should().Contain(_displayMock.colorInfo2.bitsPerColorChannel.ToString());
        str.Should().Contain(_displayMock.colorInfo2.value.ToString());
        str.Should().NotContain(_displayMock.display3.DeviceName); // Not attached to desktop.
        str.Should().NotContain(_displayMock.display3.DeviceString);
        str.Should().NotContain(_displayMock.display3.DeviceKey);

        _displayMock.Mock.Verify(m => m.GetDisplayDevice(0), Times.Once);
        _displayMock.Mock.Verify(m => m.GetDisplayDevice(1), Times.Once);
        _displayMock.Mock.Verify(m => m.GetDisplayDevice(2), Times.Once);
        _displayMock.Mock.Verify(m => m.GetDisplayDevice(3), Times.Once);
        _displayMock.Mock.Verify(m => m.GetDisplayDeviceMode(_displayMock.display0), Times.Once);
        _displayMock.Mock.Verify(m => m.GetDisplayDeviceMode(_displayMock.display1), Times.Once);
        _displayMock.Mock.Verify(m => m.GetDisplayDeviceMode(_displayMock.display2), Times.Once);
        _displayMock.Mock.Verify(m => m.GetDisplayDeviceMode(_displayMock.display3), Times.Never);
    }

    [Test]
    public async Task ChangeDisplaySettings_ChangeRefreshRate_ReturnsTrueAndCallsCorrectMethods()
    {
        var deviceProfile = new DeviceProfile
        {
            Id = 0,
            Name = "TestProfile1",
            DisplaySettings = new List<DisplaySettings>
            {
                new()
                {
                    DisplayId = 0,
                    RefreshRate = 60,
                }
            }
        };

        var sut = new DisplayDeviceService(_logger.Object, _displayMock.GetDisplayService);
        var result = await sut.ChangeDisplaySettings(deviceProfile, CancellationToken.None);
        result.Should().BeTrue();

        _displayMock.devMode0.dmDisplayFrequency.Should().Be(60);

        _displayMock.Mock.Verify(m => m.SetStandardDeviceRefreshRate(
            It.Is<DISPLAY_DEVICE>(d => d.DeviceName == _displayMock.display0.DeviceName),
            ref It.Ref<DEVMODE>.IsAny,
        60), Times.Once());

        _displayMock.Mock.Verify(m => m.SetStandardDeviceRefreshRate(
            It.IsAny<DISPLAY_DEVICE>(),
            ref It.Ref<DEVMODE>.IsAny,
            It.IsAny<int>()), Times.Once);

        _displayMock.Mock.Verify(m => m.SetStandardDeviceAsPrimaryDisplay(It.IsAny<DISPLAY_DEVICE>(), ref It.Ref<DEVMODE>.IsAny), Times.Never);
        _displayMock.Mock.Verify(m => m.SetStandardDeviceDeviceMode(It.IsAny<DISPLAY_DEVICE>(), ref It.Ref<DEVMODE>.IsAny), Times.Never);
        _displayMock.Mock.Verify(m => m.SetDisplayConfigurationAdvancedColorInformation(It.IsAny<DISPLAYCONFIG_DEVICE_INFO_HEADER>(), It.IsAny<bool>()), Times.Never);
        _displayMock.Mock.Verify(m => m.ApplyStandardDeviceChanges(), Times.Once);
    }

    [Test]
    public async Task ChangeDisplaySettings_ChangeToSameRefreshRate_ReturnsFalseAndDoesNotApplyChanges()
    {
        _displayMock.Mock
            .Setup(m => m.SetStandardDeviceRefreshRate(
                It.IsAny<DISPLAY_DEVICE>(),
                ref It.Ref<DEVMODE>.IsAny,
                It.IsAny<int>())
            )
            .Returns(false);

        var deviceProfile = new DeviceProfile
        {
            Id = 0,
            Name = "TestProfile1",
            DisplaySettings = new List<DisplaySettings>
            {
                new()
                {
                    DisplayId = 0,
                    RefreshRate = 144,
                }
            }
        };


        var sut = new DisplayDeviceService(_logger.Object, _displayMock.GetDisplayService);
        var result = await sut.ChangeDisplaySettings(deviceProfile, CancellationToken.None);
        result.Should().BeFalse();
        _displayMock.devMode0.dmDisplayFrequency.Should().Be(144);

        _displayMock.Mock.Verify(m => m.SetStandardDeviceRefreshRate(
            It.Is<DISPLAY_DEVICE>(d => d.DeviceName == _displayMock.display0.DeviceName),
            ref It.Ref<DEVMODE>.IsAny,
            144), Times.Once());

        _displayMock.Mock.Verify(m => m.SetStandardDeviceRefreshRate(
            It.IsAny<DISPLAY_DEVICE>(),
            ref It.Ref<DEVMODE>.IsAny,
            It.IsAny<int>()), Times.Once);

        _displayMock.Mock.Verify(
            m => m.SetStandardDeviceAsPrimaryDisplay(
                It.IsAny<DISPLAY_DEVICE>(), 
                ref It.Ref<DEVMODE>.IsAny), 
            Times.Never);
        _displayMock.Mock.Verify(
            m => m.SetStandardDeviceDeviceMode(
                It.IsAny<DISPLAY_DEVICE>(), 
                ref It.Ref<DEVMODE>.IsAny), 
            Times.Never);
        _displayMock.Mock.Verify(
            m => m.SetDisplayConfigurationAdvancedColorInformation(
                It.IsAny<DISPLAYCONFIG_DEVICE_INFO_HEADER>(), 
                It.IsAny<bool>()), 
            Times.Never);
        _displayMock.Mock.Verify(m => m.ApplyStandardDeviceChanges(), Times.Never);
    }

    [Test]
    public async Task ChangeDisplaySettings_RefreshRateIsUnsupported_ReturnsFalseAndDoesNotApplyChanges()
    {
        _displayMock.Mock
            .Setup(m => m.SetStandardDeviceRefreshRate(
                It.IsAny<DISPLAY_DEVICE>(),
                ref It.Ref<DEVMODE>.IsAny,
                It.IsAny<int>())
            )
            .Throws(new InvalidOperationException("Not supported"));

        var deviceProfile = new DeviceProfile
        {
            Id = 0,
            Name = "TestProfile1",
            DisplaySettings = new List<DisplaySettings>
            {
                new()
                {
                    DisplayId = 0,
                    RefreshRate = 154,
                }
            }
        };

        var sut = new DisplayDeviceService(_logger.Object, _displayMock.GetDisplayService);
        var result = await sut.ChangeDisplaySettings(deviceProfile, CancellationToken.None);
        result.Should().BeFalse();
        _displayMock.devMode0.dmDisplayFrequency.Should().Be(144);

        _displayMock.Mock.Verify(m => m.SetStandardDeviceRefreshRate(
            It.Is<DISPLAY_DEVICE>(d => d.DeviceName == _displayMock.display0.DeviceName),
            ref It.Ref<DEVMODE>.IsAny,
            154), Times.Once());

        _displayMock.Mock.Verify(m => m.SetStandardDeviceRefreshRate(
            It.IsAny<DISPLAY_DEVICE>(),
            ref It.Ref<DEVMODE>.IsAny,
            It.IsAny<int>()), Times.Once);

        _displayMock.Mock.Verify(
            m => m.SetStandardDeviceAsPrimaryDisplay(
                It.IsAny<DISPLAY_DEVICE>(), 
                ref It.Ref<DEVMODE>.IsAny), 
            Times.Never);
        _displayMock.Mock.Verify(
            m => m.SetStandardDeviceDeviceMode(
                It.IsAny<DISPLAY_DEVICE>(), 
                ref It.Ref<DEVMODE>.IsAny), 
            Times.Never);
        _displayMock.Mock.Verify(
            m => m.SetDisplayConfigurationAdvancedColorInformation(
                It.IsAny<DISPLAYCONFIG_DEVICE_INFO_HEADER>(),
                It.IsAny<bool>()), 
            Times.Never);
        _displayMock.Mock.Verify(m => m.ApplyStandardDeviceChanges(), Times.Never);
    }

    [Test]
    public async Task ChangeDisplaySettings_WinApiException_ThrowsException()
    {
        _displayMock.Mock
            .Setup(m => m.SetStandardDeviceRefreshRate(
                It.IsAny<DISPLAY_DEVICE>(),
                ref It.Ref<DEVMODE>.IsAny,
                It.IsAny<int>())
            )
            .Throws(new Win32Exception("Error"));

        var deviceProfile = new DeviceProfile
        {
            Id = 0,
            Name = "TestProfile1",
            DisplaySettings = new List<DisplaySettings>
            {
                new()
                {
                    DisplayId = 0,
                    RefreshRate = 154,
                }
            }
        };

        var sut = new DisplayDeviceService(_logger.Object, _displayMock.GetDisplayService);
        var testAction = async () =>  await sut.ChangeDisplaySettings(deviceProfile, CancellationToken.None);
        await testAction.Should().ThrowAsync<Win32Exception>();

        _displayMock.Mock.Verify(
            m => m.SetStandardDeviceRefreshRate(
                It.Is<DISPLAY_DEVICE>(d => d.DeviceName == _displayMock.display0.DeviceName),
                ref It.Ref<DEVMODE>.IsAny,
                154), 
            Times.Once());

        _displayMock.Mock.Verify(
            m => m.SetStandardDeviceRefreshRate(
                It.IsAny<DISPLAY_DEVICE>(),
                ref It.Ref<DEVMODE>.IsAny,
                It.IsAny<int>()), 
            Times.Once);

        _displayMock.Mock.Verify(
            m => m.SetStandardDeviceAsPrimaryDisplay(
                It.IsAny<DISPLAY_DEVICE>(), 
                ref It.Ref<DEVMODE>.IsAny), 
            Times.Never);
        _displayMock.Mock.Verify(
            m => m.SetStandardDeviceDeviceMode(
                It.IsAny<DISPLAY_DEVICE>(), 
                ref It.Ref<DEVMODE>.IsAny), 
            Times.Never);
        _displayMock.Mock.Verify(
            m => m.SetDisplayConfigurationAdvancedColorInformation(
                It.IsAny<DISPLAYCONFIG_DEVICE_INFO_HEADER>(), 
                It.IsAny<bool>()), 
            Times.Never);
        _displayMock.Mock.Verify(m => m.ApplyStandardDeviceChanges(), Times.Never);
    }

    [Test]
    public async Task ChangeDisplaySettings_SetExistingPrimaryDisplayAsPrimary_ReturnsFalseAndDoesNotApplyChanges()
    {
        var deviceProfile = new DeviceProfile
        {
            Id = 0,
            Name = "TestProfile1",
            DisplaySettings = new List<DisplaySettings>
            {
                new()
                {
                    DisplayId = 0,
                    PrimaryDisplay = true,
                }
            }
        };

        var sut = new DisplayDeviceService(_logger.Object, _displayMock.GetDisplayService);
        var result = await sut.ChangeDisplaySettings(deviceProfile, CancellationToken.None);
        result.Should().BeFalse();
        _displayMock.Mock.Verify(
            m => m.SetStandardDeviceRefreshRate(
                It.IsAny<DISPLAY_DEVICE>(), 
                ref It.Ref<DEVMODE>.IsAny, 
                It.IsAny<int>()), 
            Times.Never);
        _displayMock.Mock.Verify(
            m => m.SetStandardDeviceAsPrimaryDisplay(
                It.IsAny<DISPLAY_DEVICE>(), 
                ref It.Ref<DEVMODE>.IsAny), 
            Times.Never);
        _displayMock.Mock.Verify(
            m => m.SetStandardDeviceDeviceMode(
                It.IsAny<DISPLAY_DEVICE>(), 
                ref It.Ref<DEVMODE>.IsAny), 
            Times.Never);
        _displayMock.Mock.Verify(
            m => m.SetDisplayConfigurationAdvancedColorInformation(
                It.IsAny<DISPLAYCONFIG_DEVICE_INFO_HEADER>(), 
                It.IsAny<bool>()), 
            Times.Never);
        _displayMock.Mock.Verify(m => m.ApplyStandardDeviceChanges(), Times.Never);
    }

    [Test]
    public async Task ChangeDisplaySettings_SetNewPrimaryDisplayAsPrimary_ReturnsTrueAndAppliesCorrectChanges()
    {
        var deviceProfile = new DeviceProfile
        {
            Id = 0,
            Name = "TestProfile1",
            DisplaySettings = new List<DisplaySettings>
            {
                new()
                {
                    DisplayId = 2,
                    PrimaryDisplay = true,
                }
            }
        };

        var sut = new DisplayDeviceService(_logger.Object, _displayMock.GetDisplayService);
        var result = await sut.ChangeDisplaySettings(deviceProfile, CancellationToken.None);
        result.Should().BeTrue();
        _displayMock.devMode0.dmPosition.x.Should().Be(-1920); // From 0,0 to -1920,-1080 as new primary was at 1920,1080
        _displayMock.devMode0.dmPosition.y.Should().Be(-1080);  
        _displayMock.devMode1.dmPosition.x.Should().Be(0); // From 1920,0 to 0,-1080
        _displayMock.devMode1.dmPosition.y.Should().Be(-1080);
        _displayMock.devMode2.dmPosition.x.Should().Be(0); // New primary is always at 0,0
        _displayMock.devMode2.dmPosition.y.Should().Be(0);
        _displayMock.display0.StateFlags.Should().Be(DisplayDeviceStateFlags.AttachedToDesktop);
        _displayMock.display1.StateFlags.Should().Be(DisplayDeviceStateFlags.AttachedToDesktop);
        _displayMock.display2.StateFlags.Should().Be(DisplayDeviceStateFlags.PrimaryDevice);

        _displayMock.Mock.Verify(
            m => m.SetStandardDeviceAsPrimaryDisplay(
                It.Is<DISPLAY_DEVICE>(d => d.DeviceName == _displayMock.display2.DeviceName), 
                ref It.Ref<DEVMODE>.IsAny),
            Times.Once);
        
        _displayMock.Mock.Verify(
            m => m.SetStandardDeviceDeviceMode(
                It.Is<DISPLAY_DEVICE>(d => d.DeviceName == _displayMock.display0.DeviceName),
                ref It.Ref<DEVMODE>.IsAny), 
            Times.Once);
        _displayMock.Mock.Verify(
            m => m.SetStandardDeviceDeviceMode(
                It.Is<DISPLAY_DEVICE>(d => d.DeviceName == _displayMock.display1.DeviceName),
                ref It.Ref<DEVMODE>.IsAny), 
            Times.Once);
        _displayMock.Mock.Verify(m => m.ApplyStandardDeviceChanges(), Times.Once);
        _displayMock.Mock.Verify(
            m => m.SetStandardDeviceAsPrimaryDisplay(
                It.IsAny<DISPLAY_DEVICE>(), 
                ref It.Ref<DEVMODE>.IsAny), 
            Times.Once);
        _displayMock.Mock.Verify(
            m => m.SetStandardDeviceRefreshRate(
                It.IsAny<DISPLAY_DEVICE>(), 
                ref It.Ref<DEVMODE>.IsAny, 
                It.IsAny<int>()), 
            Times.Never);
        _displayMock.Mock.Verify(
            m => m.SetDisplayConfigurationAdvancedColorInformation(
                It.IsAny<DISPLAYCONFIG_DEVICE_INFO_HEADER>(), 
                It.IsAny<bool>()), 
            Times.Never);
    }

    [Test]
    public async Task ChangeDisplaySettings_DisableAdvancedColorModeOnSupportedDisplay_ReturnsTrueAndAppliesCorrectChanges()
    {
        var deviceProfile = new DeviceProfile
        {
            Id = 0,
            Name = "TestProfile1",
            DisplaySettings = new List<DisplaySettings>
            {
                new()
                {
                    DisplayId = 0,
                    EnableHdr = false,
                }
            }
        };

        var sut = new DisplayDeviceService(_logger.Object, _displayMock.GetDisplayService);
        var result = await sut.ChangeDisplaySettings(deviceProfile, CancellationToken.None);
        result.Should().BeTrue();
        _displayMock.colorInfo0.value.Should().HaveFlag(DISPLAYCONFIG_ADVANCED_COLOR_INFO_VALUE_FLAGS.AdvancedColorSupported);
        _displayMock.colorInfo0.value.Should().NotHaveFlag(DISPLAYCONFIG_ADVANCED_COLOR_INFO_VALUE_FLAGS.AdvancedColorEnabled);

        _displayMock.Mock.Verify(
            m => m.SetDisplayConfigurationAdvancedColorInformation(
                It.Is<DISPLAYCONFIG_DEVICE_INFO_HEADER>(h => h.id == _displayMock.colorInfo0.header.id),
                false),
            Times.Once);

        _displayMock.Mock.Verify(m => m.ApplyStandardDeviceChanges(), Times.Never);
        _displayMock.Mock.Verify(
            m => m.SetStandardDeviceAsPrimaryDisplay(
                It.IsAny<DISPLAY_DEVICE>(), 
                ref It.Ref<DEVMODE>.IsAny), 
            Times.Never);
        _displayMock.Mock.Verify(
            m => m.SetStandardDeviceDeviceMode(
                It.IsAny<DISPLAY_DEVICE>(), 
                ref It.Ref<DEVMODE>.IsAny), 
            Times.Never);
        _displayMock.Mock.Verify(
            m => m.SetStandardDeviceRefreshRate(
                It.IsAny<DISPLAY_DEVICE>(), 
                ref It.Ref<DEVMODE>.IsAny, 
                It.IsAny<int>()), 
            Times.Never);
    }

    [Test]
    public async Task ChangeDisplaySettings_EnableAdvancedColorModeOnSupportedDisplay_ReturnsTrueAndAppliesCorrectChanges()
    {
        var deviceProfile = new DeviceProfile
        {
            Id = 0,
            Name = "TestProfile1",
            DisplaySettings = new List<DisplaySettings>
            {
                new()
                {
                    DisplayId = 2,
                    EnableHdr = true,
                }
            }
        };

        var sut = new DisplayDeviceService(_logger.Object, _displayMock.GetDisplayService);
        var result = await sut.ChangeDisplaySettings(deviceProfile, CancellationToken.None);
        result.Should().BeTrue();
        _displayMock.colorInfo2.value.Should().HaveFlag(DISPLAYCONFIG_ADVANCED_COLOR_INFO_VALUE_FLAGS.AdvancedColorSupported);
        _displayMock.colorInfo2.value.Should().HaveFlag(DISPLAYCONFIG_ADVANCED_COLOR_INFO_VALUE_FLAGS.AdvancedColorEnabled);

        _displayMock.Mock.Verify(
            m => m.SetDisplayConfigurationAdvancedColorInformation(
                It.Is<DISPLAYCONFIG_DEVICE_INFO_HEADER>(h => h.id == _displayMock.colorInfo2.header.id),
                true),
            Times.Once);

        _displayMock.Mock.Verify(m => m.ApplyStandardDeviceChanges(), Times.Never);
        _displayMock.Mock.Verify(
            m => m.SetStandardDeviceAsPrimaryDisplay(
                It.IsAny<DISPLAY_DEVICE>(), 
                ref It.Ref<DEVMODE>.IsAny), 
            Times.Never);
        _displayMock.Mock.Verify(
            m => m.SetStandardDeviceDeviceMode(
                It.IsAny<DISPLAY_DEVICE>(), 
                ref It.Ref<DEVMODE>.IsAny), 
            Times.Never);
        _displayMock.Mock.Verify(
            m => m.SetStandardDeviceRefreshRate(
                It.IsAny<DISPLAY_DEVICE>(), 
                ref It.Ref<DEVMODE>.IsAny,
                It.IsAny<int>()), 
            Times.Never);
    }

    [Test]
    public async Task ChangeDisplaySettings_EnableAdvancedColorModeOnUnsupportedDisplay_ReturnsFalseAndDoesNotApplyChanges()
    {
        var deviceProfile = new DeviceProfile
        {
            Id = 0,
            Name = "TestProfile1",
            DisplaySettings = new List<DisplaySettings>
            {
                new()
                {
                    DisplayId = 1,
                    EnableHdr = true,
                }
            }
        };

        var sut = new DisplayDeviceService(_logger.Object, _displayMock.GetDisplayService);
        var result = await sut.ChangeDisplaySettings(deviceProfile, CancellationToken.None);
        result.Should().BeFalse();
        
        _displayMock.Mock.Verify(
            m => m.SetDisplayConfigurationAdvancedColorInformation(
                It.IsAny<DISPLAYCONFIG_DEVICE_INFO_HEADER>(),
                It.IsAny<bool>()), 
            Times.Never
            );
        _displayMock.Mock.Verify(m => m.ApplyStandardDeviceChanges(), Times.Never);
        _displayMock.Mock.Verify(
            m => m.SetStandardDeviceAsPrimaryDisplay(
                It.IsAny<DISPLAY_DEVICE>(), 
                ref It.Ref<DEVMODE>.IsAny), 
            Times.Never);
        _displayMock.Mock.Verify(
            m => m.SetStandardDeviceDeviceMode(
                It.IsAny<DISPLAY_DEVICE>(), 
                ref It.Ref<DEVMODE>.IsAny), 
            Times.Never);
        _displayMock.Mock.Verify(
            m => m.SetStandardDeviceRefreshRate(
                It.IsAny<DISPLAY_DEVICE>(), 
                ref It.Ref<DEVMODE>.IsAny, 
                It.IsAny<int>()), 
            Times.Never);
    }

    [Test]
    public async Task ChangeDisplaySettings_EnableAdvancedColorModeOnAlreadyEnabledDisplay_ReturnsFalseAndDoesNotApplyChanges()
    {
        var deviceProfile = new DeviceProfile
        {
            Id = 0,
            Name = "TestProfile1",
            DisplaySettings = new List<DisplaySettings>
            {
                new()
                {
                    DisplayId = 0,
                    EnableHdr = true,
                }
            }
        };

        var sut = new DisplayDeviceService(_logger.Object, _displayMock.GetDisplayService);
        var result = await sut.ChangeDisplaySettings(deviceProfile, CancellationToken.None);
        result.Should().BeFalse();

        _displayMock.Mock.Verify(
            m => m.SetDisplayConfigurationAdvancedColorInformation(
                It.IsAny<DISPLAYCONFIG_DEVICE_INFO_HEADER>(),
                It.IsAny<bool>()),
            Times.Never
        );
        _displayMock.Mock.Verify(m => m.ApplyStandardDeviceChanges(), Times.Never);
        _displayMock.Mock.Verify(
            m => m.SetStandardDeviceAsPrimaryDisplay(
                It.IsAny<DISPLAY_DEVICE>(),
                ref It.Ref<DEVMODE>.IsAny),
            Times.Never);
        _displayMock.Mock.Verify(
            m => m.SetStandardDeviceDeviceMode(
                It.IsAny<DISPLAY_DEVICE>(),
                ref It.Ref<DEVMODE>.IsAny),
            Times.Never);
        _displayMock.Mock.Verify(
            m => m.SetStandardDeviceRefreshRate(
                It.IsAny<DISPLAY_DEVICE>(),
                ref It.Ref<DEVMODE>.IsAny,
                It.IsAny<int>()),
            Times.Never);
    }


    [Test]
    public async Task ChangeDisplaySettings_EnableMultiDisplayProfile_ReturnsTrueAndAppliesCorrectChanges()
    {
        var deviceProfile = new DeviceProfile
        {
            Id = 0,
            Name = "TestProfile1",
            DisplaySettings = new List<DisplaySettings>
            {
                new()
                {
                    DisplayId = 0,
                    RefreshRate = 60,
                    EnableHdr = false,
                },
                new()
                {
                    DisplayId = 1,
                    RefreshRate = 100,
                    PrimaryDisplay = true,
                },
                new()
                {
                    DisplayId = 2,
                    EnableHdr = true,
                    RefreshRate = 144,
                }
            }
        };

        var sut = new DisplayDeviceService(_logger.Object, _displayMock.GetDisplayService);
        var result = await sut.ChangeDisplaySettings(deviceProfile, CancellationToken.None);
        result.Should().BeTrue();
        _displayMock.devMode0.dmDisplayFrequency.Should().Be(60);
        _displayMock.devMode0.dmPosition.x.Should().Be(-1920);
        _displayMock.devMode0.dmPosition.y.Should().Be(0);
        _displayMock.devMode1.dmDisplayFrequency.Should().Be(100);
        _displayMock.devMode1.dmPosition.x.Should().Be(0);
        _displayMock.devMode1.dmPosition.y.Should().Be(0);
        _displayMock.devMode2.dmDisplayFrequency.Should().Be(144);
        _displayMock.devMode2.dmPosition.x.Should().Be(0);
        _displayMock.devMode2.dmPosition.y.Should().Be(1080);
        _displayMock.colorInfo0.value.Should().HaveFlag(DISPLAYCONFIG_ADVANCED_COLOR_INFO_VALUE_FLAGS.AdvancedColorSupported);
        _displayMock.colorInfo0.value.Should().NotHaveFlag(DISPLAYCONFIG_ADVANCED_COLOR_INFO_VALUE_FLAGS.AdvancedColorEnabled);
        _displayMock.colorInfo2.value.Should().HaveFlag(DISPLAYCONFIG_ADVANCED_COLOR_INFO_VALUE_FLAGS.AdvancedColorSupported);
        _displayMock.colorInfo2.value.Should().HaveFlag(DISPLAYCONFIG_ADVANCED_COLOR_INFO_VALUE_FLAGS.AdvancedColorEnabled);
        
        _displayMock.Mock.Verify(
            m => m.SetDisplayConfigurationAdvancedColorInformation(
                It.Is<DISPLAYCONFIG_DEVICE_INFO_HEADER>(h => h.id == _displayMock.colorInfo2.header.id),
                true),
            Times.Once);
        _displayMock.Mock.Verify(
            m => m.SetDisplayConfigurationAdvancedColorInformation(
                It.Is<DISPLAYCONFIG_DEVICE_INFO_HEADER>(h => h.id == _displayMock.colorInfo0.header.id),
                false),
            Times.Once);

        _displayMock.Mock.Verify(
            m => m.SetStandardDeviceAsPrimaryDisplay(
                It.Is<DISPLAY_DEVICE>(d => d.DeviceName == _displayMock.display1.DeviceName),
                ref It.Ref<DEVMODE>.IsAny),
            Times.Once);
        _displayMock.Mock.Verify(
            m => m.SetStandardDeviceDeviceMode(
                It.Is<DISPLAY_DEVICE>(d => d.DeviceName == _displayMock.display0.DeviceName),
                ref It.Ref<DEVMODE>.IsAny),
            Times.Once);
        _displayMock.Mock.Verify(
            m => m.SetStandardDeviceDeviceMode(
                It.Is<DISPLAY_DEVICE>(d => d.DeviceName == _displayMock.display2.DeviceName),
                ref It.Ref<DEVMODE>.IsAny),
            Times.Once);
        _displayMock.Mock.Verify(
            m => m.SetStandardDeviceRefreshRate(
                It.Is<DISPLAY_DEVICE>(d => d.DeviceName == _displayMock.display0.DeviceName),
                ref It.Ref<DEVMODE>.IsAny,
                60),
            Times.Once);
        _displayMock.Mock.Verify(
            m => m.SetStandardDeviceRefreshRate(
                It.Is<DISPLAY_DEVICE>(d => d.DeviceName == _displayMock.display1.DeviceName),
                ref It.Ref<DEVMODE>.IsAny,
                100),
            Times.Once);
        _displayMock.Mock.Verify(
            m => m.SetStandardDeviceRefreshRate(
                It.Is<DISPLAY_DEVICE>(d => d.DeviceName == _displayMock.display2.DeviceName),
                ref It.Ref<DEVMODE>.IsAny,
                144),
            Times.Once);

        _displayMock.Mock.Verify(
            m => m.SetStandardDeviceAsPrimaryDisplay(
                It.IsAny<DISPLAY_DEVICE>(),
                ref It.Ref<DEVMODE>.IsAny),
            Times.Once);
        _displayMock.Mock.Verify(
            m => m.SetStandardDeviceDeviceMode(
                It.IsAny<DISPLAY_DEVICE>(),
                ref It.Ref<DEVMODE>.IsAny),
            Times.Exactly(2));
        _displayMock.Mock.Verify(
            m => m.SetStandardDeviceRefreshRate(
                It.IsAny<DISPLAY_DEVICE>(),
                ref It.Ref<DEVMODE>.IsAny,
                It.IsAny<int>()),
            Times.Exactly(3));

        _displayMock.Mock.Verify(
            m => m.ApplyStandardDeviceChanges(), 
            Times.Once);
    }
}