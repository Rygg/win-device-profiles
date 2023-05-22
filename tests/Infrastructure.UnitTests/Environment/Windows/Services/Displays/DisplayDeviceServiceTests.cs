using System.ComponentModel;
using Domain.Models;
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
    public async Task ChangeDisplaySettings_ChangeToSameRefreshRate_ReturnsTrueAndDoesNotApplyChanges()
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

        _displayMock.Mock.Verify(m => m.SetStandardDeviceRefreshRate(
            It.Is<DISPLAY_DEVICE>(d => d.DeviceName == _displayMock.display0.DeviceName),
            ref It.Ref<DEVMODE>.IsAny,
            144), Times.Once());

        _displayMock.Mock.Verify(m => m.SetStandardDeviceRefreshRate(
            It.IsAny<DISPLAY_DEVICE>(),
            ref It.Ref<DEVMODE>.IsAny,
            It.IsAny<int>()), Times.Once);

        _displayMock.Mock.Verify(m => m.SetStandardDeviceAsPrimaryDisplay(It.IsAny<DISPLAY_DEVICE>(), ref It.Ref<DEVMODE>.IsAny), Times.Never);
        _displayMock.Mock.Verify(m => m.SetStandardDeviceDeviceMode(It.IsAny<DISPLAY_DEVICE>(), ref It.Ref<DEVMODE>.IsAny), Times.Never);
        _displayMock.Mock.Verify(m => m.SetDisplayConfigurationAdvancedColorInformation(It.IsAny<DISPLAYCONFIG_DEVICE_INFO_HEADER>(), It.IsAny<bool>()), Times.Never);
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

        _displayMock.Mock.Verify(m => m.SetStandardDeviceRefreshRate(
            It.Is<DISPLAY_DEVICE>(d => d.DeviceName == _displayMock.display0.DeviceName),
            ref It.Ref<DEVMODE>.IsAny,
            154), Times.Once());

        _displayMock.Mock.Verify(m => m.SetStandardDeviceRefreshRate(
            It.IsAny<DISPLAY_DEVICE>(),
            ref It.Ref<DEVMODE>.IsAny,
            It.IsAny<int>()), Times.Once);

        _displayMock.Mock.Verify(m => m.SetStandardDeviceAsPrimaryDisplay(It.IsAny<DISPLAY_DEVICE>(), ref It.Ref<DEVMODE>.IsAny), Times.Never);
        _displayMock.Mock.Verify(m => m.SetStandardDeviceDeviceMode(It.IsAny<DISPLAY_DEVICE>(), ref It.Ref<DEVMODE>.IsAny), Times.Never);
        _displayMock.Mock.Verify(m => m.SetDisplayConfigurationAdvancedColorInformation(It.IsAny<DISPLAYCONFIG_DEVICE_INFO_HEADER>(), It.IsAny<bool>()), Times.Never);
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

        _displayMock.Mock.Verify(m => m.SetStandardDeviceRefreshRate(
            It.Is<DISPLAY_DEVICE>(d => d.DeviceName == _displayMock.display0.DeviceName),
            ref It.Ref<DEVMODE>.IsAny,
            154), Times.Once());

        _displayMock.Mock.Verify(m => m.SetStandardDeviceRefreshRate(
            It.IsAny<DISPLAY_DEVICE>(),
            ref It.Ref<DEVMODE>.IsAny,
            It.IsAny<int>()), Times.Once);

        _displayMock.Mock.Verify(m => m.SetStandardDeviceAsPrimaryDisplay(It.IsAny<DISPLAY_DEVICE>(), ref It.Ref<DEVMODE>.IsAny), Times.Never);
        _displayMock.Mock.Verify(m => m.SetStandardDeviceDeviceMode(It.IsAny<DISPLAY_DEVICE>(), ref It.Ref<DEVMODE>.IsAny), Times.Never);
        _displayMock.Mock.Verify(m => m.SetDisplayConfigurationAdvancedColorInformation(It.IsAny<DISPLAYCONFIG_DEVICE_INFO_HEADER>(), It.IsAny<bool>()), Times.Never);
        _displayMock.Mock.Verify(m => m.ApplyStandardDeviceChanges(), Times.Never);
    }
}