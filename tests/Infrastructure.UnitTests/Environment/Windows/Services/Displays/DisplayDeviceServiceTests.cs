using Infrastructure.Environment.Windows.Services.Displays;
using Infrastructure.UnitTests.Environment.Windows.Mocks;
using Microsoft.Extensions.Logging;

namespace Infrastructure.UnitTests.Environment.Windows.Services.Displays;

[TestFixture]
public sealed class DisplayDeviceServiceTests
{
    private readonly Mock<ILogger<DisplayDeviceService>> _logger = new();

    [SetUp]
    public void SetUp()
    {
        DisplayServiceMock.ClearInvocations();
        DisplayServiceMock.SetDefaultBehavior();
    }

    [Test]
    public async Task GetCurrentDisplayInformationString_ValidData_ReturnsCorrectDataInString()
    {
        var sut = new DisplayDeviceService(_logger.Object, DisplayServiceMock.GetDisplayService);
        var str = await sut.GetCurrentDisplayInformationString(CancellationToken.None);
        str.Should().Contain(DisplayServiceMock.Display0.DeviceName);
        str.Should().Contain(DisplayServiceMock.Display0.DeviceString);
        str.Should().Contain(DisplayServiceMock.Display0.DeviceKey);
        str.Should().Contain(DisplayServiceMock.Display0.StateFlags.ToString());
        str.Should().Contain(DisplayServiceMock.TargetDevice0.monitorFriendlyDeviceName);
        str.Should().Contain($"{DisplayServiceMock.DevMode0.dmPelsWidth}x{DisplayServiceMock.DevMode0.dmPelsHeight}@{DisplayServiceMock.DevMode0.dmDisplayFrequency}Hz");
        str.Should().Contain($"{DisplayServiceMock.DevMode0.dmPosition.x},{DisplayServiceMock.DevMode0.dmPosition.y}");
        str.Should().Contain(DisplayServiceMock.ColorInfo0.colorEncoding.ToString());
        str.Should().Contain(DisplayServiceMock.ColorInfo0.bitsPerColorChannel.ToString());
        str.Should().Contain(DisplayServiceMock.ColorInfo0.value.ToString());
        str.Should().Contain(DisplayServiceMock.Display1.DeviceName);
        str.Should().Contain(DisplayServiceMock.Display1.DeviceString);
        str.Should().Contain(DisplayServiceMock.Display1.DeviceKey);
        str.Should().Contain(DisplayServiceMock.Display1.StateFlags.ToString());
        str.Should().Contain(DisplayServiceMock.TargetDevice1.monitorFriendlyDeviceName);
        str.Should().Contain($"{DisplayServiceMock.DevMode1.dmPelsWidth}x{DisplayServiceMock.DevMode1.dmPelsHeight}@{DisplayServiceMock.DevMode1.dmDisplayFrequency}Hz");
        str.Should().Contain($"{DisplayServiceMock.DevMode1.dmPosition.x},{DisplayServiceMock.DevMode1.dmPosition.y}");
        str.Should().Contain(DisplayServiceMock.ColorInfo1.colorEncoding.ToString());
        str.Should().Contain(DisplayServiceMock.ColorInfo1.bitsPerColorChannel.ToString());
        str.Should().Contain(DisplayServiceMock.ColorInfo1.value.ToString());
        str.Should().Contain(DisplayServiceMock.Display2.DeviceName);
        str.Should().Contain(DisplayServiceMock.Display2.DeviceString);
        str.Should().Contain(DisplayServiceMock.Display2.DeviceKey);
        str.Should().Contain(DisplayServiceMock.Display2.StateFlags.ToString());
        str.Should().Contain(DisplayServiceMock.TargetDevice2.monitorFriendlyDeviceName);
        str.Should().Contain($"{DisplayServiceMock.DevMode2.dmPelsWidth}x{DisplayServiceMock.DevMode2.dmPelsHeight}@{DisplayServiceMock.DevMode2.dmDisplayFrequency}Hz");
        str.Should().Contain($"{DisplayServiceMock.DevMode2.dmPosition.x},{DisplayServiceMock.DevMode2.dmPosition.y}");
        str.Should().Contain(DisplayServiceMock.ColorInfo2.colorEncoding.ToString());
        str.Should().Contain(DisplayServiceMock.ColorInfo2.bitsPerColorChannel.ToString());
        str.Should().Contain(DisplayServiceMock.ColorInfo2.value.ToString());
        str.Should().NotContain(DisplayServiceMock.Display3.DeviceName); // Not attached to desktop.
        str.Should().NotContain(DisplayServiceMock.Display3.DeviceString);
        str.Should().NotContain(DisplayServiceMock.Display3.DeviceKey);

        DisplayServiceMock.Mock.Verify(m => m.GetDisplayDevice(0), Times.Once);
        DisplayServiceMock.Mock.Verify(m => m.GetDisplayDevice(1), Times.Once);
        DisplayServiceMock.Mock.Verify(m => m.GetDisplayDevice(2), Times.Once);
        DisplayServiceMock.Mock.Verify(m => m.GetDisplayDevice(3), Times.Once);
        DisplayServiceMock.Mock.Verify(m => m.GetDisplayDeviceMode(DisplayServiceMock.Display0), Times.Once);
        DisplayServiceMock.Mock.Verify(m => m.GetDisplayDeviceMode(DisplayServiceMock.Display1), Times.Once);
        DisplayServiceMock.Mock.Verify(m => m.GetDisplayDeviceMode(DisplayServiceMock.Display2), Times.Once);
        DisplayServiceMock.Mock.Verify(m => m.GetDisplayDeviceMode(DisplayServiceMock.Display3), Times.Never);
    }
}